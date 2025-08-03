using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LiteRP.Core.Enums;
using LiteRP.Core.Helpers;
using LiteRP.Core.Models;
using LiteRP.Core.Models.CharacterCard;
using LiteRP.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace LiteRP.Core.Services;

public class CharacterService : ICharacterService
{
    private static readonly Regex DialogueTurnRegex = new(
        @"^\s*\{\{(char|user)\}\}\s*:\s*", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly IAvatarService _avatarService;
    private readonly ILogger<CharacterService> _logger;

    public CharacterService(IAvatarService avatarService, ILogger<CharacterService> logger)
    {
        _avatarService = avatarService;
        _logger = logger;
    }

    public async Task<List<Character>> GetCharactersAsync()
    {
        var characters = new List<Character>();
        var files = Directory.GetFiles(PathManager.CharactersDataPath, "*.json");

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var character = JsonSerializer.Deserialize<Character>(json);
                if (character != null)
                {
                    characters.Add(character);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading character file {file}: {ex.Message}");
            }
        }
        return characters;
    }
    
    public async Task<Character?> GetCharacterAsync(Guid id)
    {
        var filePath = PathManager.GetCharacterFilePath(id);
        if (!File.Exists(filePath))
        {
            _logger.LogError("Can't find character by its id");
            return null;
        }
        
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<Character>(json);
    }

    public async Task SaveCharacterAsync(Character character)
    {
        var filePath = PathManager.GetCharacterFilePath(character.Id);
        var json = JsonSerializer.Serialize(character, JsonHelper.SerializerOptions);
        await File.WriteAllTextAsync(filePath, json);
    }
    
    public async Task DeleteCharacterAsync(Guid id)
    {
        await _avatarService.DeleteAvatarAsync(id);

        var filePath = PathManager.GetCharacterFilePath(id);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async Task SetAvatarStatusAsync(Guid characterId, bool hasAvatar)
    {
        var character = await GetCharacterAsync(characterId);
        if (character == null) return;

        character.HasAvatar = hasAvatar;
        if (hasAvatar)
        {
            character.AvatarVersion++;
        }
        
        await SaveCharacterAsync(character);
    }

    public async Task<(Character? character, Lorebook? lorebook)> ParseTavernAiCardAsync(Stream imageStream)
    {
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        try
        {
            using var image = await Image.LoadAsync(memoryStream);

            if (image.Metadata.DecodedImageFormat != PngFormat.Instance)
            {
                return (null, null);
            }

            var pngMetadata = image.Metadata.GetPngMetadata();

            var charaChunk = pngMetadata.TextData.FirstOrDefault(chunk => chunk.Keyword == "chara");
            if (charaChunk.Keyword != "chara")
            {
                return (null, null);
            }

            var base64String = charaChunk.Value;
            var jsonDataBytes = Convert.FromBase64String(base64String);
            var jsonString = Encoding.UTF8.GetString(jsonDataBytes);

            return ParseCharacterJson(jsonString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to parse character card.");
            return (null, null);
        }
    }

    private static (Character? character, Lorebook? lorebook) ParseCharacterJson(string jsonString)
    {
        // At first try V2
        try
        {
            var cardV2 = JsonSerializer.Deserialize<TavernCardV2>(jsonString);
            if (cardV2?.Data != null && !string.IsNullOrWhiteSpace(cardV2.Data.Name))
            {
                return ToDomainModels(cardV2);
            }
        }
        catch { /* Ignore it to try V1 */ }

        // If not V2, then try V1
        try
        {
            var cardV1 = JsonSerializer.Deserialize<TavernCardV1>(jsonString);
            if (!string.IsNullOrWhiteSpace(cardV1?.Name))
            {
                return (ToDomainModel(cardV1), null);
            }
        }
        catch { /* Ignore an error */ }

        // This is not TavernAI character card
        return (null, null);
    }

    /// <summary>
    /// Converts a TavernCardV2 DTO into the application's domain models.
    /// A V2 card can contain an embedded lorebook, which is returned as a separate object.
    /// </summary>
    /// <param name="cardV2">The V2 card DTO to convert.</param>
    /// <returns>A tuple containing the mapped Character and an optional Lorebook if one was present.</returns>
    public static (Character Character, Lorebook? EmbeddedLorebook) ToDomainModels(TavernCardV2 cardV2)
    {
        if (cardV2.Data == null)
        {
            throw new ArgumentNullException(nameof(cardV2.Data), "V2 card data cannot be null.");
        }

        var data = cardV2.Data;

        var character = new Character
        {
            // Core Identity Fields
            Name = data.Name.Trim(),
            Description = data.Description.Trim(),
            Personality = data.Personality.Trim(),
            Scenario = data.Scenario.Trim(),

            // Metadata
            Creator = data.Creator.Trim(),
            CreatorNotes = data.CreatorNotes.Trim(),
            Tags = [..data.Tags],
            Prompt = new CharacterPrompt
            {
                Greetings = BuildGreetings(data.FirstMessage, data.AlternateGreetings),
                SystemPrompt = data.SystemPrompt.Trim(),
                PostHistoryInstructions = data.PostHistoryInstructions.Trim(),
                ExampleOfDialogues = data.MessageExample //ParseExampleOfDialogues(data.MessageExample)
            }
        };

        Lorebook? embeddedLorebook = null;
        if (data.CharacterBook != null)
        {
            embeddedLorebook = MapToLorebook(data.CharacterBook);
            character.LorebookId = embeddedLorebook.Id;
        }

        return (character, embeddedLorebook);
    }

    /// <summary>
    /// Converts a TavernCardV1 DTO into the application's Character domain model.
    /// V1 cards do not support many of the advanced features of V2.
    /// </summary>
    /// <param name="cardV1">The V1 card DTO to convert.</param>
    /// <returns>The mapped Character domain model.</returns>
    public static Character ToDomainModel(TavernCardV1 cardV1)
    {
        var character = new Character
        {
            Name = cardV1.Name,
            Description = cardV1.Description,
            Personality = cardV1.Personality,
            Scenario = cardV1.Scenario,
            Prompt = new CharacterPrompt
            {
                Greetings = BuildGreetings(cardV1.FirstMessage),
                ExampleOfDialogues = cardV1.MessageExample // ParseExampleOfDialogues(cardV1.MessageExample)
            }
        };

        return character;
    }

    private static List<string> BuildGreetings(string firstMessage, List<string>? alternateGreetings = null)
    {
        var greetings = new List<string>();

        firstMessage = firstMessage.Trim();

        if (!string.IsNullOrEmpty(firstMessage))
            greetings.Add(firstMessage);

        if (alternateGreetings != null)
            greetings.AddRange(alternateGreetings
                .Select(g => g.Trim())
                .Where(g => !string.IsNullOrEmpty(g)));

        return greetings;
    }

    /// <summary>
    /// Parses the flat string of example messages into a structured list.
    /// </summary>
    private static List<string> ParseExampleOfDialogues(string? messageExample)
    {
        if (string.IsNullOrWhiteSpace(messageExample))
        {
            return [];
        }

        //var exampleMessages = new List<ExampleMessage>();
        
        // Split conversation blocks, trim whitespace from each block
        var blocks = messageExample.Split("<START>", StringSplitOptions.RemoveEmptyEntries)
                                   .Select(b => b.Trim())
                                   .ToList();

        //foreach (var block in blocks)
        //{
        //    // Split each block into lines
        //    var lines = block.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        //    foreach (var line in lines)
        //    {
        //        var match = DialogueTurnRegex.Match(line);
        //        if (!match.Success) continue;

        //        var senderText = match.Groups[1].Value;
        //        var content = line[match.Length..].Trim();
                
        //        if (Enum.TryParse<MessageSender>(senderText, true, out var sender))
        //        {
        //            exampleMessages.Add(new ExampleMessage
        //            {
        //                Sender = sender,
        //                Content = content
        //            });
        //        }
        //    }
        //}

        return blocks;
    }

    /// <summary>
    /// Maps a CharacterBook DTO to a Lorebook domain model.
    /// </summary>
    private static Lorebook MapToLorebook(CharacterBook bookDto)
    {
        var lorebook = new Lorebook
        {
            // Id is generated by default in the constructor
            Name = bookDto.Name,
            Description = bookDto.Description,
            ScanDepth = bookDto.ScanDepth,
            TokenBudget = bookDto.TokenBudget,
            RecursiveScanning = bookDto.RecursiveScanning,
            Entries = bookDto.Entries.Select(MapToLorebookEntry).ToList()
        };

        return lorebook;
    }

    /// <summary>
    /// Maps a CharacterBookEntry DTO to a LorebookEntry domain model.
    /// </summary>
    private static LorebookEntry MapToLorebookEntry(CharacterBookEntry entryDto)
    {
        return new LorebookEntry
        {
            Keys = [.. entryDto.Keys ],
            Content = entryDto.Content,
            IsEnabled = entryDto.Enabled,
            InsertionOrder = entryDto.InsertionOrder,
            Priority = entryDto.Priority,
            IsCaseSensitive = entryDto.CaseSensitive ?? false, // Default to false if null
            IsConstant = entryDto.Constant ?? false,
            IsSelective = entryDto.Selective ?? false,
            SecondaryKeys = [.. entryDto.SecondaryKeys ?? Enumerable.Empty<string>()],
            Position = MapPosition(entryDto.Position),
            Name = entryDto.Name,
            Comment = entryDto.Comment
        };
    }

    /// <summary>
    /// Safely maps the nullable CardPosition enum to the non-nullable LorebookEntryPosition enum.
    /// </summary>
    private static LorebookEntryPosition MapPosition(CardPosition? position)
    {
        return position switch
        {
            CardPosition.AfterChar => LorebookEntryPosition.AfterCharacter,
            _ => LorebookEntryPosition.BeforeCharacter
        };
    }
}
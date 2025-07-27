using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LiteRP.Core.Enums;
using LiteRP.Core.Exceptions;
using LiteRP.Core.Helpers;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LiteRP.Core.Entities;

public class ChatSession
{
    private readonly Character _character;
    private readonly ISettingsService _settingsService;
    private readonly ChatHistory _history;
    
    private ChatSession(Character character, ISettingsService settingsService, ChatHistory chatHistory)
    {
        _character = character;
        _settingsService = settingsService;
        _history = chatHistory;
    }

    public static async Task<ChatSession> CreateAsync(
        Character character,
        ISettingsService settingsService,
        ChatSessionState? state = null)
    {
        var history = await CreateInitialHistoryAsync(character, settingsService, state);
        return new ChatSession(character, settingsService, history);
    }

    private static async Task<ChatHistory> CreateInitialHistoryAsync(
        Character character,
        ISettingsService settingsService,
        ChatSessionState? state)
    {
        var history = new ChatHistory();

        if (state == null)
        {
            var settings = await settingsService.GetSettingsAsync();

            var charName = character.Name;
            var userName = settings.UserName;

            var sb = new StringBuilder()
                .AppendLine($"You are {charName}. Your goal is to engage in a roleplay conversation with {userName}.")
                .AppendLine();
            
            void Section(string title, string? content)
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    sb.AppendLine($"## {title}")
                        .AppendLine(content)
                        .AppendLine();
                }
            }

            Section($"{charName}'s Description", character.Description);
            Section($"{charName}'s Personality", character.Personality);
            Section("Scenario", character.Scenario);
            Section("Example dialogue", character.Prompt.ExampleOfDialogues);
            Section("Rules", settings.SystemPrompt);

            var finalSystemPrompt = ReplacePlaceholders(sb.ToString(), charName, userName);
            history.AddSystemMessage(finalSystemPrompt);

            if (!string.IsNullOrWhiteSpace(character.Prompt.Greetings[0]))
            {
                var finalGreeting = ReplacePlaceholders(character.Prompt.Greetings[0], charName, userName);
                history.AddAssistantMessage(finalGreeting);
            }
        }
        else
        {
            foreach (var message in state.Messages)
            {
                history.AddMessage(message.Role.ToAuthorRole(), message.Text);
            }
        }

        return history;
    }

    private static string ReplacePlaceholders(string text, string charName, string userName)
    {
        // Using a case-insensitive replace is more robust.
        return text
            .Replace("{{char}}", charName, StringComparison.InvariantCultureIgnoreCase)
            .Replace("<BOT>", charName, StringComparison.InvariantCultureIgnoreCase)
            .Replace("{{user}}", userName, StringComparison.InvariantCultureIgnoreCase)
            .Replace("<USER>", userName, StringComparison.InvariantCultureIgnoreCase);
    }

    public async IAsyncEnumerable<string> GetStreamingResponseAsync(
        string userInput,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _history.AddUserMessage(userInput);

        // We need to get new settings each time, because they might change during ChatSession lifetime.
        var settings = await _settingsService.GetSettingsAsync();
        
        if (string.IsNullOrEmpty(settings.OllamaUrl) || string.IsNullOrEmpty(settings.ModelName))
        {
            throw new ChatSessionException(
                ChatSessionError.ConfigurationMissing,
                "Ollama URL and Model Name must be configured in settings.");
        }

        var kernelBuilder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0070
        kernelBuilder.AddOllamaChatCompletion(
            modelId: settings.ModelName,
            endpoint: new Uri(settings.OllamaUrl)
        );
#pragma warning restore SKEXP0070
        var kernel = kernelBuilder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        var resultStream = chatCompletionService.GetStreamingChatMessageContentsAsync(
            _history,
            kernel: kernel,
            cancellationToken: cancellationToken
        );

        var response = new StringBuilder();
        await using var enumerator = resultStream.GetAsyncEnumerator(cancellationToken);

        while (true)
        {
            StreamingChatMessageContent? content = null;
            try
            {
                if (!await enumerator.MoveNextAsync())
                {
                    break;
                }

                content = enumerator.Current;
            }
            catch (HttpRequestException ex)
            {
                throw new ChatSessionException(
                    ChatSessionError.ConnectionFailed,
                    $"Failed to connect to Ollama at {settings.OllamaUrl}",
                    ex);
            }

            if (!string.IsNullOrEmpty(content.Content))
            {
                response.Append(content.Content);
                yield return content.Content;
            }
        }

        _history.AddAssistantMessage(response.ToString());
    }

    public ChatSessionState ToState()
    {
        return new ChatSessionState
        {
            CharacterId = _character.Id,
            Messages = _history
                .Select(message => new ChatMessage(message.Role.ToChatRole(), message.Content))
                .ToList()
        };
    }
}
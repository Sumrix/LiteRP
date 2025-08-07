using LiteRP.Core.Models;
using LiteRP.WebApp.Components.Organisms;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace LiteRP.WebApp.Components.Pages;

public partial class Characters
{
    private List<Character>? _characters = null;
    // private string _sort = "Name (A-Z)";
    private bool _isDragOver;
    private int _dragCounter = 0;
    private DeleteCharacterModal _deleteModal = null!;
    private CharacterViewModal _characterModal = null!;

    private void HandleDragEnter()
    {
        _dragCounter++;
        if (_dragCounter > 0)
        {
            _isDragOver = true;
            StateHasChanged();
        }
    }

    private void HandleDragLeave()
    {
        _dragCounter--;
        if (_dragCounter <= 0)
        {
            _dragCounter = 0;
            _isDragOver = false;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _characters = await CharacterService.GetCharactersAsync();
    }

    private async Task OpenPicker()
    {
        await JS.InvokeVoidAsync("LiteRP.openFileDialog", "filePicker");
    }

    private async Task UploadCharacter(InputFileChangeEventArgs e)
    {
        _dragCounter = 0;
        _isDragOver = false;
        
        try
        {
            foreach (var file in e.GetMultipleFiles(int.MaxValue))
            {
                if (file.Size > AvatarSettings.Value.MaxAllowedUploadBytes)
                {
                    ToastService.ShowError($"File is too large (max {AvatarSettings.Value.MaxAllowedUploadMegabytes} MB)");
                    continue;
                }

                await using var imageStream = file.OpenReadStream(maxAllowedSize: AvatarSettings.Value.MaxAllowedUploadBytes);

                using var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);

                // Attempt to parse character data from the image
                memoryStream.Position = 0;
                var (parsedCharacter, parsedLorebook) = await CharacterService.ParseTavernAiCardAsync(memoryStream);

                if (parsedCharacter == null)
                {
                    ToastService.ShowError($"Couldn't upload {file.Name}, because it not TavernAI V2 character card.");
                    continue;
                }

                parsedCharacter.HasAvatar = true;
                if (parsedLorebook != null)
                {
                    parsedCharacter.LorebookId = parsedLorebook.Id;
                }

                await CharacterService.SaveCharacterAsync(parsedCharacter);

                memoryStream.Position = 0;
                await AvatarService.SavePermanentAvatarAsync(parsedCharacter.Id, memoryStream);

                _characters!.Add(parsedCharacter);

                if (parsedLorebook != null)
                {
                    await LorebookService.SaveLorebookAsync(parsedLorebook);
                }
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error processing file: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    private void ShowDeleteCharacterModal(Character character)
    {
        _deleteModal.ShowDialog(character);
    }

    private void ShowCharacterViewModal(Character character)
    {
        _characterModal.ShowDialog(character);
    }
    
    private async Task DeleteCharacter(Character character)
    {
        await CharacterService.DeleteCharacterAsync(character.Id);
        _characters!.Remove(character);
    }
    
    private void OpenChat(Character character)
    {
        Nav.NavigateTo($"/chat/new/{character.Id}");
    }
}
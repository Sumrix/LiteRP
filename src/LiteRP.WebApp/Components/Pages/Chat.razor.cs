using LiteRP.Core.Entities;
using LiteRP.Core.Exceptions;
using LiteRP.Core.Models;
using LiteRP.WebApp.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LiteRP.WebApp.Components.Pages;

public partial class Chat : IDisposable
{
    [Parameter, EditorRequired]
    public Guid CharacterIdForNewChat { get; set; }

    private List<ChatMessageViewModel> _chatMessageViewModels = new();
    private string _userInput = String.Empty;
    private Character _character = null!;
    private ChatSession _chatSession = null!;
    private readonly CancellationTokenSource _cts = new();

    protected override async Task OnParametersSetAsync()
    {
        var character = await CharacterService.GetCharacterAsync(CharacterIdForNewChat);
        if (character == null)
        {
            Nav.NavigateTo("/not-found", replace: true);
        }
        else
        {
            _character = character;
            _chatSession = await ChatSessionService.CreateNewSessionAsync(character);
            _chatMessageViewModels = _chatSession.ToState().Messages
                .Where(message => message.Role is ChatRole.Assistant or ChatRole.User)
                .Select(message => ChatMessageViewModel.FromChatMessage(message, character))
                .ToList();
        }
    }
    
    private async Task HandleKey(KeyboardEventArgs e)
    {
        if (e is { Key: "Enter", ShiftKey: false })
        {
            await SendMessage();
        }
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(_userInput)) return;

        _chatMessageViewModels.Add(ChatMessageViewModel.UserMessage(_userInput));

        var aiResponse = ChatMessageViewModel.AiMessage("", _character);
        _chatMessageViewModels.Add(aiResponse);

        StateHasChanged();
        await ScrollToBottom();

        try 
        {
            var stream = _chatSession.GetStreamingResponseAsync(_userInput, _cts.Token);
            _userInput = "";
            
            StateHasChanged();
            await ScrollToBottom();

            await foreach (var chunk in stream.WithCancellation(_cts.Token))
            {
                aiResponse.MessageText += chunk;
                StateHasChanged();
                await ScrollToBottom();
            }
        }
        catch (ChatSessionException ex)
        {
            var errorMessage = GetUserFriendlyErrorMessage(ex.ErrorCode);
            Logger.LogError(ex, errorMessage);
            ToastService.ShowError(errorMessage);
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Chat generation was canceled.");
        }
        catch (Exception ex)
        {
            const string errorMessage = "An unexpected error occurred. Please try again later.";
            Logger.LogError(ex, errorMessage);
            ToastService.ShowError(errorMessage);
        }
        finally
        {
            StateHasChanged();
        }
    }

    private string GetUserFriendlyErrorMessage(ChatSessionError code)
    {
        return code switch
        {
            ChatSessionError.ConfigurationMissing => "AI settings are not configured. Please go to the settings page to set the Ollama URL and model name.",
            ChatSessionError.ConnectionFailed => "Could not connect to the AI server. Please check your settings and ensure the server is running.",
            ChatSessionError.ModelNotAvailable => "The selected AI model is not available on the server.",
            _ => "An unknown error occurred with the AI service."
        };
    }

    [Inject] private IJSRuntime JS { get; set; } = null!;
    private async Task ScrollToBottom()
    {
        try
        {
            await JS.InvokeVoidAsync("LiteRP.scrollToBottom");
        }
        catch (Exception ex)
        {
            // We don't want it to crash the app. The user can just scroll manually.
            Logger.LogWarning(ex, "Failed to scroll to bottom.");
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
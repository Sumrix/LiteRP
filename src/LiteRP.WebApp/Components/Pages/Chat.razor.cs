using LiteRP.Core.Entities;
using LiteRP.Core.Enums;
using LiteRP.Core.Exceptions;
using LiteRP.Core.Models;
using LiteRP.WebApp.Helpers;
using LiteRP.WebApp.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LiteRP.WebApp.Components.Pages;

public partial class Chat : IDisposable
{
    [Parameter, EditorRequired]
    public Guid CharacterIdForNewChat { get; set; }

    private List<ChatMessageViewModel> _chatMessageViewModels = [];
    private Character _character = null!;
    private ChatSession _chatSession = null!;
    private AppSettings _appSettings = null!;

    private string _userInput = String.Empty;
    private CancellationTokenSource _messageGenerationCts = new();
    private bool _isAiResponding;
    private bool _shouldPreventDefault;

    protected override async Task OnInitializedAsync()
    {
        _appSettings = await SettingsService.GetSettingsAsync();
    }

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
                .Select(message => ChatMessageViewModel.FromChatMessage(message, _appSettings.UserName, character))
                .ToList();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
           var dotNetObjectReference = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsyncWithErrorHandling("LiteRP.submitOnEnter", "chat-input", dotNetObjectReference);
        }
    }

    [JSInvokable]
    public async Task OnSubmitFromJs()
    {
        await SendMessage();
    }

    private async Task HandleKey(KeyboardEventArgs e)
    {
        if (e is { Key: "Enter", ShiftKey: false })
        {
            _shouldPreventDefault = true;
            await SendMessage();
        }
        else
        {
            _shouldPreventDefault = false;
        }
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(_userInput) && _chatMessageViewModels.Last().Sender.SenderType == SenderType.Ai
            || _isAiResponding) return;

        _messageGenerationCts = new();
        var userMessage = _userInput;
        _userInput = "";

        var aiResponseViewModel = PrepareUIForNewMessage(userMessage);
        var wasMessagePopulated = false;

        try
        {
            wasMessagePopulated = await StreamAndDisplayAiResponseAsync(userMessage, aiResponseViewModel, _messageGenerationCts.Token);
        }
        catch (ChatSessionException ex)
        {
            _chatMessageViewModels.Remove(aiResponseViewModel);
            var errorMessage = GetUserFriendlyErrorMessage(ex.ErrorCode);
            Logger.LogError(ex, "ChatSessionException: {ErrorMessage}", errorMessage);
            ToastService.ShowError(errorMessage, 10000);
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Chat generation was canceled.");
        }
        catch (Exception ex)
        {
            _chatMessageViewModels.Remove(aiResponseViewModel);
            const string errorMessage = "An unexpected error occurred. Please try again later.";
            Logger.LogError(ex, "UnexpectedException: {ErrorMessage}", errorMessage);
            ToastService.ShowError(errorMessage, 10000);
        }
        finally
        {
            // If the operation was cancelled BUT no content was ever received, remove the placeholder.
            if (!wasMessagePopulated)
            {
                _chatMessageViewModels.Remove(aiResponseViewModel);
            }

            _isAiResponding = false;
            StateHasChanged();
        }
    }

    private ChatMessageViewModel PrepareUIForNewMessage(string userMessage)
    {
        _isAiResponding = true;

        if (!string.IsNullOrWhiteSpace(userMessage))
            _chatMessageViewModels.Add(ChatMessageViewModel.UserMessage(_appSettings.UserName, userMessage));
    
        // Create the placeholder for the AI's response
        var aiResponseViewModel = ChatMessageViewModel.AiMessage("", _character);
        _chatMessageViewModels.Add(aiResponseViewModel);
    
        StateHasChanged();

        // Return the placeholder so we can track it
        return aiResponseViewModel;
    }

    private async Task<bool> StreamAndDisplayAiResponseAsync(string userMessage, ChatMessageViewModel aiResponseViewModel, CancellationToken token)
    {
        await ScrollToBottom();

        var stream = _chatSession.GetStreamingResponseAsync(userMessage, token);
        var hasReceivedAnyChunks = false;

        await foreach (var chunk in stream)
        {
            // As soon as we get the first chunk, set the flag.
            if (!hasReceivedAnyChunks)
            {
                hasReceivedAnyChunks = true;
            }

            aiResponseViewModel.MessageText += chunk;
            StateHasChanged();
            await ScrollToBottom();
        }

        // Return true if we added any content to the message.
        return hasReceivedAnyChunks;
    }

    private void StopResponding()
    {
        _messageGenerationCts.Cancel();
    }

    private static string GetUserFriendlyErrorMessage(ChatSessionError code)
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
        _messageGenerationCts.Cancel();
        _messageGenerationCts.Dispose();
        GC.SuppressFinalize(this);
    }
}
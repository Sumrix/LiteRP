using LiteRP.Core.Entities;
using LiteRP.Core.Enums;
using LiteRP.Core.Exceptions;
using LiteRP.Core.Models;
using LiteRP.WebApp.Helpers;
using LiteRP.WebApp.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LiteRP.WebApp.Components.Pages;

public partial class Chat : IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = null!;

    [Parameter] public Guid? CharacterIdForNewChat { get; set; }
    [Parameter] public Guid? ChatSessionId { get; set; }

    private List<ChatMessageViewModel> _chatMessageViewModels = [];
    private Character _character = null!;
    private ChatSession _chatSession = null!;
    private AppSettings _appSettings = null!;

    private bool CanSendMessage => 
        (!string.IsNullOrWhiteSpace(_userInput) ||
         _chatMessageViewModels.LastOrDefault()?.Sender.SenderType != SenderType.Ai) &&
        !_isAiResponding &&
        _isAiConnected;
    
    private const string InputId = "chat-input";
    private string _userInput = string.Empty;
    private CancellationTokenSource _messageGenerationCts = new();
    private bool _isAiResponding;
    private bool _isAiConnected;

    protected override async Task OnInitializedAsync()
    {
        OllamaStatusService.StatusChanged += HandleOllamaStatusChanged;
        OllamaStatusService.StartMonitoring(this);
        _isAiConnected = OllamaStatusService.Status != ConnectionStatus.Failed;
        _appSettings = await SettingsService.GetSettingsAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (ChatSessionId.HasValue)
        {
            var chatSession = await ChatSessionService.LoadSessionAsync(ChatSessionId.Value);
            if (chatSession == null)
            {
                Nav.NavigateTo("/not-found", replace: true);
                return;
            }

            _chatSession = chatSession;
            _character = _chatSession.Character;
        }
        else if (CharacterIdForNewChat.HasValue)
        {
            var character = await CharacterService.GetCharacterAsync(CharacterIdForNewChat.Value);
            if (character == null)
            {
                Nav.NavigateTo("/not-found", replace: true);
                return;
            }

            _character = character;
            _chatSession = await ChatSessionService.CreateNewSessionAsync(character);
        }
        
        _chatMessageViewModels = _chatSession.GetState().Messages
            .Where(message => message.Role is ChatRole.Assistant or ChatRole.User)
            .Select(message => ChatMessageViewModel.FromChatMessage(message, _appSettings.UserName, _character))
            .ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("LiteRP.autoScroll.start", "main-content");
            
            var dotNetObjectReference = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsyncWithErrorHandling("LiteRP.submitOnEnter", InputId, dotNetObjectReference);
        }
    }

    private void HandleOllamaStatusChanged(ConnectionStatus status)
    {
        var oldIsAiConnected = _isAiConnected;

        if (_isAiConnected)
        {
            if (status is ConnectionStatus.Failed or ConnectionStatus.Unknown)
            {
                _isAiConnected = false;
            }
        }
        else
        {
            if (status is ConnectionStatus.Success)
            {
                _isAiConnected = true;
            }
        }

        if (oldIsAiConnected != _isAiConnected)
        {
            InvokeAsync(StateHasChanged).CatchAndLog();
        }
    }

    [JSInvokable]
    public void OnSubmitFromJs()
    {
        SendMessage();
    }

    private void SendMessage()
    {
        if (!CanSendMessage) return;

        _messageGenerationCts = new();
        var userMessage = _userInput;
        _userInput = "";

        var aiResponseViewModel = PrepareUIForNewMessage(userMessage);

        _ = Task.Run(async () => await StreamAndDisplayAiResponseAsync(userMessage, aiResponseViewModel));
    }

    private async Task StreamAndDisplayAiResponseAsync(string userMessage, ChatMessageViewModel aiResponseViewModel)
    {
        var wasMessagePopulated = false;

        try
        {
            var stream = _chatSession.GetStreamingResponseAsync(userMessage, _messageGenerationCts.Token);

            await foreach (var chunk in stream)
            {
                if (!wasMessagePopulated)
                {
                    wasMessagePopulated = true;
                }
                
                await InvokeAsync(() =>
                {
                    if (aiResponseViewModel.Mode == MessageDisplayMode.Thinking)
                        aiResponseViewModel.Mode = MessageDisplayMode.Streaming;

                    aiResponseViewModel.MessageText += chunk;
                    StateHasChanged();
                });
            }
        }
        catch (ChatSessionException ex)
        {
            await InvokeAsync(() =>
            {
                _chatMessageViewModels.Remove(aiResponseViewModel);
                var errorMessage = GetUserFriendlyErrorMessage(ex.ErrorCode);
                Logger.LogError(ex, "ChatSessionException: {ErrorMessage}", errorMessage);
                ToastService.ShowError(errorMessage, 10000);
            });
            await OllamaStatusService.TriggerCheckAsync();
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Chat generation was canceled.");
        }
        catch (Exception ex)
        {
            await InvokeAsync(() =>
            {
                _chatMessageViewModels.Remove(aiResponseViewModel);
                const string errorMessage = "An unexpected error occurred. Please try again later.";
                Logger.LogError(ex, "UnexpectedException: {ErrorMessage}", errorMessage);
                ToastService.ShowError(errorMessage, 10000);
            });
        }
        finally
        {
            if (wasMessagePopulated)
            {
                await ChatSessionService.SaveSessionAsync(_chatSession);
            }

            await InvokeAsync(() =>
            {
                // If the operation was cancelled BUT no content was ever received, remove the placeholder.
                if (!wasMessagePopulated)
                {
                    _chatMessageViewModels.Remove(aiResponseViewModel);
                }
                else
                {
                    if (ChatSessionId == null)
                    {
                        Nav.NavigateTo($"/chat/{_chatSession.Id}", replace: true);
                    }
                }
                
                aiResponseViewModel.Mode = MessageDisplayMode.Ready;
                _isAiResponding = false;
                StateHasChanged();
            });
        }
    }

    private ChatMessageViewModel PrepareUIForNewMessage(string userMessage)
    {
        _isAiResponding = true;

        if (!string.IsNullOrWhiteSpace(userMessage))
            _chatMessageViewModels.Add(ChatMessageViewModel.UserMessage(_appSettings.UserName, userMessage));
    
        // Create the placeholder for the AI's response
        var aiResponseViewModel = ChatMessageViewModel.AiMessage("", _character);
        aiResponseViewModel.Mode = MessageDisplayMode.Thinking;
        _chatMessageViewModels.Add(aiResponseViewModel);
    
        StateHasChanged();

        // Return the placeholder so we can track it
        return aiResponseViewModel;
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
            ChatSessionError.ResponseInterrupted => "The AI's response was interrupted. Please check Ollama server status and try again.",
            ChatSessionError.ModelNotAvailable => "The selected AI model is not available on the server.",
            _ => "An unknown error occurred with the AI service."
        };
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("LiteRP.autoScroll.stop");
        }
        catch (JSDisconnectedException)
        {
            // We don't care
        }

        await _messageGenerationCts.CancelAsync();
        _messageGenerationCts.Dispose();

        OllamaStatusService.StopMonitoring(this);
        OllamaStatusService.StatusChanged -= HandleOllamaStatusChanged;

        GC.SuppressFinalize(this);
    }
}
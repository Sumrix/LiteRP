//using System;
//using System.Threading.Tasks;
//using LiteRP.Core.Services.Interfaces;
//using Microsoft.Extensions.AI;

//namespace LiteRP.Core.Services;

//public class ChatService : IChatService
//{
//    private readonly ISettingsService _settingsService;
//    private readonly IChatHistoryService _chatHistoryService;

//    public event Action? OnChange;
//    public string StreamingAiResponse { get; private set; }
//    public bool IsLoading { get; private set; }

//    public ChatService(ISettingsService settingsService, IChatHistoryService chatHistoryService)
//    {
//        _settingsService = settingsService;
//        _chatHistoryService = chatHistoryService;
//    }

//    public async Task SendMessageAsync(string userMessage)
//    {
//    //    var settings = await _settingsService.GetSettingsAsync();
        
//    //    if (string.IsNullOrWhiteSpace(userMessage)) return;

//    //    IsLoading = true;
//    //    StreamingAiResponse = "";

//    //    _chatHistoryService.DeleteChatSessionAsync( ).Add(new ChatMessage(ChatRole.User, userMessage));
//    //    OnChange();
//    //    await ScrollToBottomAsync();

//    //    try
//    //    {
//    //        var responseStream = ChatClient.GetStreamingResponseAsync(_chatHistory);

//    //        await foreach (var messagePart in responseStream)
//    //        {
//    //            StreamingAiResponse += messagePart.Text;
//    //            OnChange();
//    //            await ScrollToBottomAsync();
//    //        }

//    //        if (!string.IsNullOrEmpty(_streamingAiResponse))
//    //        {
//    //            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, _streamingAiResponse));
//    //        }
//    //        _streamingAiResponse = "";
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        await Console.Error.WriteLineAsync($"Error getting AI response: {ex}");
//    //        _errorMessage = $"An error occurred: {ex.Message}";

//    //        if (_chatHistory.Count > 0 && _chatHistory[^1].Role == ChatRole.User)
//    //        {
//    //            _chatHistory.RemoveAt(_chatHistory.Count - 1);
//    //            _errorMessage += " (Your last message was removed from history due to the error.)";
//    //        }
//    //    }
//    //    finally
//    //    {
//    //        _isLoading = false;
//    //        OnChange();
//    //        await ScrollToBottomAsync();
//    //        await FocusInputAsync();
//    //    }
//    }
//}
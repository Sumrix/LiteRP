using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LiteRP.Core.Exceptions;
using LiteRP.Core.Services.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;

namespace LiteRP.Core.Services;

public class AIChatService : IAIChatService
{
    private readonly Kernel _kernel;
    private readonly ISettingsService _settingsService;
    private readonly IHttpClientFactory _httpClientFactory;

    public AIChatService(Kernel kernel, ISettingsService settingsService, IHttpClientFactory httpClientFactory)
    {
        _kernel = kernel;
        _settingsService = settingsService;
        _httpClientFactory = httpClientFactory;
    }

    public async IAsyncEnumerable<string> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetSettingsAsync();

        if (string.IsNullOrEmpty(settings.OllamaUrl) || string.IsNullOrEmpty(settings.ModelName))
        {
            throw new AIChatServiceException(
                AIChatServiceError.ConfigurationMissing,
                "Ollama URL and Model Name must be configured in settings.");
        }

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(settings.OllamaUrl);

        using var ollamaClient = new OllamaApiClient(httpClient, settings.ModelName);

#pragma warning disable SKEXP0001
        var chatCompletionService = ollamaClient.AsChatCompletionService();
#pragma warning restore SKEXP0001

        var history = new ChatHistory();
        foreach (var message in messages)
        {
            history.Add(new ChatMessageContent(
                message.Role == ChatRole.User ? AuthorRole.User : AuthorRole.Assistant, 
                message.Text
            ));
        }

        IAsyncEnumerable<StreamingChatMessageContent> resultStream;
        try
        {
            resultStream = chatCompletionService.GetStreamingChatMessageContentsAsync(
                history,
                kernel: _kernel,
                cancellationToken: cancellationToken
            );
        }
        catch (HttpRequestException ex)
        {
            throw new AIChatServiceException(
                AIChatServiceError.ConnectionFailed,
                $"Failed to connect to Ollama at {settings.OllamaUrl}", 
                ex);
        }

        await foreach (var content in resultStream.WithCancellation(cancellationToken))
        {
            if (!string.IsNullOrEmpty(content.Content))
            {
                yield return content.Content;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LiteRP.Core.Helpers;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LiteRP.Core.Services;

public class ChatSessionStateService : IChatSessionStateService
{
    private readonly ILogger<ChatSessionStateService> _logger;

    public ChatSessionStateService(ILogger<ChatSessionStateService> logger)
    {
        _logger = logger;
    }

    public async Task<List<ChatSessionMetadata>> GetAllSessionMetadataAsync()
    {
        var index = await GetIndexAsync();
        return index.Sessions.Values.ToList();
    }

    public async Task<ChatSessionState?> GetSessionAsync(Guid id)
    {
        var filePath = PathManager.GetChatSessionFilePath(id);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Attempted to load a non-existent chat session with ID '{id}'", id);
            return null;
        }
        
        var json = await File.ReadAllTextAsync(filePath);
        List<ChatMessage> chatMessages;
        try
        {
            chatMessages = JsonSerializer.Deserialize<List<ChatMessage>>(json, JsonHelper.Context.ListChatMessage)!;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during chat session file parsing '{path}'", filePath);
            return null;
        }

        var index = await GetIndexAsync();
        var metadata = index.Sessions[id];

        return new ChatSessionState()
        {
            Id = id,
            CharacterId = metadata.CharacterId,
            LastModified = metadata.LastModified,
            Messages = chatMessages
        };
    }

    public async Task SaveSessionAsync(ChatSessionState session)
    {
        // Save messages
        var filePath = PathManager.GetChatSessionFilePath(session.Id);
        var json = JsonSerializer.Serialize(session.Messages, JsonHelper.Context.ListChatMessage);
        await File.WriteAllTextAsync(filePath, json);

        // Update metadata in index
        var index = await GetIndexAsync();
        if (index.Sessions.TryGetValue(session.Id, out var metadata))
        {
            metadata.LastModified = session.LastModified;
            metadata.LastMessage = session.Messages.Last().Text;
        }
        else
        {
            metadata = new ChatSessionMetadata
            {
                Id = session.Id,
                CharacterId = session.CharacterId,
                LastMessage = session.Messages.Last().Text,
                LastModified = session.LastModified
            };
        }
        index.Sessions[session.Id] = metadata;

        await SaveIndexAsync(index);
    }
    
    public async Task DeleteChatSessionAsync(Guid id)
    {
        // Delete messages
        var filePath = PathManager.GetChatSessionFilePath(id);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Delete metadata in index
        var index = await GetIndexAsync();
        index.Sessions.Remove(id);
        await SaveIndexAsync(index);
    }

    private async Task<ChatSessionIndex> GetIndexAsync()
    {
        if (!File.Exists(PathManager.GetChatSessionIndexFilePath))
        {
            return new ChatSessionIndex();
        }
        var json = await File.ReadAllTextAsync(PathManager.GetChatSessionIndexFilePath);
        try
        {
            return JsonSerializer.Deserialize<ChatSessionIndex>(json, JsonHelper.Context.ChatSessionIndex) ?? new ChatSessionIndex();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during chat session index file parsing '{path}'", PathManager.GetChatSessionIndexFilePath);
            return new ChatSessionIndex();
        }
    }

    private async Task SaveIndexAsync(ChatSessionIndex index)
    {
        var json = JsonSerializer.Serialize(index, JsonHelper.Context.ChatSessionIndex);
        await File.WriteAllTextAsync(PathManager.GetChatSessionIndexFilePath, json);
    }
}
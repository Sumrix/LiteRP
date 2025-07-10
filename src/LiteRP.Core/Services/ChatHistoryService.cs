using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;

namespace LiteRP.Core.Services;

public class ChatHistoryService : IChatHistoryService
{

    private static readonly string ChatsPath = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LiteRP", "Chats");

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public ChatHistoryService()
    {
        Directory.CreateDirectory(ChatsPath);
    }

    public async Task<List<ChatSession>> GetChatHistoryListAsync()
    {
        var sessions = new List<ChatSession>();
        var files = Directory.GetFiles(ChatsPath, "*.json");

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var session = JsonSerializer.Deserialize<ChatSession>(json);
                if (session != null)
                {
                    sessions.Add(session);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading chat session file {file}: {ex.Message}");
            }
        }

        return sessions.OrderByDescending(s => s.LastModified).ToList();
    }

    public async Task<ChatSession?> GetChatSessionAsync(Guid id)
    {
        var filePath = Path.Combine(ChatsPath, $"{id}.json");
        if (!File.Exists(filePath))
        {
            return null;
        }
        
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<ChatSession>(json);
    }
    
    public async Task SaveChatSessionAsync(ChatSession session)
    {
        session.LastModified = DateTime.UtcNow;

        var filePath = Path.Combine(ChatsPath, $"{session.Id}.json");
        var json = JsonSerializer.Serialize(session, SerializerOptions);
        await File.WriteAllTextAsync(filePath, json);
    }
    
    public Task DeleteChatSessionAsync(Guid id)
    {
        var filePath = Path.Combine(ChatsPath, $"{id}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}
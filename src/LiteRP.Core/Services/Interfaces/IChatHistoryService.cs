using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteRP.Core.Models;

namespace LiteRP.Core.Services.Interfaces;

public interface IChatHistoryService
{
    Task<List<ChatSession>> GetChatHistoryListAsync();
    Task<ChatSession?> GetChatSessionAsync(Guid id);
    Task SaveChatSessionAsync(ChatSession session);
    Task DeleteChatSessionAsync(Guid id);
}
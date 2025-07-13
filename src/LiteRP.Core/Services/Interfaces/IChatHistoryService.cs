using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteRP.Core.Models;

namespace LiteRP.Core.Services.Interfaces;

public interface IChatHistoryService
{
    Task<List<ChatSessionState>> GetChatHistoryListAsync();
    Task<ChatSessionState?> GetChatSessionAsync(Guid id);
    Task SaveChatSessionAsync(ChatSessionState session);
    Task DeleteChatSessionAsync(Guid id);
}
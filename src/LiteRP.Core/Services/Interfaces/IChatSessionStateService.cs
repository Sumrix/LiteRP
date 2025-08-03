using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteRP.Core.Models;

namespace LiteRP.Core.Services.Interfaces;

public interface IChatSessionStateService
{
    Task<List<ChatSessionMetadata>> GetSessionMetadataListAsync(int skip, int take);
    Task<ChatSessionState?> GetSessionAsync(Guid id);
    Task SaveSessionAsync(ChatSessionState session);
    Task DeleteChatSessionAsync(Guid id);
}
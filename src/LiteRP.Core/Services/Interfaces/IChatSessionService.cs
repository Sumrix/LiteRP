using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteRP.Core.Entities;
using LiteRP.Core.Models;

namespace LiteRP.Core.Services.Interfaces;

public interface IChatSessionService
{
    ChatSessionMetadata? NewSession { get; }

    event Action OnChange;

    Task<ChatSession> CreateNewSessionAsync(Character character);
    Task<ChatSession?> LoadSessionAsync(Guid id);
    Task SaveSessionAsync(ChatSession session);
    Task DeleteSessionAsync(Guid id);
    Task<List<ChatSessionMetadata>> GetSessionMetadataListAsync(int skip, int take);
}
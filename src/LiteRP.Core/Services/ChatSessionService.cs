using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteRP.Core.Entities;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;

namespace LiteRP.Core.Services;

public class ChatSessionService : IChatSessionService
{
    private readonly ISettingsService _settingsService;
    private readonly IChatSessionStateService _chatSessionStateService;
    private readonly ICharacterService _characterService;
    
    public ChatSessionMetadata? NewSession { get; private set; }

    public event Action? OnChange;

    public ChatSessionService(
        ISettingsService settingsService, 
        IChatSessionStateService chatSessionStateService, 
        ICharacterService characterService)
    {
        _settingsService = settingsService;
        _chatSessionStateService = chatSessionStateService;
        _characterService = characterService;
    }

    public async Task<ChatSession> CreateNewSessionAsync(Character character)
    {
        var chatSession = await ChatSession.CreateAsync(character, _settingsService);

        NewSession = chatSession.GetMetadata();
        OnChange?.Invoke();

        return chatSession;
    }

    public async Task<ChatSession?> LoadSessionAsync(Guid id)
    {
        var sessionState = await _chatSessionStateService.GetSessionAsync(id);
        if (sessionState == null)
        {
            return null;
        }

        var character = await _characterService.GetCharacterAsync(sessionState.CharacterId);
        if (character == null)
        {
            return null; 
        }

        return await ChatSession.CreateAsync(character, _settingsService, sessionState);
    }

    public async Task SaveSessionAsync(ChatSession session)
    {
        var state = session.GetState();
        await _chatSessionStateService.SaveSessionAsync(state);

        if (NewSession?.Id == state.Id)
        {
            NewSession = null;
        }
        OnChange?.Invoke();
    }

    public async Task DeleteSessionAsync(Guid id)
    {
        await _chatSessionStateService.DeleteChatSessionAsync(id);

        if (NewSession?.Id == id)
        {
            NewSession = null;
        }
        OnChange?.Invoke();
    }

    public async Task<List<ChatSessionMetadata>> GetSessionMetadataListAsync(int skip, int take)
    {
        return await _chatSessionStateService.GetSessionMetadataListAsync(skip, take);
    }
}
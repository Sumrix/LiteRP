using System.Threading.Tasks;
using LiteRP.Core.Entities;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;

namespace LiteRP.Core.Services;

public class ChatSessionService : IChatSessionService
{
    private readonly ISettingsService _settingsService;

    public ChatSessionService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<ChatSession> CreateNewSessionAsync(Character character)
    {
        return await ChatSession.CreateAsync(character, _settingsService);
    }

    //public ChatSession LoadSession(Guid id)
    //{
    //    // TODO
    //}
}
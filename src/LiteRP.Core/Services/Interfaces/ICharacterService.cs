using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LiteRP.Core.Models;

namespace LiteRP.Core.Services.Interfaces;

public interface ICharacterService
{
    Task<(Character? character, Lorebook? lorebook)> ParseTavernAiCardAsync(Stream imageStream);
    Task<List<Character>> GetCharactersAsync();
    Task<Character?> GetCharacterAsync(Guid id);
    Task SaveCharacterAsync(Character character);
    Task DeleteCharacterAsync(Guid id);
    Task SetAvatarStatusAsync(Guid characterId, bool hasAvatar);
}
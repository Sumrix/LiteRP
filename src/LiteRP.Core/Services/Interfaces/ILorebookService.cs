using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteRP.Core.Models;

namespace LiteRP.Core.Services.Interfaces;

public interface ILorebookService
{
    Task<List<Lorebook>> GetLorebooksAsync();
    Task<Lorebook?> GetLorebookAsync(Guid id);
    Task SaveLorebookAsync(Lorebook lorebook);
    Task DeleteLorebookAsync(Guid id);
}
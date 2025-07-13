using System.Threading.Tasks;
using LiteRP.Core.Entities;
using LiteRP.Core.Models;

namespace LiteRP.Core.Services.Interfaces;

public interface IChatSessionService
{
    Task<ChatSession> CreateNewSessionAsync(Character character);
}
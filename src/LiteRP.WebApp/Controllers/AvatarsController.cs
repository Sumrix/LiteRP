using LiteRP.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace LiteRP.WebApp.Controllers;

[ApiController]
[Route("/characters/{characterId:guid}/avatar")]
public class AvatarsController : ControllerBase
{
    private readonly IAvatarService _avatarService;

    public AvatarsController(IAvatarService avatarService)
    {
        _avatarService = avatarService;
    }

    [HttpGet("{sizeToken}")]
    [OutputCache(PolicyName = "AvatarPolicy")]
    public async Task<IActionResult> GetAvatar(
        Guid characterId, 
        string sizeToken, 
        [FromQuery] int? v, 
        [FromQuery] int dpr = 1)
    {
        var stream = await _avatarService.GetResizedAvatarStreamAsync(characterId, sizeToken, dpr);
        
        if (stream == null)
        {
            return NotFound();
        }
        
        const long oneYearInSeconds = 31536000;
        Response.Headers.CacheControl = $"public,max-age={oneYearInSeconds},immutable";

        return File(stream, "image/webp");
    }
}
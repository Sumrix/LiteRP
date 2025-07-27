using System;
using System.IO;
using System.Threading.Tasks;
using LiteRP.Core.Helpers;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace LiteRP.Core.Services;

public class AvatarService : IAvatarService
{
    private readonly AvatarOptions _options;

    public AvatarService(IOptions<AvatarOptions> avatarSettings)
    {
        _options = avatarSettings.Value;
    }

    public async Task SavePermanentAvatarAsync(Guid characterId, Stream imageStream)
    {
        var permanentPath = GetPermanentAvatarPath(characterId);
        
        using var image = await Image.LoadAsync(imageStream);
        image.Mutate(x => x.AutoOrient());
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(1024, 1536),
            Mode = ResizeMode.Crop,
            PadColor = Color.Transparent
        }));

        image.Metadata.ExifProfile?.SetValue(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.Orientation, (ushort)1);
        
        await image.SaveAsWebpAsync(permanentPath);
    }

    public async Task<Stream?> GetResizedAvatarStreamAsync(Guid characterId, string sizeKey, int multiplier)
    {
        var permanentPath = GetPermanentAvatarPath(characterId);
        if (!File.Exists(permanentPath))
        {
            return null;
        }

        var imageSize = CalculateDimensions(sizeKey, multiplier);

        var image = await Image.LoadAsync(permanentPath);
        image.Mutate(x => x.Resize(imageSize));
        
        var memoryStream = new MemoryStream();
        await image.SaveAsWebpAsync(memoryStream);
        memoryStream.Position = 0; // Важно сбросить позицию потока
        return memoryStream;
    }

    public async Task<string> GetPreviewAvatarAsDataUrlAsync(Stream imageStream, string sizeKey)
    {
        var imageSize = CalculateDimensions(sizeKey, 1);

        using var image = await Image.LoadAsync(imageStream);
        image.Mutate(x => x.AutoOrient());
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = imageSize,
            Mode = ResizeMode.Crop,
            PadColor = Color.Transparent
        }));

        image.Metadata.ExifProfile?.SetValue(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.Orientation, (ushort)1);

        using var memoryStream = new MemoryStream();
        await image.SaveAsWebpAsync(memoryStream);
        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        
        return $"data:image/webp;base64,{base64}";
    }

    public Task DeleteAvatarAsync(Guid characterId)
    {
        var permanentPath = GetPermanentAvatarPath(characterId);
        if (File.Exists(permanentPath))
        {
            File.Delete(permanentPath);
        }
        return Task.CompletedTask;
    }

    private string GetPermanentAvatarPath(Guid characterId)
    {
        return Path.Combine(PathManager.AvatarsDataPath, characterId + ".webp");
    }
    
    private Size CalculateDimensions(string sizeKey, int multiplier)
    {
        if (!_options.BaseWidth.TryGetValue(sizeKey, out var baseWidth) || !_options.AllowedMultipliers.Contains(multiplier))
        {
            sizeKey = "m"; 
            multiplier = 1;
            baseWidth = _options.BaseWidth[sizeKey];
        }

        int width = baseWidth * multiplier;
        int height = (int)(width / _options.AspectRatio);
        return new(width, height);
    }
}
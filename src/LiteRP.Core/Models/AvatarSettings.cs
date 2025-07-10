using System.Collections.Generic;

namespace LiteRP.Core.Models;

public class AvatarSettings
{
    public Dictionary<string, int> BaseWidth { get; set; } = new();
    public double AspectRatio { get; set; }
    public List<int> AllowedMultipliers { get; set; } = new();
    public int MaxAllowedUploadMegabytes { get; set; } = 5;
    public int MaxAllowedUploadBytes => MaxAllowedUploadMegabytes * 1024 * 1024;
}
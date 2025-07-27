using System.Collections.Generic;

namespace LiteRP.Core.Models;

public class AvatarOptions
{
    public Dictionary<string, int> BaseWidth { get; set; } = [];
    public double AspectRatio { get; set; }
    public List<int> AllowedMultipliers { get; set; } = [];
    public int MaxAllowedUploadMegabytes { get; set; } = 5;
    public int MaxAllowedUploadBytes => MaxAllowedUploadMegabytes * 1024 * 1024;
}
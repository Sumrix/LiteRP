using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LiteRP.Core.Helpers;

internal static class JsonHelper
{
    internal static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    internal static readonly AppJsonContext Context = new(
        new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        });
}
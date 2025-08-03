using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LiteRP.Core.Helpers;

public static class JsonHelper
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
}
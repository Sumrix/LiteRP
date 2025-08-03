using System;
using System.IO;

namespace LiteRP.Core.Helpers;

public static class PathManager
{
    private static readonly string RootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LiteRP");

    public static readonly string CharactersDataPath = Path.Combine(RootPath, "Characters");
    public static readonly string LorebooksDataPath = Path.Combine(RootPath, "Lorebooks");
    public static readonly string AvatarsDataPath = Path.Combine(RootPath, "Avatars");
    public static readonly string ChatSessionsPath = Path.Combine(RootPath, "Chats");
    public static readonly string GetChatSessionIndexFilePath = Path.Combine(ChatSessionsPath, "index.json");
    public static readonly string LogsPath = Path.Combine(RootPath, "Logs");
    public static readonly string SettingsFilePath = Path.Combine(RootPath, "settings.json");

    static PathManager()
    {
        Directory.CreateDirectory(RootPath);
        Directory.CreateDirectory(CharactersDataPath);
        Directory.CreateDirectory(AvatarsDataPath);
        Directory.CreateDirectory(ChatSessionsPath);
        Directory.CreateDirectory(LorebooksDataPath);
        Directory.CreateDirectory(LogsPath);
    }

    public static string GetCharacterFilePath(Guid characterId)
    {
        return Path.Combine(CharactersDataPath, $"{characterId}.json");
    }

    public static string GetAvatarFilePath(Guid characterId)
    {
        return Path.Combine(AvatarsDataPath, $"{characterId}.webp");
    }

    public static string GetLorebookFilePath(Guid lorebookId)
    {
        return Path.Combine(LorebooksDataPath, $"{lorebookId}.json");
    }

    public static string GetChatSessionFilePath(Guid chatSessionId)
    {
        return Path.Combine(ChatSessionsPath, $"{chatSessionId}.json");
    }
}
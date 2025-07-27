namespace LiteRP.WebApp;

public static class UrlManager
{
    public static string AvatarPlaceholderUrl => "/img/avatar_placeholder_large.png";

    //public static string GetAvatarUrl(Guid characterId, AvatarSize avatarSize) =>
    //    $"/characters/{characterId}/avatar/{avatarSize}";

    //public static string GetAvatarUrl(Character character, AvatarSize avatarSize) =>
    //    character.HasAvatar
    //        ? GetAvatarUrl(character.Id, avatarSize)
    //        : AvatarPlaceholderUrl;

    //public static string GetAvatarUrl(Character character, string size) =>
    //    character.HasAvatar
    //        ? $"/characters/{character.Id}/avatar/{size}"
    //        : AvatarPlaceholderUrl;
}
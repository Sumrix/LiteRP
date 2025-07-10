//using System;

//namespace LiteRP.Core.Models;

//public readonly record struct AvatarSize(string Value)
//{
//    public static readonly AvatarSize Large = new("large");
//    public static readonly AvatarSize Medium   = new("medium");
//    public static readonly AvatarSize Small    = new("small");

//    public override string ToString() => Value;

//    public static AvatarSize Parse(string value) =>
//        TryParse(value, out var size)
//            ? size
//            : throw new ArgumentException($"Unknown size '{value}'", nameof(value));

//    public static bool TryParse(string? value, out AvatarSize result)
//    {
//        result = value switch
//        {
//            "large" => Large,
//            "medium"   => Medium,
//            "small"    => Small,
//            _          => default
//        };

//        return result != default;
//    }
//}
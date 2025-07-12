using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LiteRP.Core.Models.CharacterCard;

/// <summary>
/// Defines the insertion position for a CharacterBook entry relative to the character's main definitions.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardPosition
{
    /// <summary>
    /// The entry is placed before the character definitions in the prompt.
    /// </summary>
    [EnumMember(Value = "before_char")]
    BeforeChar,

    /// <summary>
    /// The entry is placed after the character definitions in the prompt.
    /// </summary>
    [EnumMember(Value = "after_char")]
    AfterChar
}
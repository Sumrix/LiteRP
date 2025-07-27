namespace LiteRP.Core.Enums;

/// <summary>
/// Defines the insertion position for a Lorebook entry relative to the character's main definitions.
/// </summary>
public enum LorebookEntryPosition
{
    /// <summary>
    /// The entry is placed before the character definitions in the prompt.
    /// </summary>
    BeforeCharacter,

    /// <summary>
    /// The entry is placed after the character definitions in the prompt.
    /// </summary>
    AfterCharacter
}
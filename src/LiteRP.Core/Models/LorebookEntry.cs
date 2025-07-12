using System.Collections.Generic;

namespace LiteRP.Core.Models;

/// <summary>
/// Represents a single entry within a Lorebook.
/// </summary>
public class LorebookEntry
{
    /// <summary>
    /// A list of keywords that trigger the insertion of this entry's content.
    /// </summary>
    public List<string> Keys { get; set; } = new();

    /// <summary>
    /// The text content to be inserted into the prompt when the entry is triggered.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Whether this entry is currently active.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Determines the insertion order when multiple entries are triggered.
    /// Lower numbers are inserted higher (earlier) in the prompt.
    /// </summary>
    public int InsertionOrder { get; set; } = 0;

    /// <summary>
    /// If the token budget is reached, entries with lower priority values are discarded first. Optional.
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// If true, keyword matching is case-sensitive. Defaults to false.
    /// </summary>
    public bool IsCaseSensitive { get; set; } = false;

    /// <summary>
    /// If true, this entry is always inserted into the prompt (within token budget limits),
    /// without needing a keyword trigger.
    /// </summary>
    public bool IsConstant { get; set; } = false;

    /// <summary>
    /// If true, requires a key from both 'Keys' and 'SecondaryKeys' to trigger the entry.
    /// </summary>
    public bool IsSelective { get; set; } = false;

    /// <summary>
    /// Secondary keys used when 'IsSelective' is true. Ignored otherwise.
    /// </summary>
    public List<string> SecondaryKeys { get; set; } = new();

    /// <summary>
    /// Defines where the entry is placed relative to the character definition.
    /// </summary>
    public LorebookEntryPosition Position { get; set; } = LorebookEntryPosition.BeforeCharacter;
    
    /// <summary>
    /// A user-defined name for the entry, for display in the UI.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// A user-defined comment for the entry, for display in the UI.
    /// </summary>
    public string? Comment { get; set; }
}

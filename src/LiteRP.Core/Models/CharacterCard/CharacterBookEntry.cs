using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LiteRP.Core.Models.CharacterCard;

/// <summary>
/// Represents a single entry in a character's lorebook.
/// </summary>
public class CharacterBookEntry
{
    /// <summary>
    /// A list of keywords that trigger the insertion of this entry's content into the prompt.
    /// </summary>
    [JsonPropertyName("keys")]
    public List<string> Keys { get; set; } = new();

    /// <summary>
    /// The text content to be inserted into the prompt when the entry is triggered.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// A place for applications to store extra data that is not part of the specification.
    /// Should default to an empty object.
    /// </summary>
    [JsonPropertyName("extensions")]
    public Dictionary<string, object> Extensions { get; set; } = new();

    /// <summary>
    /// Whether this entry is currently active.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Determines the insertion order when multiple entries are triggered. Lower numbers are inserted higher (earlier) in the prompt.
    /// </summary>
    [JsonPropertyName("insertion_order")]
    public int InsertionOrder { get; set; } = 0;

    /// <summary>
    /// If true, keyword matching is case-sensitive. Optional.
    /// </summary>
    [JsonPropertyName("case_sensitive")]
    public bool? CaseSensitive { get; set; }

    /// <summary>
    /// A user-defined name for the entry, not used in prompt engineering. Optional.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// If the token budget is reached, entries with lower priority values are discarded first. Optional.
    /// </summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }
    
    /// <summary>
    /// A unique identifier for the entry, not used in prompt engineering. Optional.
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// A user-defined comment for the entry, not used in prompt engineering. Optional.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    /// If true, requires a key from both 'keys' and 'secondary_keys' to trigger the entry. Optional.
    /// </summary>
    [JsonPropertyName("selective")]
    public bool? Selective { get; set; }

    /// <summary>
    /// Secondary keys used when 'selective' is true. Ignored otherwise. Optional.
    /// </summary>
    [JsonPropertyName("secondary_keys")]
    public List<string>? SecondaryKeys { get; set; }

    /// <summary>
    /// If true, this entry is always inserted into the prompt (within token budget limits), without needing a keyword trigger. Optional.
    /// </summary>
    [JsonPropertyName("constant")]
    public bool? Constant { get; set; }

    /// <summary>
    /// Defines where the entry is placed relative to the character definition. Optional.
    /// </summary>
    [JsonPropertyName("position")]
    public CardPosition? Position { get; set; }
}
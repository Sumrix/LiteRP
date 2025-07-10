using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LiteRP.Core.Models.CharacterCard;

/// <summary>
/// Represents a lorebook embedded within a V2 character card.
/// </summary>
public class CharacterBook
{
    /// <summary>
    /// The name of the character book. Optional.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// A description for the character book. Optional.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// How many recent chat messages to scan for keywords. (Agnai: "Memory: Chat History Depth"). Optional.
    /// </summary>
    [JsonPropertyName("scan_depth")]
    public int? ScanDepth { get; set; }

    /// <summary>
    /// The maximum number of tokens the lorebook can inject into the context. (Agnai: "Memory: Context Limit"). Optional.
    /// </summary>
    [JsonPropertyName("token_budget")]
    public int? TokenBudget { get; set; }

    /// <summary>
    /// Whether the content of an inserted entry can trigger other entries. Optional.
    /// </summary>
    [JsonPropertyName("recursive_scanning")]
    public bool? RecursiveScanning { get; set; }
    
    /// <summary>
    /// A place for applications to store extra data that is not part of the specification.
    /// Should default to an empty object.
    /// </summary>
    [JsonPropertyName("extensions")]
    public Dictionary<string, object> Extensions { get; set; } = new();

    /// <summary>
    /// The list of lorebook entries.
    /// </summary>
    [JsonPropertyName("entries")]
    public List<CharacterBookEntry> Entries { get; set; } = new();
}
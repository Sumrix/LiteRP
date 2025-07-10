using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LiteRP.Core.Models.CharacterCard;

/// <summary>
/// Represents the `data` object within a V2 TavernAI Character Card.
/// Contains all V1 fields plus new V2 fields.
/// </summary>
/// <remarks>
/// Inherits V1 fields for convenience
/// </remarks>
public class TavernCardV2Data : TavernCardV1
{
    /// <summary>
    /// Notes from the creator for the user. This data is NEVER included in the AI prompt.
    /// </summary>
    [JsonPropertyName("creator_notes")]
    public string CreatorNotes { get; set; } = string.Empty;

    /// <summary>
    /// The system prompt for the character. By default, this MUST override the user's global system prompt.
    /// Can use {{original}} placeholder to include the user's prompt.
    /// </summary>
    [JsonPropertyName("system_prompt")]
    public string SystemPrompt { get; set; } = string.Empty;

    /// <summary>
    /// Instructions inserted after the conversation history, which have a strong effect on AI generation.
    /// By default, this MUST override the user's global "Jailbreak" or "UJB" settings.
    /// Can use {{original}} placeholder to include the user's setting.
    /// </summary>
    [JsonPropertyName("post_history_instructions")]
    public string PostHistoryInstructions { get; set; } = string.Empty;

    /// <summary>
    /// A list of alternate greetings. Frontends should offer a way to "swipe" or choose between these and the main greeting.
    /// </summary>
    [JsonPropertyName("alternate_greetings")]
    public List<string> AlternateGreetings { get; set; } = new();
    
    /// <summary>
    /// A character-specific lorebook. This property is optional and can be null.
    /// </summary>
    [JsonPropertyName("character_book")]
    public CharacterBook? CharacterBook { get; set; }

    /// <summary>
    /// A list of tags for categorizing the character. These SHOULD NOT be used for prompt engineering.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// The name of the card's creator. MUST NOT be used for prompt engineering.
    /// </summary>
    [JsonPropertyName("creator")]
    public string Creator { get; set; } = string.Empty;

    /// <summary>
    /// The version of the character card. Useful for distinguishing between updates of the same character.
    /// MUST NOT be used for prompt engineering.
    /// </summary>
    [JsonPropertyName("character_version")]
    public string CharacterVersion { get; set; } = string.Empty;

    /// <summary>
    /// A flexible space for applications to store extra data that is not part of the specification.
    /// Any unknown key-value pairs here MUST be preserved. Defaults to an empty object.
    /// </summary>
    [JsonPropertyName("extensions")]
    public Dictionary<string, object> Extensions { get; set; } = new();
}
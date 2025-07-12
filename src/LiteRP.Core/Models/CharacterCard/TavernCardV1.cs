using System.Text.Json.Serialization;

namespace LiteRP.Core.Models.CharacterCard;

/// <summary>
/// Represents a V1 TavernAI Character Card. All fields are mandatory and default to an empty string.
/// </summary>
public class TavernCardV1
{
    /// <summary>
    /// The character's name. Used to replace {{char}} or &lt;BOT&gt; placeholders.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The character's description. Should be included in the prompt.
    /// In some frontends, this is also known as "Persona Attributes".
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// A short summary of the character's personality. Should be included in the prompt.
    /// </summary>
    [JsonPropertyName("personality")]
    public string Personality { get; set; } = string.Empty;

    /// <summary>
    /// The context and circumstances of the conversation/scenario. Should be included in the prompt.
    /// </summary>
    [JsonPropertyName("scenario")]
    public string Scenario { get; set; } = string.Empty;

    /// <summary>
    /// The first message sent by the character to start the chat. Also known as the "greeting".
    /// </summary>
    [JsonPropertyName("first_mes")]
    public string FirstMessage { get; set; } = string.Empty;

    /// <summary>
    /// A string containing example conversations to demonstrate the character's speech patterns and behavior.
    /// Conversations are separated by &lt;START&gt;.
    /// </summary>
    [JsonPropertyName("mes_example")]
    public string MessageExample { get; set; } = string.Empty;
}
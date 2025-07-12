using System.Text.Json.Serialization;

namespace LiteRP.Core.Models.CharacterCard;

/// <summary>
/// Represents a V2 TavernAI Character Card. The top-level object for the V2 specification.
/// </summary>
public class TavernCardV2
{
    /// <summary>
    /// The specification identifier. MUST be "chara_card_v2".
    /// </summary>
    [JsonPropertyName("spec")]
    public string Spec { get; set; } = "chara_card_v2";

    /// <summary>
    /// The minor version of the specification. MUST be "2.0".
    /// </summary>
    [JsonPropertyName("spec_version")]
    public string SpecVersion { get; set; } = "2.0";

    /// <summary>
    /// The main data payload of the card.
    /// </summary>
    [JsonPropertyName("data")]
    public TavernCardV2Data Data { get; set; } = new();
}
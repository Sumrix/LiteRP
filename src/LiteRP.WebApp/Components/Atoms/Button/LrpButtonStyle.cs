namespace LiteRP.WebApp.Components.Atoms.Button;

/// <summary>
/// Defines the visual style of a button.
/// </summary>
public enum LrpButtonStyle
{
    /// <summary> Filled button style. </summary>
    Filled,
    /// <summary> Outline button style with a transparent background. </summary>
    Outline,
    /// <summary> Ghost button style with a transparent background and no borders. </summary>
    Ghost,
    /// <summary> Text button style without background and borders. </summary>
    Text,
    /// <summary> Subtle style with a grey border and text that becomes colored on hover. </summary>
    Subtle,
    /// <summary> Subtle style with a grey border and colored text. </summary>
    SubtleColored,
    /// <summary> Borderless version of the Subtle style. </summary>
    SubtleGhost,
    /// <summary> Borderless version of the SubtleColored style. </summary>
    SubtleGhostColored,
    /// <summary>
    /// A special style for buttons placed on top of images or other rich visual content.
    /// It features high-contrast text with a shadow and a translucent background on hover.
    /// The appearance is controlled by the Color parameter.
    /// </summary>
    Overlay
}
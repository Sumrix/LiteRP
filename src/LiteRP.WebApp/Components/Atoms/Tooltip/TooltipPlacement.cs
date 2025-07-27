namespace LiteRP.WebApp.Components.Atoms.Tooltip;

/// <summary>
/// Defines the placement options for the LrpTooltip component.
/// </summary>
public enum TooltipPlacement
{
    /// <summary>
    /// Automatically choose the best placement
    /// </summary>
    Auto,

    /// <summary>
    /// Place the tooltip above the target element
    /// </summary>
    Top,

    /// <summary>
    /// Place the tooltip below the target element
    /// </summary>
    Bottom,

    /// <summary>
    /// Place the tooltip to the left of the target element
    /// </summary>
    Left,

    /// <summary>
    /// Place the tooltip to the right of the target element
    /// </summary>
    Right
}
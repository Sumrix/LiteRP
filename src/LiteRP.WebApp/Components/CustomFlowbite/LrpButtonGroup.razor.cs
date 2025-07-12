using Microsoft.AspNetCore.Components;

namespace LiteRP.WebApp.Components.CustomFlowbite;

/// <summary>
/// Groups multiple LrpButton components together.
/// </summary>
public partial class LrpButtonGroup
{
    /// <summary>
    /// Content of the button group, typically composed of LrpButton components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes to be applied to the button group container.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the CSS classes for the button group container.
    /// </summary>
    private string GroupClasses => CombineClasses("inline-flex");
}
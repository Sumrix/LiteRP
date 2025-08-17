using Microsoft.AspNetCore.Components;

namespace LiteRP.WebApp.Components.Flowbite;

/// <summary>
/// Base class for icon components providing common functionality.
/// </summary>
public abstract class IconBase : FlowbiteComponentBase
{
    /// <summary>
    /// Whether the icon should be hidden from screen readers.
    /// </summary>
    [Parameter]
    public bool AriaHidden { get; set; } = true;

    /// <summary>
    /// Additional attributes to be applied to the SVG element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The stroke width of the SVG icon.
    /// </summary>
    [Parameter]
    public double StrokeWidth { get; set; } = 2.0;

    /// <summary>
    /// Gets the combined CSS classes including base and additional classes.
    /// </summary>
    protected string CombinedClassNames => CombineClasses(Class is not null ? "" : "w-6 h-6 text-gray-800 dark:text-white");
}

using Flowbite.Base;
using Microsoft.AspNetCore.Components;

namespace LiteRP.WebApp.Components.Shared;

public partial class Icon1
{
    /// <summary>
    /// Whether the icon should be hidden from screen readers.
    /// </summary>
    [Parameter]
    public bool AriaHidden { get; set; } = true;

    /// <summary>
    /// The icon to display.
    /// </summary>
    [EditorRequired, Parameter]
    public required IconBase Type { get; set; }

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
    protected string CombinedClassNames => Class is not null ? Class : "w-6 h-6";

    private Dictionary<string, object>? _parameters;

    protected override void OnParametersSet()
    {
        _parameters = (AdditionalAttributes ?? new Dictionary<string, object>())
            .Append(new(nameof(Class), CombinedClassNames))
            .Append(new(nameof(StrokeWidth), StrokeWidth))
            .Append(new(nameof(AriaHidden), AriaHidden))
            .ToDictionary();
    }
}
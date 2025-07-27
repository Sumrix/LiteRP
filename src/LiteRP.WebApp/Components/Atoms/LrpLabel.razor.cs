using Microsoft.AspNetCore.Components;

namespace LiteRP.WebApp.Components.Atoms;

/// <summary>
/// Label component for form fields and other UI elements.
/// </summary>
public partial class LrpLabel
{
    /// <summary>
    /// Gets or sets the text content of the label.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;Label Value="Username" /&gt;
    /// </code>
    /// </example>
    [Parameter] public string? Value { get; set; }

    /// <summary>
    /// Gets or sets whether the label is disabled.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;Label Disabled="true" Value="Inactive Field" /&gt;
    /// </code>
    /// </example>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the child content of the label.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;Label&gt;Custom Content&lt;/Label&gt;
    /// </code>
    /// </example>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes that will be applied to the label element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        // Define the component's internal classes
        const string baseClasses = "text-sm font-medium text-gray-900 dark:text-gray-300";
        string disabledClass = Disabled ? "cursor-not-allowed opacity-50" : "";

        // Use the base class helper to correctly merge with the inherited `Class` property
        return CombineClasses(baseClasses, disabledClass);
    }
}
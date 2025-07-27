using LiteRP.WebApp.Components.Atoms.Button;
using Microsoft.AspNetCore.Components;

namespace LiteRP.WebApp.Components.Molecules.ToggleGroup;

/// <summary>
/// A container for a group of LrpToggleButton components, managing the selected value.
/// This component is generic to support any value type (string, int, enum, etc.).
/// </summary>
/// <typeparam name="TValue">The type of the value to be managed.</typeparam>
public partial class LrpToggleGroup<TValue>
{
    [Parameter] public LrpButtonColor Color { get; set; } = LrpButtonColor.Primary;
    [Parameter] public LrpButtonSize Size { get; set; } = LrpButtonSize.Medium;
    [Parameter] public bool Pill { get; set; }
    
    /// <summary>
    /// The style of the button when it is in the "toggled" (selected) state.
    /// </summary>
    [Parameter]
    public LrpButtonStyle ToggledStyle { get; set; } = LrpButtonStyle.Filled;

    /// <summary>
    /// The style of the button when it is in the "untoggled" (not selected) state.
    /// </summary>
    [Parameter]
    public LrpButtonStyle UntoggledStyle { get; set; } = LrpButtonStyle.Outline;

    /// <summary>
    /// The content of the toggle group, which should consist of LrpToggleButton components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The currently selected value in the group.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// An event callback that is invoked when the selected value changes.
    /// Used for two-way binding with @bind-Value.
    /// </summary>
    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>
    /// The collection of a currently selected values in the group (used for Multiple selection mode).
    /// </summary>
    [Parameter]
    public ICollection<TValue>? Values { get; set; }

    /// <summary>
    /// An event callback that is invoked when the selected values collection changes (used for Multiple selection mode).
    /// </summary>
    [Parameter]
    public EventCallback<ICollection<TValue>?> ValuesChanged { get; set; }

    /// <summary>
    /// Defines the selection behavior of the group. Defaults to Single.
    /// </summary>
    [Parameter]
    public SelectionMode SelectionMode { get; set; } = SelectionMode.Single;

    /// <summary>
    /// Additional attributes to pass to the underlying LrpButtonGroup component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Toggles the selection state of a given value based on the current SelectionMode.
    /// This method is called by child LrpToggleButton components.
    /// </summary>
    /// <param name="valueToToggle">The value from the clicked button.</param>
    internal async Task ToggleValueAsync(TValue? valueToToggle)
    {
        if (valueToToggle == null) return;

        if (SelectionMode == SelectionMode.Single)
        {
            //var newValue = EqualityComparer<TValue>.Default.Equals(Value, valueToToggle)
            //    ? default // Deselect by setting to default (null for reference types)
            //    : valueToToggle; // Select the new value

            if (!EqualityComparer<TValue>.Default.Equals(Value, valueToToggle))
            {
                Value = valueToToggle;
                await ValueChanged.InvokeAsync(Value);
            }
        }
        else // SelectionMode.Multiple
        {
            if (Values == null)
            {
                // This is a developer error; the component is used incorrectly.
                throw new InvalidOperationException($"When {nameof(SelectionMode)} is {nameof(SelectionMode.Multiple)}, the '{nameof(Values)}' parameter must be bound to a non-null collection.");
            }

            // The toggle logic: if it's there, remove it. If not, add it.
            if (Values.Contains(valueToToggle))
            {
                Values.Remove(valueToToggle);
            }
            else
            {
                Values.Add(valueToToggle);
            }

            // Notify the parent component of the change to the collection.
            await ValuesChanged.InvokeAsync(Values);
        }

        StateHasChanged();
    }

    /// <summary>
    /// Checks if a given value is currently selected, accommodating both selection modes.
    /// </summary>
    internal bool IsValueSelected(TValue? value)
    {
        if (value == null) return false;

        return SelectionMode == SelectionMode.Single
            ? EqualityComparer<TValue>.Default.Equals(Value, value)
            : Values?.Contains(value) ?? false;
    }
}

/// <summary>
/// Defines the selection behavior for a toggle group.
/// </summary>
public enum SelectionMode
{
    /// <summary>
    /// Allows only one item to be selected at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Allows multiple items to be selected simultaneously.
    /// </summary>
    Multiple
}
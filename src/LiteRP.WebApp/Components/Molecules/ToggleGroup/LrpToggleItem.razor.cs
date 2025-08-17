using System.Diagnostics.CodeAnalysis;
using LiteRP.WebApp.Components.Atoms.Button;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiteRP.WebApp.Components.Molecules.ToggleGroup;

/// <summary>
/// A button that can be toggled on or off. Designed to be used inside an LrpToggleGroup.
/// It reuses the LrpButton for its visual appearance.
/// </summary>
/// <typeparam name="TValue">The type of the value this button represents.</typeparam>
public partial class LrpToggleItem<TValue>
{
    /// <summary>
    /// The cascading parent toggle group. If this is not null, the button operates as part of a group.
    /// </summary>
    [CascadingParameter]
    private LrpToggleGroup<TValue>? ParentGroup { get; set; }

    /// <summary>
    /// The value associated with this specific toggle button.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// The content to display inside the button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The icon to display in the button.
    /// </summary>
    [Parameter]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public Type? Icon { get; set; }
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Determines if this button is the currently active one in the group.
    /// </summary>
    private bool IsActive => ParentGroup?.IsValueSelected(Value) ?? false;
    
    /// <summary>
    /// Determines the effective visual style to apply to the button based on its active state.
    /// </summary>
    private LrpButtonStyle EffectiveStyle => IsActive ? ParentGroup!.ToggledStyle : ParentGroup!.UntoggledStyle;

    protected override void OnInitialized()
    {
        if (ParentGroup == null)
        {
            throw new InvalidOperationException($"{nameof(LrpToggleItem<TValue>)} must be used inside an {nameof(LrpToggleGroup<TValue>)}.");
        }
    }

    private async Task HandleClick(MouseEventArgs args)
    {
        if (Disabled) return;
        
        // We know ParentGroup is not null due to the check in OnInitialized.
        await ParentGroup!.ToggleValueAsync(Value);
    }
}
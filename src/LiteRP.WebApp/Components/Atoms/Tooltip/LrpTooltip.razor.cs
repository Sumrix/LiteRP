using LiteRP.WebApp.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiteRP.WebApp.Components.Atoms.Tooltip;

public partial class LrpTooltip
{
    protected string WrapperClasses => new CssBuilder("relative group inline-block")
        .AddClass(Class)
        .Build();

    protected string TooltipContainerClasses => new CssBuilder("absolute z-10 w-max pointer-events-none")
        // Visibility Logic
        .AddClass("opacity-100 visible", IsClickTrigger && IsAnimated && _isVisible)
        .AddClass("opacity-0 invisible", IsClickTrigger && IsAnimated && !_isVisible)
        .AddClass("block", IsClickTrigger && !IsAnimated && _isVisible)
        .AddClass("hidden", IsClickTrigger && !IsAnimated && !_isVisible)
        .AddClass("invisible opacity-0 group-hover:opacity-100 group-hover:visible", !IsClickTrigger && IsAnimated)
        .AddClass("hidden group-hover:block", !IsClickTrigger && !IsAnimated)
        // Placement Logic
        .AddClass("top-full left-1/2 -translate-x-1/2 mt-2", Placement == TooltipPlacement.Bottom)
        .AddClass("right-full top-1/2 -translate-y-1/2 mr-2", Placement == TooltipPlacement.Left)
        .AddClass("left-full top-1/2 -translate-y-1/2 ml-2", Placement == TooltipPlacement.Right)
        .AddClass("bottom-full left-1/2 -translate-x-1/2 mb-2", Placement == TooltipPlacement.Top) // Default
        // Style Logic
        .AddClass("inline-block rounded-lg px-3 py-2 text-sm font-medium shadow-sm border border-gray-200 bg-white text-gray-900", Style == "light")
        .AddClass("inline-block rounded-lg px-3 py-2 text-sm font-medium shadow-sm bg-gray-900 text-white dark:bg-gray-700", Style != "light")
        // Animation Logic
        .AddClass("transition-opacity", IsAnimated)
        .AddClass(Animation, IsAnimated)
        .Build();

    protected string ArrowClasses => new CssBuilder("absolute z-10 h-2 w-2 rotate-45")
        // Arrow Placement
        .AddClass("left-1/2 -translate-x-1/2 -top-1", Placement == TooltipPlacement.Bottom)
        .AddClass("top-1/2 -translate-y-1/2 -right-1", Placement == TooltipPlacement.Left)
        .AddClass("top-1/2 -translate-y-1/2 -left-1", Placement == TooltipPlacement.Right)
        .AddClass("left-1/2 -translate-x-1/2 -bottom-1", Placement == TooltipPlacement.Top) // Default
        // Arrow Color
        .AddClass("bg-white border-l border-t border-gray-200", Style == "light")
        .AddClass("bg-gray-900 dark:bg-gray-700", Style != "light")
        .Build();

    private bool IsClickTrigger => Trigger == "click";
    private bool IsAnimated => !string.IsNullOrEmpty(Animation);

    private bool _isVisible;
    private bool _isFocusLeaving;

    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter, EditorRequired] public required string Text { get; set; }
    [Parameter] public string? Animation { get; set; } = "duration-300";
    [Parameter] public bool Arrow { get; set; } = true;
    [Parameter] public TooltipPlacement Placement { get; set; } = TooltipPlacement.Top;
    [Parameter] public string Style { get; set; } = "dark";
    [Parameter] public string Trigger { get; set; } = "hover";
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private async Task HandleClick()
    {
        if (Trigger == "click")
        {
            _isVisible = !_isVisible;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task HandleFocusOut(FocusEventArgs args)
    {
        if (_isFocusLeaving || Trigger != "click") return;
        _isFocusLeaving = true;
        await Task.Delay(100); // Increased delay for reliability
        if (_isFocusLeaving)
        {
            _isVisible = false;
            await InvokeAsync(StateHasChanged);
        }
        _isFocusLeaving = false;
    }

    private async Task HandleKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && _isVisible)
        {
            _isVisible = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}
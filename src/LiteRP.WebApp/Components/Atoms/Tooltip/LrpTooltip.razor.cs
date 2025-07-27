using LiteRP.WebApp.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiteRP.WebApp.Components.Atoms.Tooltip;

public partial class LrpTooltip : IDisposable
{
    protected string WrapperClasses => new CssBuilder("relative group inline-block")
        .AddClass(Class)
        .Build();

    protected string TooltipContainerClasses => new CssBuilder("absolute z-10 w-max pointer-events-none")
        // Visibility Logic
        .AddClass((IsClickTrigger, IsAnimated) switch
        {
            (true, true) => _isVisible ? "opacity-100 visible" : "opacity-0 invisible",
            (true, false) => _isVisible ? "block" : "hidden",
            (false, true) => "invisible opacity-0 group-hover:opacity-100 group-hover:visible",
            (false, false) => "hidden group-hover:block"
        })
        // Placement Logic
        .AddClass(Placement switch
        {
            TooltipPlacement.Bottom => "top-full left-1/2 -translate-x-1/2 mt-2",
            TooltipPlacement.Left => "right-full top-1/2 -translate-y-1/2 mr-2",
            TooltipPlacement.Right => "left-full top-1/2 -translate-y-1/2 ml-2",
            _ => "bottom-full left-1/2 -translate-x-1/2 mb-2" // Top is default
        })
        // Style Logic
        .AddClass(Style switch
        {
            "light" => "inline-block rounded-lg px-3 py-2 text-sm font-medium shadow-sm border border-gray-200 bg-white text-gray-900",
            _ => "inline-block rounded-lg px-3 py-2 text-sm font-medium shadow-sm bg-gray-900 text-white dark:bg-gray-700"
        })
        // Animation Logic
        .AddClass("transition-opacity", IsAnimated)
        .AddClass(Animation, IsAnimated)
        .Build();

    protected string ArrowClasses => new CssBuilder("absolute z-10 h-2 w-2 rotate-45")
        // Arrow Placement
        .AddClass(Placement switch
        {
            TooltipPlacement.Bottom => "left-1/2 -translate-x-1/2 -top-1",
            TooltipPlacement.Left => "top-1/2 -translate-y-1/2 -right-1", // Corrected from -left-1
            TooltipPlacement.Right => "top-1/2 -translate-y-1/2 -left-1", // Corrected from -right-1
            _ => "left-1/2 -translate-x-1/2 -bottom-1" // Top is default
        })
        // Arrow Color
        .AddClass(Style switch
        {
            "light" => "bg-white border-l border-t border-gray-200", // Adjusted for better light style
            _ => "bg-gray-900 dark:bg-gray-700"
        })
        .Build();

    private bool IsClickTrigger => Trigger == "click";
    private bool IsAnimated => !string.IsNullOrEmpty(Animation);

    private bool _isVisible;
    private bool _isDisposed;
    private bool _isFocusLeaving;

    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter, EditorRequired] public string Content { get; set; } = string.Empty;
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
            await Task.CompletedTask;
        }
    }

    private async Task HandleFocusOut(FocusEventArgs args)
    {
        if (_isFocusLeaving || Trigger != "click") return;
        _isFocusLeaving = true;
        await Task.Delay(10);
        if (_isFocusLeaving) _isVisible = false;
        _isFocusLeaving = false;
    }

    private async Task HandleKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && _isVisible)
        {
            _isVisible = false;
            await Task.CompletedTask;
        }
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
        }
    }
}
using System.ComponentModel;
using Flowbite.Base;
using LiteRP.WebApp.Components.Molecules;
using LiteRP.WebApp.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiteRP.WebApp.Components.Atoms.Button;

public partial class LrpButton
{
    protected string ButtonClasses => new CssBuilder(Class)
        .AddClass("group inline-flex items-center justify-center text-center font-medium focus:outline-none")
        // Handle states for being inside a ButtonGroup
        .AddClass("rounded-none -ml-px first:ml-0 border focus:z-10", IsInGroup)
        .AddClass("first:rounded-l-lg last:rounded-r-lg", IsInGroup)
        // Handle standard border radius (Pill or regular)
        .AddClass("rounded-full", Pill && !IsInGroup)
        .AddClass("rounded-lg", !Pill && !IsInGroup)
        // Add size-specific classes
        .AddClass(GetSizeClasses())
        // Add classes based on the button's style (variant and color)
        .AddClass(GetStyleClasses())
        // Add classes for disabled/loading states
        .AddClass("cursor-not-allowed opacity-50", Disabled && Variant != LrpButtonStyle.Overlay)
        .AddClass("opacity-50", Disabled && Variant == LrpButtonStyle.Overlay)
        .AddClass("cursor-wait opacity-75 pointer-events-none", Loading)
        // Add any additional classes passed through attributes
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    /// <summary>
    /// Gets the consolidated CSS classes for the icon element.
    /// </summary>
    protected string IconClasses => new CssBuilder(GetIconSizeClasses())
        .AddClass("mr-2", ChildContent != null)
        .Build();

    private bool IsInGroup => ParentGroup != null;

    private bool IsIconOnly => Icon != null && ChildContent == null;

    private Dictionary<string, object> IconParameters => new()
    {
        { nameof(IconBase.Class), IconClasses },
        { nameof(IconBase.StrokeWidth), IconStrokeWidth }
    };

    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter] public string Type { get; set; } = "button";
    [Parameter] public string? Href { get; set; }
    [Parameter] public string? Target { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Loading { get; set; }
    [Parameter] public bool Pill { get; set; }
    /// <summary>
    /// Set null to manually define button style
    /// </summary>
    [Parameter] public LrpButtonStyle? Variant { get; set; } = LrpButtonStyle.Filled;
    [Parameter] public LrpButtonSize Size { get; set; } = LrpButtonSize.Medium;
    [Parameter] public LrpButtonColor Color { get; set; } = LrpButtonColor.Primary;
    [Parameter] public Type? Icon { get; set; }
    [Parameter] public double IconStrokeWidth { get; set; } = 2;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [CascadingParameter]
    private LrpButtonGroup? ParentGroup { get; set; }

    private string GetIconSizeClasses() => IsIconOnly
        ? Size switch
        {
            LrpButtonSize.Small => "w-5 h-5",
            LrpButtonSize.Medium => "w-6 h-6",
            LrpButtonSize.Large => "w-7 h-7",
            _ => "w-6 h-6"
        }
        : Size switch
        {
            LrpButtonSize.Small => "w-4 h-4",
            LrpButtonSize.Medium => "w-5 h-5",
            LrpButtonSize.Large => "w-6 h-6",
            _ => "w-5 h-5"
        };
    
    private string GetSizeClasses() => IsIconOnly
        ? Size switch
        {
            LrpButtonSize.Small => "p-1.5",
            LrpButtonSize.Medium => "p-2",
            LrpButtonSize.Large => "p-2.5",
            _ => throw new InvalidEnumArgumentException(nameof(Size), (int)Size, typeof(LrpButtonSize))
        }
        : Size switch
        {
            LrpButtonSize.Small => "text-xs px-3 py-2",
            LrpButtonSize.Medium => "text-sm px-5 py-2.5",
            LrpButtonSize.Large => "text-base px-6 py-3",
            _ => throw new InvalidEnumArgumentException(nameof(Size), (int)Size, typeof(LrpButtonSize))
        };
    
    private string GetStyleClasses() => Variant switch
    {
        LrpButtonStyle.Filled => GetFilledColorClasses(),
        LrpButtonStyle.Outline => GetOutlineColorClasses(),
        LrpButtonStyle.Ghost => GetGhostColorClasses(),
        LrpButtonStyle.Text => GetTextColorClasses(),
        LrpButtonStyle.Subtle => GetSubtleColorClasses(),
        LrpButtonStyle.SubtleColored => GetSubtleColoredColorClasses(),
        LrpButtonStyle.SubtleGhost => GetSubtleGhostColorClasses(),
        LrpButtonStyle.SubtleGhostColored => GetSubtleGhostColoredColorClasses(),
        LrpButtonStyle.Overlay => GetOverlayColorClasses(),
        null => string.Empty,
        _ => throw new InvalidEnumArgumentException(nameof(Variant), (int)Variant, typeof(LrpButtonStyle))
    };

    private async Task HandleClick(MouseEventArgs args)
    {
        if (Disabled || Loading)
        {
            return;
        }

        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }
    }

    private string GetFilledColorClasses() => Color switch
    {
        LrpButtonColor.Primary => "text-white bg-primary-700 hover:bg-primary-800 active:ring-4 active:ring-primary-300 dark:bg-primary-600 dark:hover:bg-primary-700 dark:active:ring-primary-800 ",
        LrpButtonColor.Grey => "text-white bg-gray-800 hover:bg-gray-900 active:ring-4 active:ring-gray-300 dark:bg-gray-800 dark:hover:bg-gray-700 dark:active:ring-gray-700 ",
        LrpButtonColor.Green => "text-white bg-green-700 hover:bg-green-800 active:ring-4 active:ring-green-300 dark:bg-green-600 dark:hover:bg-green-700 dark:active:ring-green-800 ",
        LrpButtonColor.Red => "text-white bg-red-700 hover:bg-red-800 active:ring-4 active:ring-red-300 dark:bg-red-600 dark:hover:bg-red-700 dark:active:ring-red-800 ",
        LrpButtonColor.Yellow => "text-white bg-yellow-400 hover:bg-yellow-500 active:ring-4 active:ring-yellow-300 dark:active:ring-yellow-900 ",
        LrpButtonColor.Purple => "text-white bg-purple-700 hover:bg-purple-800 active:ring-4 active:ring-purple-300 dark:bg-purple-600 dark:hover:bg-purple-700 dark:active:ring-purple-900 ",
        _ => throw new InvalidEnumArgumentException(nameof(Color), (int)Color, typeof(LrpButtonColor))
    };

    private string GetOutlineColorClasses() => Color switch
    {
        LrpButtonColor.Primary => "text-primary-700 border border-primary-700 hover:bg-primary-800 hover:text-white active:ring-4 active:ring-primary-300 dark:border-primary-500 dark:text-primary-500 dark:hover:text-white dark:hover:bg-primary-600 dark:active:ring-primary-800 ",
        LrpButtonColor.Grey => "text-gray-900 border border-gray-800 hover:bg-gray-900 hover:text-white active:ring-4 active:ring-gray-300 dark:border-gray-600 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 dark:active:ring-gray-700 ",
        LrpButtonColor.Green => "text-green-700 border border-green-700 hover:bg-green-800 hover:text-white active:ring-4 active:ring-green-300 dark:border-green-500 dark:text-green-500 dark:hover:text-white dark:hover:bg-green-600 dark:active:ring-green-800 ",
        LrpButtonColor.Red => "text-red-700 border border-red-700 hover:bg-red-800 hover:text-white active:ring-4 active:ring-red-300 dark:border-red-500 dark:text-red-500 dark:hover:text-white dark:hover:bg-red-600 dark:active:ring-red-800 ",
        LrpButtonColor.Yellow => "text-yellow-400 border border-yellow-400 hover:bg-yellow-500 hover:text-white active:ring-4 active:ring-yellow-300 dark:border-yellow-300 dark:text-yellow-300 dark:hover:text-white dark:hover:bg-yellow-400 dark:active:ring-yellow-900 ",
        LrpButtonColor.Purple => "text-purple-700 border border-purple-700 hover:bg-purple-800 hover:text-white active:ring-4 active:ring-purple-300 dark:border-purple-500 dark:text-purple-500 dark:hover:text-white dark:hover:bg-purple-600 dark:active:ring-purple-900 ",
        _ => throw new InvalidEnumArgumentException(nameof(Color), (int)Color, typeof(LrpButtonColor))
    };

    private string GetGhostColorClasses() => Color switch
    {
        LrpButtonColor.Primary => "text-primary-700 hover:bg-primary-100 active:ring-4 active:ring-primary-300 dark:text-primary-500 dark:hover:text-primary-400 dark:hover:bg-primary-900/70 dark:active:ring-primary-800 ",
        LrpButtonColor.Grey => "text-gray-500 hover:bg-gray-100 active:ring-4 active:ring-gray-300 dark:text-gray-400 dark:hover:bg-gray-700 dark:active:ring-gray-800 ",
        LrpButtonColor.Green => "text-green-700 hover:bg-green-100 active:ring-4 active:ring-green-300 dark:text-green-500 dark:hover:text-green-400 dark:hover:bg-green-900/70 dark:active:ring-green-800 ",
        LrpButtonColor.Red => "text-red-700 hover:bg-red-100 active:ring-4 active:ring-red-300 dark:text-red-500 dark:hover:text-red-400 dark:hover:bg-red-900/70 dark:active:ring-red-800 ",
        LrpButtonColor.Yellow => "text-yellow-400 hover:bg-yellow-100 active:ring-4 active:ring-yellow-300 dark:text-yellow-300 dark:hover:bg-yellow-900/70 dark:active:ring-yellow-900 ",
        LrpButtonColor.Purple => "text-purple-700 hover:bg-purple-200 active:ring-4 active:ring-purple-300 dark:text-purple-500 dark:hover:text-purple-400 dark:hover:bg-purple-900/70 dark:active:ring-purple-900 ",
        _ => throw new InvalidEnumArgumentException(nameof(Color), (int)Color, typeof(LrpButtonColor))
    };

    private string GetTextColorClasses() => Color switch
    {
        LrpButtonColor.Primary => "text-primary-700 hover:text-primary-900 active:text-primary-900 active:ring-4 active:ring-primary-200 dark:text-primary-500 dark:hover:text-primary-400 dark:active:ring-primary-800 ",
        LrpButtonColor.Grey => "text-gray-500 hover:text-gray-900 active:text-gray-950 active:ring-4 active:ring-gray-200 dark:text-gray-400 dark:hover:text-white dark:active:ring-gray-800 ",
        LrpButtonColor.Green => "text-green-700 hover:text-green-900 active:text-green-900 active:ring-4 active:ring-green-200 dark:text-green-500 dark:hover:text-green-400 dark:active:ring-green-800 ",
        LrpButtonColor.Red => "text-red-700 hover:text-red-900 active:text-red-900 active:ring-4 active:ring-red-200 dark:text-red-500 dark:hover:text-red-400 dark:active:ring-red-800 ",
        LrpButtonColor.Yellow => "text-yellow-400 hover:text-yellow-500 active:text-yellow-600 active:ring-4 active:ring-yellow-200 dark:text-yellow-300 dark:hover:text-yellow-200 dark:active:ring-yellow-900 ",
        LrpButtonColor.Purple => "text-purple-700 hover:text-purple-900 active:text-purple-900 active:ring-4 active:ring-purple-200 dark:text-purple-500 dark:hover:text-purple-400 dark:active:ring-purple-900 ",
        _ => throw new InvalidEnumArgumentException(nameof(Color), (int)Color, typeof(LrpButtonColor))
    };

    private const string SubtleBorderClasses = "border border-gray-300 dark:border-gray-600";
    private const string SubtleHoverBgClasses = "hover:bg-gray-100 dark:hover:bg-gray-700 active:ring-2 active:ring-gray-200 dark:active:ring-gray-600";

    private string GetSubtleColorClasses() => new CssBuilder(SubtleBorderClasses)
        .AddClass(SubtleHoverBgClasses)
        .AddClass(GetSubtleTextColorClasses())
        .Build();

    private string GetSubtleGhostColorClasses() => new CssBuilder(SubtleHoverBgClasses)
        .AddClass(GetSubtleTextColorClasses())
        .Build();

    private string GetSubtleColoredColorClasses() => new CssBuilder(SubtleBorderClasses)
        .AddClass(SubtleHoverBgClasses)
        .AddClass(GetSubtleColoredTextColorClasses())
        .Build();

    private string GetSubtleGhostColoredColorClasses() => new CssBuilder(SubtleHoverBgClasses)
        .AddClass(GetSubtleColoredTextColorClasses())
        .Build();

    private string GetSubtleTextColorClasses()
    {
        var baseTextColor = "text-gray-900 dark:text-gray-200 ";
        var hoverTextColor = Color switch
        {
            LrpButtonColor.Primary => "hover:text-primary-700 dark:hover:text-primary-400 ",
            LrpButtonColor.Grey    => "hover:text-gray-900 dark:hover:text-white ",
            LrpButtonColor.Green   => "hover:text-green-600 dark:hover:text-green-500 ",
            LrpButtonColor.Red     => "hover:text-red-600 dark:hover:text-red-500 ",
            LrpButtonColor.Yellow  => "hover:text-yellow-500 dark:hover:text-yellow-400 ",
            LrpButtonColor.Purple  => "hover:text-purple-600 dark:hover:text-purple-500 ",
            _ => throw new InvalidEnumArgumentException(nameof(Color), (int)Color, typeof(LrpButtonColor))
        };
        return baseTextColor + hoverTextColor;
    }

    private string GetSubtleColoredTextColorClasses() => Color switch
    {
        LrpButtonColor.Primary => "text-primary-700 dark:text-primary-400 ",
        LrpButtonColor.Grey    => "text-gray-900 dark:text-white ",
        LrpButtonColor.Green   => "text-green-600 dark:text-green-500 ",
        LrpButtonColor.Red     => "text-red-600 dark:text-red-500 ",
        LrpButtonColor.Yellow  => "text-yellow-500 dark:text-yellow-400 ",
        LrpButtonColor.Purple  => "text-purple-600 dark:text-purple-500 ",
        _ => throw new InvalidEnumArgumentException(nameof(Color), (int)Color, typeof(LrpButtonColor))
    };

    private string GetOverlayColorClasses() => new CssBuilder("drop-shadow-[0_1.5px_1.5px_rgba(0,0,0,0.6)]")
        .AddClass(Color switch
        {
            LrpButtonColor.Grey => "text-white hover:bg-black/30 dark:hover:bg-black/50 active:bg-black/40 dark:active:bg-black/60",
            LrpButtonColor.Primary => "text-primary-400 hover:bg-primary-950/30 active:bg-primary-950/40",
            LrpButtonColor.Red => "text-red-500 hover:bg-red-950/30 active:bg-red-950/40",
            LrpButtonColor.Green => "text-green-400 hover:bg-green-950/30 active:bg-green-950/40",
            LrpButtonColor.Yellow => "text-yellow-400 hover:bg-yellow-950/30 active:bg-yellow-950/40",
            LrpButtonColor.Purple => "text-purple-400 hover:bg-purple-950/30 active:bg-purple-950/40",
            _ => throw new InvalidEnumArgumentException(nameof(Color), (int)Color, typeof(LrpButtonColor))
        }).Build();
}
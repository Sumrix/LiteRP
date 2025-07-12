using System.ComponentModel;
using System.Text;
using Flowbite.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiteRP.WebApp.Components.CustomFlowbite;

/// <summary>
/// Partial class for the Button component, providing additional logic and code-behind functionality.
/// </summary>
public partial class LrpButton
{
    /// <summary>
    /// The type of button (e.g., "button", "submit", "reset").
    /// Only used when not rendering as a link.
    /// </summary>
    [Parameter]
    public string Type { get; set; } = "button";

    /// <summary>
    /// The URL that the button links to. If provided, the button will render as an anchor tag.
    /// </summary>
    [Parameter]
    public string? Href { get; set; }

    /// <summary>
    /// The target attribute for the link (e.g., "_blank", "_self").
    /// Only used when Href is provided.
    /// </summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Determines if the button is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Indicates if the button is in a loading state.
    /// </summary>
    [Parameter]
    public bool Loading { get; set; }

    /// <summary>
    /// Indicates if the button should use fully rounded corners.
    /// </summary>
    [Parameter]
    public bool Pill { get; set; }

    /// <summary>
    /// The visual style of the button.
    /// </summary>
    [Parameter]
    public LrpButtonStyle Variant { get; set; } = LrpButtonStyle.Filled;

    /// <summary>
    /// The size of the button.
    /// </summary>
    [Parameter]
    public LrpButtonSize Size { get; set; } = LrpButtonSize.Medium;

    /// <summary>
    /// The color variant of the button.
    /// </summary>
    [Parameter]
    public LrpButtonColor Color { get; set; } = LrpButtonColor.Primary;

    /// <summary>
    /// The icon to display in the button.
    /// </summary>
    [Parameter]
    public Type? Icon { get; set; }

    [Parameter]
    public double IconStrokeWidth { get; set; } = 2;

    /// <summary>
    /// Child content of the button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Callback for button click event.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Additional attributes to be applied to the button element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [CascadingParameter]
    private LrpButtonGroup? ParentGroup { get; set; }

    /// <summary>
    /// Determines if the button contains only an icon with no text content.
    /// </summary>
    private bool IsIconOnly => Icon != null && ChildContent == null;

    /// <summary>
    /// Prepares the parameters dictionary for the DynamicComponent that renders the icon.
    /// </summary>
    private Dictionary<string, object> IconParameters => new()
    {
        { nameof(IconBase.Class), GetIconClasses() },
        { nameof(IconBase.StrokeWidth), IconStrokeWidth }
    };

    private string GetIconSizeClasses() => Size switch
    {
        LrpButtonSize.Small => "w-4 h-4",
        LrpButtonSize.Medium => "w-5 h-5",
        LrpButtonSize.Large => "w-6 h-6",
        _ => "w-5 h-5"
    };

    private string GetIconClasses()
    {
        var sb = new StringBuilder();
        sb.Append(GetIconSizeClasses());

        if (ChildContent != null)
        {
            sb.Append(" mr-2");
        }

        return sb.ToString();
    }

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

    private string GetButtonClasses()
    {
        var sb = new StringBuilder();
        sb.Append("group inline-flex items-center justify-center text-center font-medium focus:outline-none ");
        
        if (ParentGroup != null)
        {
            sb.Append("rounded-none -ml-px first:ml-0 border ");
            sb.Append("first:rounded-l-lg last:rounded-r-lg ");
            sb.Append("focus:z-10 ");
        }
        else
        {
            sb.Append(Pill ? "rounded-full " : "rounded-lg ");
        }

        sb.Append(GetSizeClasses());
        sb.Append(GetStyleClasses());

        if (Disabled)
        {
            sb.Append(Variant == LrpButtonStyle.Overlay ? "opacity-50 " : "cursor-not-allowed opacity-50 ");
        }
        else if (Loading)
        {
            sb.Append("cursor-wait opacity-75 pointer-events-none ");
        }

        return CombineClasses(sb.ToString().Trim()) ?? string.Empty;
    }

    private string GetSizeClasses()
    {
        if (IsIconOnly)
        {
            return Size switch
            {
                LrpButtonSize.Small => "p-2 ",
                LrpButtonSize.Medium => "p-2.5 ",
                LrpButtonSize.Large => "p-3 ",
                _ => throw new InvalidEnumArgumentException(nameof(Size), (int)Size, typeof(LrpButtonSize))
            };
        }
        
        return Size switch
        {
            LrpButtonSize.Small => "text-xs px-3 py-2 ",
            LrpButtonSize.Medium => "text-sm px-5 py-2.5 ",
            LrpButtonSize.Large => "text-base px-6 py-3 ",
            _ => throw new InvalidEnumArgumentException(nameof(Size), (int)Size, typeof(LrpButtonSize))
        };
    }

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
        _ => throw new InvalidEnumArgumentException(nameof(Variant), (int)Variant, typeof(LrpButtonStyle))
    };

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

    private const string SubtleBorderClasses = "border border-gray-300 dark:border-gray-600 ";
    private const string SubtleHoverBgClasses = "hover:bg-gray-100 dark:hover:bg-gray-700 active:ring-2 active:ring-gray-200 dark:active:ring-gray-600 ";

    /// <summary>
    /// Gets the CSS classes for the Subtle style (with a border).
    /// </summary>
    private string GetSubtleColorClasses() => 
        SubtleBorderClasses + SubtleHoverBgClasses + GetSubtleTextColorClasses();

    /// <summary>
    /// Gets the CSS classes for the SubtleGhost style (without a border).
    /// </summary>
    private string GetSubtleGhostColorClasses() => 
        SubtleHoverBgClasses + GetSubtleTextColorClasses();

    /// <summary>
    /// Gets the CSS classes for the SubtleColored style (with a border).
    /// </summary>
    private string GetSubtleColoredColorClasses() => 
        SubtleBorderClasses + SubtleHoverBgClasses + GetSubtleColoredTextColorClasses();

    /// <summary>
    /// Gets the CSS classes for the SubtleGhostColored style (without a border).
    /// </summary>
    private string GetSubtleGhostColoredColorClasses() => 
        SubtleHoverBgClasses + GetSubtleColoredTextColorClasses();

    /// <summary>
    /// Helper method. Gets text color classes for Subtle/SubtleGhost styles
    /// (base grey text, becomes colored on hover).
    /// </summary>
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

    /// <summary>
    /// Helper method. Gets text color classes for SubtleColored/SubtleGhostColored styles
    /// (text is always colored).
    /// </summary>
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

    private string GetOverlayColorClasses()
    {
        var baseClasses = "drop-shadow-[0_1.5px_1.5px_rgba(0,0,0,0.6)] ";
        
        return baseClasses + (Color switch
        {
            LrpButtonColor.Grey => "text-white  hover:bg-black/30 dark:hover:bg-black/50 active:bg-black/40 dark:active:bg-black/60 ",
            LrpButtonColor.Primary => "text-primary-400  hover:bg-primary-950/30  active:bg-primary-950/40 ",
            LrpButtonColor.Red => "text-red-500  hover:bg-red-950/30  active:bg-red-950/40 ",
            LrpButtonColor.Green => "text-green-400 hover:bg-green-950/30 active:bg-green-950/40 ",
            LrpButtonColor.Yellow => "text-yellow-400 hover:bg-yellow-950/30 active:bg-yellow-950/40 ",
            LrpButtonColor.Purple => "text-purple-400 hover:bg-purple-950/30 active:bg-purple-950/40 ",
            _ => throw new InvalidEnumArgumentException(nameof(Color), (int)Color, typeof(LrpButtonColor))
        });
    }

    /// <summary>
    /// Defines the visual style of a button.
    /// </summary>
    public enum LrpButtonStyle
    {
        /// <summary> Filled button style. </summary>
        Filled,
        /// <summary> Outline button style with a transparent background. </summary>
        Outline,
        /// <summary> Ghost button style with a transparent background and no borders. </summary>
        Ghost,
        /// <summary> Text button style without background and borders. </summary>
        Text,
        /// <summary> Subtle style with a grey border and text that becomes colored on hover. </summary>
        Subtle,
        /// <summary> Subtle style with a grey border and colored text. </summary>
        SubtleColored,
        /// <summary> Borderless version of the Subtle style. </summary>
        SubtleGhost,
        /// <summary> Borderless version of the SubtleColored style. </summary>
        SubtleGhostColored,
        /// <summary>
        /// A special style for buttons placed on top of images or other rich visual content.
        /// It features high-contrast text with a shadow and a translucent background on hover.
        /// The appearance is controlled by the Color parameter.
        /// </summary>
        Overlay
    }

    /// <summary>
    /// Defines the color variants for buttons.
    /// </summary>
    public enum LrpButtonColor
    {
        Primary,
        Grey,
        Green,
        Red,
        Yellow,
        Purple
    }

    /// <summary>
    /// Defines the size variants for buttons.
    /// </summary>
    public enum LrpButtonSize
    {
        Small,
        Medium,
        Large
    }
}
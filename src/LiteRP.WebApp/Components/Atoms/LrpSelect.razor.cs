using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Flowbite.Components;
using LiteRP.WebApp.Utilities;
using Microsoft.AspNetCore.Components;

namespace LiteRP.WebApp.Components.Atoms;

public partial class LrpSelect<TValue>
{
    protected string WrapperClasses => new CssBuilder("relative flex")
        .AddClass(CssClass)
        .Build();

    protected const string IconClasses = "w-4 h-4 text-gray-500 dark:text-gray-400";
    protected readonly Dictionary<string, object> IconParameters = new() { { "class", IconClasses } };

    protected const string AddonClasses =
        "inline-flex items-center border border-gray-300 bg-gray-200 px-3 text-sm text-gray-900 " +
        "dark:border-gray-600 dark:bg-gray-600 dark:text-gray-400";

    protected string AddonLeftClasses => new CssBuilder(AddonClasses)
        .AddClass("rounded-l-md border-r-0")
        .Build();

    protected string AddonRightClasses => new CssBuilder(AddonClasses)
        .AddClass("rounded-r-md border-l-0")
        .Build();

    protected string InputClasses => new CssBuilder("block w-full border disabled:cursor-not-allowed disabled:opacity-50")
            // Size
            .AddClass(Size switch
            {
                TextInputSize.Small => "py-2 text-sm",
                TextInputSize.Large => "sm:text-md py-4",
                _ => "p-2.5 text-sm"
            })
            // Padding for icons
            .AddClass("pl-10", GetLeftIconState())
            .AddClass("px-2.5", !GetLeftIconState())
            // Color
            .AddClass(GetEffectiveColor() switch
            {
                TextInputColor.Gray => "border-gray-300 bg-gray-50 text-gray-900 focus:border-primary-500 focus:ring-primary-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-primary-500 dark:focus:ring-primary-500",
                TextInputColor.Success => "border-green-500 bg-green-50 text-green-900 focus:border-green-500 focus:ring-green-500 dark:border-green-400 dark:bg-green-100 dark:focus:border-green-500 dark:focus:ring-green-500",
                TextInputColor.Failure => "border-red-500 bg-red-50 text-red-900 focus:border-red-500 focus:ring-red-500 dark:border-red-400 dark:bg-red-100 dark:focus:border-red-500 dark:focus:ring-red-500",
                TextInputColor.Warning => "border-yellow-500 bg-yellow-50 text-yellow-900 focus:border-yellow-500 focus:ring-yellow-500 dark:border-yellow-400 dark:bg-yellow-100 dark:focus:border-yellow-500 dark:focus:ring-yellow-500",
                TextInputColor.Info => "border-primary-500 bg-primary-50 text-primary-900 focus:border-primary-500 focus:ring-primary-500 dark:border-primary-400 dark:bg-primary-100 dark:focus:border-primary-500 dark:focus:ring-primary-500",
                _ => string.Empty
            })
            // Border radius based on addons
            .AddClass("rounded-lg", !GetLeftAddonState() && !GetRightAddonState())
            .AddClass("rounded-r-lg border-l-0", GetLeftAddonState() && !GetRightAddonState())
            .AddClass("rounded-l-lg border-r-0", !GetLeftAddonState() && GetRightAddonState())
            .AddClass("!rounded-none", GetLeftAddonState() && GetRightAddonState())
            // Other attributes
            .AddClass("shadow-sm dark:shadow-sm-light", Shadow)
            // Splatted attributes
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

    protected string HelperTextClasses => new CssBuilder("mt-2 text-sm")
            .AddClass(GetEffectiveColor() switch
            {
                TextInputColor.Success => "text-green-600 dark:text-green-500",
                TextInputColor.Failure => "text-red-600 dark:text-red-500",
                TextInputColor.Warning => "text-yellow-600 dark:text-yellow-500",
                TextInputColor.Info => "text-primary-600 dark:text-primary-500",
                _ => "text-gray-500 dark:text-gray-400"
            })
            .Build();

    private ElementReference _elementRef;
    private readonly string _helperTextId = $"lrp-select-helper-{Guid.NewGuid()}";
    private string? _internalHelpText;
    
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public TextInputColor Color { get; set; } = TextInputColor.Gray;
    [Parameter] public TextInputSize Size { get; set; } = TextInputSize.Medium;
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Shadow { get; set; }
    [Parameter] public string? HelperText { get; set; }
    [Parameter] public Type? Icon { get; set; }
    [Parameter] public RenderFragment? IconFragment { get; set; }
    [Parameter] public string? AddonLeft { get; set; }
    [Parameter] public string? AddonRight { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        UpdateInternalHelpText();
    }
    
    protected bool GetLeftIconState() => Icon != null || IconFragment != null;
    protected bool GetLeftAddonState() => !string.IsNullOrEmpty(AddonLeft);
    protected bool GetRightAddonState() => !string.IsNullOrEmpty(AddonRight);
    
    protected TextInputColor GetEffectiveColor()
    {
        var hasErrors = EditContext != null && EditContext.GetValidationMessages(FieldIdentifier).Any();
        return hasErrors ? TextInputColor.Failure : Color;
    }

    private void OnValueChanged(ChangeEventArgs e)
    {
        CurrentValueAsString = e.Value?.ToString();
    }

    protected override bool TryParseValueFromString(
        string? value,
        [MaybeNullWhen(false)] out TValue result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var parsedValue))
        {
            result = parsedValue;
            validationErrorMessage = null;
            return true;
        }
        result = default;
        validationErrorMessage = string.Format(CultureInfo.InvariantCulture, 
            "The {0} field must be a {1}", DisplayName ?? FieldIdentifier.FieldName, typeof(TValue).Name);
        return false;
    }

    private void UpdateInternalHelpText()
    {
        if (EditContext is null)
        {
            _internalHelpText = HelperText ?? string.Empty;
            return;
        }

        var messages = EditContext.GetValidationMessages(FieldIdentifier);
        _internalHelpText = messages.Any() ? messages.First() : HelperText ?? string.Empty;
    }
}
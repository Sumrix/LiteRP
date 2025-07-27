using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Flowbite.Components;
using LiteRP.WebApp.Helpers;
using LiteRP.WebApp.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LiteRP.WebApp.Components.Atoms;

public partial class LrpInput<TValue> : IAsyncDisposable
{
    protected string WrapperClasses => new CssBuilder("relative flex")
        .AddClass(CssClass)
        .Build();

    protected const string IconClasses = "w-5 h-5 text-gray-500 dark:text-gray-400";

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
            .AddClass(CssClass)
            // Size
            .AddClass(Size switch {
                TextInputSize.Small => "py-2 text-sm",
                TextInputSize.Large => "sm:text-md py-4",
                _ => "p-2.5 text-sm"
            })
            // Padding for icons
            .AddClass("pl-10", GetLeftIconState())
            .AddClass("pl-2.5", !GetLeftIconState())
            .AddClass("pr-10", GetRightContentState())
            .AddClass("pr-2.5", !GetRightContentState())
            // Color
            .AddClass(GetEffectiveColor() switch {
                TextInputColor.Gray => " border-gray-300 bg-gray-50 text-gray-900 focus:border-primary-500 focus:ring-primary-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-primary-500 dark:focus:ring-primary-500",
                TextInputColor.Success => "border-green-500 bg-green-50 text-green-900 placeholder-green-700 focus:border-green-500 focus:ring-green-500 dark:border-green-400 dark:bg-green-100 dark:focus:border-green-500 dark:focus:ring-green-500",
                TextInputColor.Failure => "border-red-500 bg-red-50 text-red-900 placeholder-red-700 focus:border-red-500 focus:ring-red-500 dark:border-red-400 dark:bg-red-100 dark:focus:border-red-500 dark:focus:ring-red-500",
                TextInputColor.Warning => "border-yellow-500 bg-yellow-50 text-yellow-900 placeholder-yellow-700 focus:border-yellow-500 focus:ring-yellow-500 dark:border-yellow-400 dark:bg-yellow-100 dark:focus:border-yellow-500 dark:focus:ring-yellow-500",
                TextInputColor.Info => "border-primary-500 bg-primary-50 text-primary-900 placeholder-primary-700 focus:border-primary-500 focus:ring-primary-500 dark:border-primary-400 dark:bg-primary-100 dark:focus:border-primary-500 dark:focus:ring-primary-500",
                _ => string.Empty
            })
            // Border radius based on addons
            .AddClass("rounded-lg", !GetLeftAddonState() && !GetRightAddonState())
            .AddClass("rounded-r-lg border-l-0", GetLeftAddonState() && !GetRightAddonState())
            .AddClass("rounded-l-lg border-r-0", !GetLeftAddonState() && GetRightAddonState())
            .AddClass("!rounded-none", GetLeftAddonState() && GetRightAddonState())
            // Other attributes
            .AddClass("shadow-sm dark:shadow-sm-light", Shadow)
            .AddClass("min-h-[4rem]", _isTextarea && !AutoGrow)
            .AddClass("resize-none", _isTextarea)
            // Splatted attributes
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

    protected string HelperTextClasses => new CssBuilder("mt-2 text-sm")
            .AddClass(GetEffectiveColor() switch {
                TextInputColor.Success => "text-green-600 dark:text-green-500",
                TextInputColor.Failure => "text-red-600 dark:text-red-500",
                TextInputColor.Warning => "text-yellow-600 dark:text-yellow-500",
                TextInputColor.Info => "text-primary-600 dark:text-primary-500",
                _ => "text-gray-500 dark:text-gray-400"
            })
            .Build();
    
    private ElementReference _elementRef;
    private Debouncer<TValue> _debouncer = null!;
    private readonly string _helperTextId = $"lrp-input-helper-{Guid.NewGuid()}";
    private string? _internalHelpText;
    private bool _isTextarea = false;
    private bool _shouldInitAutoGrow;
    private string? _lastAdjustedText;

    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    
    [Parameter] public int DebounceInterval { get; set; }
    [Parameter] public EventCallback<TValue> DebouncedValueChanged { get; set; }
    [Parameter] public TextInputColor Color { get; set; } = TextInputColor.Gray;
    [Parameter] public TextInputSize Size { get; set; } = TextInputSize.Medium;
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Shadow { get; set; }
    [Parameter] public string? HelperText { get; set; }
    [Parameter] public Type? Icon { get; set; }
    [Parameter] public RenderFragment? IconFragment { get; set; }
    [Parameter] public Type? RightIcon { get; set; }
    [Parameter] public RenderFragment? RightIconFragment { get; set; }
    [Parameter] public string? AddonLeft { get; set; }
    [Parameter] public string? AddonRight { get; set; }
    [Parameter] public bool Clearable { get; set; }
    [Parameter] public int Lines { get; set; } = 1;
    [Parameter] public bool AutoGrow { get; set; }
    [Parameter] public int MaxLines { get; set; }
    [Parameter] public string InputType { get; set; } = "text";
    
    protected TextInputColor GetEffectiveColor()
    {
        var hasErrors = EditContext != null && EditContext.GetValidationMessages(FieldIdentifier).Any();
        return hasErrors ? TextInputColor.Failure : Color;
    }

    protected bool GetLeftIconState() => Icon != null || IconFragment != null;

    protected bool GetRightIconState() => RightIcon != null || RightIconFragment != null;

    protected bool GetRightContentState() => GetClearButtonState() || GetRightIconState();

    protected bool GetLeftAddonState() => !string.IsNullOrEmpty(AddonLeft);

    protected bool GetRightAddonState() => !string.IsNullOrEmpty(AddonRight);

    protected bool GetClearButtonState() => Clearable && !string.IsNullOrEmpty(CurrentValueAsString);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _debouncer = new Debouncer<TValue>();
        _debouncer.Debounced += OnDebouncedFired; 
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var oldLines = Lines;
        var oldMaxLines = MaxLines;
        var oldAutoGrow = AutoGrow;

        await base.SetParametersAsync(parameters);
        
        _debouncer.DebounceInterval = DebounceInterval;
        _isTextarea = Lines > 1 || AutoGrow;

        UpdateInternalHelpText();
        
        // Flag AutoGrow to be initialized on the next render.
        if (!oldAutoGrow && AutoGrow)
        {
            _shouldInitAutoGrow = true;
        }
        
        if (oldAutoGrow && !AutoGrow)
        {
            // Disable AutoGrow.
            _shouldInitAutoGrow = false;
            await JSRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.destroy", _elementRef);
        }
        else if (oldLines != Lines || oldMaxLines != MaxLines)
        {
            if (AutoGrow && !_shouldInitAutoGrow)
            {
                // Update AutoGrow parameters (if it was already enabled).
                await JSRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.updateParams", _elementRef, MaxLines);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (AutoGrow)
        {
            if (firstRender || _shouldInitAutoGrow)
            {
                _shouldInitAutoGrow = false;
                await JSRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.initAutoGrow", _elementRef, MaxLines);
                _lastAdjustedText = CurrentValueAsString;
            }
            else if (_lastAdjustedText != CurrentValueAsString)
            {
                await JSRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.adjustHeight", _elementRef);
                _lastAdjustedText = CurrentValueAsString;
            }
        }

        await base.OnAfterRenderAsync(firstRender);
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

    private void UpdateDebounceValue(TValue? value)
    {
        if (DebouncedValueChanged.HasDelegate)
        {
            _debouncer.Update(value);
        }
    }

    private void OnDebouncedFired(TValue? value)
    {
        InvokeAsync(async () => await DebouncedValueChanged.InvokeAsync(value)).CatchAndLog();
    }
    
    private void OnInputChanged(ChangeEventArgs e)
    {
        CurrentValueAsString = (string?)e.Value;
        // Don't manually call mudInputAutoGrow.adjustHeight, because it's handled in JS
        _lastAdjustedText = CurrentValueAsString;
        UpdateDebounceValue(Value);
        UpdateInternalHelpText();
    }

    private void UpdateInternalHelpText()
    {
        if (EditContext is null)
        {
            _internalHelpText = HelperText ?? string.Empty;
            return;
        }

        var messages = EditContext.GetValidationMessages(FieldIdentifier);
        if (messages.Any())
        {
            _internalHelpText = messages.First();
        }
        else
        {
            _internalHelpText = HelperText ?? string.Empty;
        }
    }

    public void Clear()
    {
        CurrentValueAsString = string.Empty;
        UpdateDebounceValue(default);
        UpdateInternalHelpText();
    }

    public async ValueTask DisposeAsync()
    {
        _debouncer.Dispose();

        if (AutoGrow)
        {
            await JSRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.destroy", _elementRef);
        }

        GC.SuppressFinalize(this);
    }
}
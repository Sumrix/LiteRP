using LiteRP.WebApp.Components.Atoms.Tooltip;
using Microsoft.AspNetCore.Components;

namespace LiteRP.WebApp.Components.Molecules;

public partial class LrpHelpIcon
{
    /// <summary>
    /// Текст, который будет отображаться внутри всплывающей подсказки.
    /// </summary>
    [Parameter, EditorRequired]
    public required string Content { get; set; }

    // Change the default value assignment for Icon to use Icons.QuestionCircleSolid instead of LiteRP.WebApp.Helpers.Icons.QuestionCircleSolid
    [Parameter] public Type Icon { get; set; } = Helpers.IconTypes.QuestionCircleSolid;

    /// <summary>
    /// Позиция всплывающей подсказки (например, Top, Bottom, Left, Right).
    /// </summary>
    [Parameter]
    public TooltipPlacement Position { get; set; } = TooltipPlacement.Top;
    
    private string GetClasses()
    {
        const string baseClasses = "w-4 h-4 text-gray-400 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white cursor-help";

        return CombineClasses(baseClasses);
    }
}
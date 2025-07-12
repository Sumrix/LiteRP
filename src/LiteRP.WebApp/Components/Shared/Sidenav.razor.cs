using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiteRP.WebApp.Components.Shared;

public partial class Sidenav
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public new string Id => base.Id;

    public async Task ToggleDrawerAsync()
    {
        if (IsVisible)
        {
            await HideAsync();
        }
        else
        {
            await ShowAsync();
        }
    }

    protected override async Task HandleEscapeKeyAsync(KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && IsVisible)
        {
            await HideAsync();
        }
    }
    
    protected string SidenavClasses => CombineClasses(
        "fixed top-0 left-0 z-40 w-64 h-screen bg-white border-r border-gray-200 transition-transform dark:bg-gray-800 dark:border-gray-700",
        !IsVisible ? "-translate-x-full" : "translate-x-0",
        "md:translate-x-0"
    );
}
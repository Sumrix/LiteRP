using Serilog;

namespace LiteRP.WebApp.Helpers;

public static class LrpGlobal
{
    public static Action<Exception> UnhandledExceptionHandler { get; set; } = (exception) =>
        Log.Logger.Error(exception, "Unhandled exception has occured");
}
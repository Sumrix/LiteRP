namespace LiteRP.WebApp.Helpers;

public static class TimeSpanExtensions
{
    public static string ToShortAgo(this TimeSpan ts)
    {
        if (ts < TimeSpan.Zero) ts = ts.Negate();

        if (ts.TotalMinutes < 1) return "now";
        if (ts.TotalHours   < 1) return $"{(int)ts.TotalMinutes}m";
        if (ts.TotalDays    < 1) return $"{(int)ts.TotalHours}h";
        if (ts.TotalDays    < 7) return $"{(int)ts.TotalDays}d";if (ts.TotalDays < 30) return $"{(int)(ts.TotalDays / 7)}w";
        if (ts.TotalDays < 365) return $"{(int)(ts.TotalDays / 30)}mo";
        return $"{(int)(ts.TotalDays / 365)}y";
    }
}
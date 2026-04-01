
namespace DeskBooking.WebClient.Infrastructure;

public static class DateTimeLocalHelper
{
    public static DateTime ToUtc(DateTime localDateTime)
    {
        if (localDateTime.Kind == DateTimeKind.Utc)
        {
            return localDateTime;
        }

        var local = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
        return local.ToUniversalTime();
    }

    public static DateTime ToLocal(DateTime utcDateTime)
    {
        if (utcDateTime.Kind == DateTimeKind.Local)
        {
            return utcDateTime;
        }

        var utc = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        return utc.ToLocalTime();
    }

    public static string ToInputValue(DateTime utcDateTime)
    {
        return ToLocal(utcDateTime).ToString("yyyy-MM-ddTHH:mm");
    }
}

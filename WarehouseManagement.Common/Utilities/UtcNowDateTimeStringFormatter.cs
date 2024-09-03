namespace WarehouseManagement.Common.Utilities;

public static class UtcNowDateTimeStringFormatted
{
    public static string GetUtcNow(DateTime dateTime)
    {
        var formattedDateTime = $"{dateTime.ToString("s")}Z";

        return formattedDateTime;
    }
}

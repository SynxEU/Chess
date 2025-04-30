namespace ChessAPI.DataCollect.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime DateTimeConverter(this DateOnly date, TimeSpan time) =>
            date.ToDateTime(new TimeOnly(time.Hours, time.Minutes, time.Seconds));
    }
}
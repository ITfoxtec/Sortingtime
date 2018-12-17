namespace Sortingtime.PdfMailWebJob.Infrastructure.Extension
{
    public static class NumberExtensions
    {
        public static string ToTimeFormat(this short value)
        {
            var minutes = value % 60;
            var hours = (value - minutes) / 60;

            return $"{hours}:{MinutesFormatted(minutes)}";
        }

        public static string ToTimeFormat(this int value)
        {
            var minutes = value % 60;
            var hours = (value - minutes) / 60;

            return $"{hours}:{MinutesFormatted(minutes)}";
        }

        private static string MinutesFormatted(int minutes)
        {
            return minutes < 10 ? $"0{minutes}" : $"{minutes}";
        }
    }
}

using System;
namespace FrontendApi.Helpers
{
    public static class DateTimeHelper
    {
        public static long ConvertToUnixTime(this DateTime datetime)
        {
            var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds * 1000;
        }
    }
}
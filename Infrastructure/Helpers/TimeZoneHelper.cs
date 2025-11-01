using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class TimeZoneHelper
    {
        //public static readonly TimeZoneInfo EgyptTimeZone =
        //    TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

        // Hardcode Egypt timezone as UTC+3
        public static readonly TimeZoneInfo EgyptTimeZone =
            TimeZoneInfo.CreateCustomTimeZone(
                "Egypt Time",
                TimeSpan.FromHours(2),
                "Egypt Time",
                "Egypt Time");

        /// Gets current date and time in Egypt timezone
        public static DateTime GetEgyptNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EgyptTimeZone);
        }

        /// Gets current date in Egypt timezone
        public static DateOnly GetEgyptToday()
        {
            return DateOnly.FromDateTime(GetEgyptNow());
        }
        /// <summary>
        /// Gets yesterday's date in Egypt timezone
        /// </summary>
        public static DateOnly GetEgyptYesterday()
        {
            return DateOnly.FromDateTime(GetEgyptNow().AddDays(-1));
        }

        /// <summary>
        /// Converts UTC DateTime to Egypt timezone
        /// </summary>
        public static DateTime ConvertToEgyptTime(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, EgyptTimeZone);
        }

        /// <summary>
        /// Gets the date in Egypt timezone for a given UTC DateTime
        /// </summary>
        public static DateOnly GetEgyptDate(DateTime utcDateTime)
        {
            return DateOnly.FromDateTime(ConvertToEgyptTime(utcDateTime));
        }
    }
}

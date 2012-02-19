using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace NetFrameworkExtensions
{
    public static class DateTimeExtensions
    {
        public static string AppointmentDateTimeString(this DateTime dateTime)
        {
            if (dateTime == dateTime.Date) { return dateTime.ToString("ddd dd MMM"); }

            return dateTime.ToString("ddd dd MMM HH:mm");       
        }
        
       public static bool TryParseDateAndTime(string date, string time, out DateTime dateTime)
        {
            dateTime = default(DateTime);
            
            if (String.IsNullOrWhiteSpace(date + time)) { return false; }
            try
            {
                dateTime = ParseDateAndTime(date, time);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public static DateTime ParseDateAndTime(string date, string time)
        {
            var dateTimeString = date + time;
            var dateTime = DateTime.MinValue;

            try
            {
                //-- Here we first try to parse both the date and time
                if (!DateTime.TryParseExact(dateTimeString, "ddd dd MMM yyyyHH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    //-- if it failed we try to parse just the date (allows the user to optionally specify the time
                    dateTime = DateTime.ParseExact(dateTimeString, "ddd dd MMM yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.AllowTrailingWhite);
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Date & time could not be parsed correctly: " + e.Message);
            }

            return dateTime;
        }
        
        public static string GetDaysToGoString(this DateTime dateTime)
        {
            TimeSpan timeUntil = dateTime.Subtract(DateTime.Now);
            if (timeUntil.Days > 1) { return (String.Format("{0} days to go", timeUntil.Days.ToString())); }
            else if (timeUntil.Days == 1) { return ("Tomorrow"); }
            else if (timeUntil.Days == 0) { return ("Today"); }
            else if (timeUntil.Days == -1) { return ("Yesterday"); }
            else { return (String.Format("{0} days ago", (-1 * timeUntil.Days).ToString())); }
        }

        public static string GetDaysToGoStringUtc(this DateTime dateTime)
        {
            TimeSpan timeUntil = dateTime.Subtract(DateTime.UtcNow);
            if (timeUntil.Days > 1) { return (String.Format("{0} days to go", timeUntil.Days.ToString())); }
            else if (timeUntil.Days == 1) { return ("Tomorrow"); }
            else if (timeUntil.Days == 0) { return ("Today"); }
            else if (timeUntil.Days == -1) { return ("Yesterday"); }
            else { return (String.Format("{0} days ago", (-1 * timeUntil.Days).ToString())); }
        }

        public static string GetAgoString(this DateTime dateTime)
        {
            TimeSpan timeAfter = DateTime.Now.Subtract(dateTime);
            return GetAgoString(timeAfter);
        }

        public static string GetAgoStringUtc(this DateTime dateTime)
        {
            TimeSpan timeAfter = DateTime.UtcNow.Subtract(dateTime);
            return GetAgoString(timeAfter);
        }

        public static string GetDaysAgoStringUtc(this DateTime dateTime)
        {
            TimeSpan timeAfter = DateTime.UtcNow.Subtract(dateTime);
            return GetDaysAgoString(timeAfter);
        }

        public static string GetDaysAgoString(this DateTime dateTime)
        {
            TimeSpan timeAfter = DateTime.Now.Subtract(dateTime);
            return GetDaysAgoString(timeAfter);
        }

        public static DateTime AjustUtcToLocal(this DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }


        private static string GetAgoString(this TimeSpan timeAfter)
        {
            if (timeAfter.Days == 1) { return ("1 day ago"); }
            else if (timeAfter.Days > 14) { return (String.Format("{0} weeks ago", (timeAfter.Days / 7).ToString())); }
            else if (timeAfter.Days > 1) { return (String.Format("{0:G} days ago", timeAfter.Days)); }
            else if (timeAfter.Hours == 1) { return ("1 hour ago"); }
            else if (timeAfter.Hours > 1) { return (String.Format("{0:G} hours ago", timeAfter.Hours)); }
            else if (timeAfter.Minutes <= 1) { return ("1 minute ago"); }
            else { return (String.Format("{0:G} minutes ago", timeAfter.Minutes)); }
        }


        private static string GetDaysAgoString(this TimeSpan timeAfter)
        {
            if (timeAfter.Days == 1) { return ("1 day ago"); }
            else if (timeAfter.Days > 14) { return (String.Format("{0} weeks ago", (timeAfter.Days / 7).ToString())); }
            else if (timeAfter.Days > 1) { return (String.Format("{0:G} days ago", timeAfter.Days)); }
            else { return ("today"); }
        }


        /// <summary>
        /// After a while it just leaves the date
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetHappenedStringUtc(this DateTime dateTime)
        {
            TimeSpan timeAfter = DateTime.UtcNow.Subtract(dateTime);
            if (timeAfter.Days == 1) { return ("yesterday"); }
            else if (timeAfter.Days > 1) { return (String.Format("{0:dd MMM}", dateTime)); }
            else if (timeAfter.Hours == 1) { return ("about an hour ago"); }
            else if (timeAfter.Hours > 1) { return (String.Format("{0:G} hours ago", timeAfter.Hours)); }
            else if (timeAfter.Minutes <= 5) { return ("just now"); }
            else { return (String.Format("{0:G} minutes ago", timeAfter.Minutes)); }
        }


        public static string ConvertIntToDate(int day)
        {
            string lastDigit = day.ToString().Substring(day.ToString().Length - 1, 1);

            if (day.ToString() == "11") { return (day.ToString() + "th"); }
            if (day.ToString() == "12") { return (day.ToString() + "th"); }
            if (day.ToString() == "13") { return (day.ToString() + "th"); }

            if (lastDigit == "1") { return (day.ToString() + "st"); }
            else if (lastDigit == "2") { return (day.ToString() + "nd"); }
            else if (lastDigit == "3") { return (day.ToString() + "rd"); }
            else { return (day.ToString() + "th"); }
        }



        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        public static ulong ToEpochTime(this DateTime dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ts = date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return Convert.ToUInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        public static ulong ToEpochTime(this DateTimeOffset dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ts = date - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

            return Convert.ToUInt64(ts.TotalSeconds);
        }

        public static string ToEpochTimeString(this DateTime dateTime)
        {
            var d1 = new DateTime(1970, 1, 1);
            var ts = new TimeSpan(dateTime.Ticks - d1.Ticks);

            return ts.TotalMilliseconds.ToString("0.#");
        }

        /// <summary>
        /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
        /// </summary>
        public static DateTime ToDateTimeFromEpoch(this ulong secondsSince1970)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(secondsSince1970);
        }

        /// <summary>
        /// Converts the given epoch time to a UTC <see cref="DateTimeOffset"/>.
        /// </summary>
        public static DateTimeOffset ToDateTimeOffsetFromEpoch(this ulong secondsSince1970)
        {
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(secondsSince1970);
        }
    }
}

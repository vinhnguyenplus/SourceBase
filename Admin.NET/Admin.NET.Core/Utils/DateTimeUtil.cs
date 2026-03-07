// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// time help class
/// </summary>
public class DateTimeUtil
{
    public readonly DateTime Date;

    private DateTimeUtil(TimeSpan timeSpan = default)
    {
        Date = DateTime.Now.AddTicks(timeSpan.Ticks);
    }

    private DateTimeUtil(DateTime time)
    {
        Date = time;
    }

    /// <summary>
    /// instantiate class
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static DateTimeUtil Init(TimeSpan timeSpan = default)
    {
        return new DateTimeUtil(timeSpan);
    }

    /// <summary>
    /// instantiate class
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static DateTimeUtil Init(DateTime time)
    {
        return new DateTimeUtil(time);
    }

    /// <summary>
    /// Automatically determine whether the unix timestamp is in seconds or milliseconds based on its length.
    /// </summary>
    /// <param name="unixTime"></param>
    /// <returns></returns>
    public static DateTime ConvertUnixTime(long unixTime)
    {
        // Determine timestamp length
        bool isMilliseconds = unixTime > 9999999999;

        if (isMilliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTime).ToLocalTime().DateTime;
        }
        else
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).ToLocalTime().DateTime;
        }
    }

    /// <summary>
    /// Get start time
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    public static DateTime GetBeginTime(DateTime? dateTime, int days = 0)
    {
        return dateTime == DateTime.MinValue || dateTime == null ? DateTime.Now.AddDays(days) : (DateTime)dateTime;
    }

    /// <summary>
    ///  Convert timestamp to local time - timestamp is accurate to seconds
    /// </summary>
    public static DateTime ToLocalTimeDateBySeconds(long unix)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unix).ToLocalTime().DateTime;
    }

    /// <summary>
    ///  Time to timestamp Unix-timestamp is accurate to seconds
    /// </summary>
    public static long ToUnixTimestampBySeconds(DateTime dt)
    {
        return new DateTimeOffset(dt).ToUnixTimeSeconds();
    }

    /// <summary>
    ///  Convert timestamp to local time - timestamp is accurate to milliseconds
    /// </summary>
    public static DateTime ToLocalTimeDateByMilliseconds(long unix)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(unix).ToLocalTime().DateTime;
    }

    /// <summary>
    ///  Time to timestamp Unix - timestamp is accurate to milliseconds
    /// </summary>
    public static long ToUnixTimestampByMilliseconds(DateTime dt)
    {
        return new DateTimeOffset(dt).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// milliseconds to days hours minutes seconds
    /// </summary>
    /// <param name="ms">TotalMilliseconds</param>
    /// <param name="isSimplify">Whether to simplify the display</param>
    /// <returns></returns>
    public static string FormatTime(long ms, bool isSimplify = false)
    {
        int ss = 1000;
        int mi = ss * 60;
        int hh = mi * 60;
        int dd = hh * 24;

        long day = ms / dd;
        long hour = (ms - day * dd) / hh;
        long minute = (ms - day * dd - hour * hh) / mi;
        long second = (ms - day * dd - hour * hh - minute * mi) / ss;
        //long milliSecond = ms - day * dd - hour * hh - minute * mi - second * ss;

        string sDay = day < 10 ? "0" + day : "" + day; // sky
        string sHour = hour < 10 ? "0" + hour : "" + hour;// Hour
        string sMinute = minute < 10 ? "0" + minute : "" + minute;// minute
        string sSecond = second < 10 ? "0" + second : "" + second;// Second
        //string sMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//Milliseconds
        //sMilliSecond = milliSecond < 100 ? "0" + sMilliSecond : "" + sMilliSecond;

        if (!isSimplify)
            return $"{sDay} days {sHour} hours {sMinute} minutes {sSecond} seconds";
        else
        {
            string result = string.Empty;
            if (day > 0)
                result = $"{sDay} days";
            if (hour > 0)
                result = $"{result}{sHour} hours";
            if (minute > 0)
                result = $"{result}{sMinute} minutes";
            if (!result.IsNullOrEmpty())
                result = $"{result}{sSecond} seconds";
            else
                result = $"{sSecond}second";
            return result;
        }
    }

    /// <summary>
    /// Get unix timestamp
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static long GetUnixTimeStamp(DateTime dt)
    {
        return ((DateTimeOffset)dt).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Get the minimum time of the date day
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static DateTime GetDayMinDate(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
    }

    /// <summary>
    /// Get the maximum time in date days
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>

    public static DateTime GetDayMaxDate(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
    }

    /// <summary>
    /// Format a date based on whether it is in the current year
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string FormatDateTime(DateTime? dt)
    {
        return dt == null ? string.Empty : dt.Value.ToString(dt.Value.Year == DateTime.Now.Year ? "MM-dd HH:mm" : "yyyy-MM-dd HH:mm");
    }

    /// <summary>
    /// Get the date range 00:00:00 - 23:59:59
    /// </summary>
    /// <returns></returns>
    public static List<DateTime> GetTodayTimeList(DateTime time)
    {
        return new List<DateTime>
        {
            Convert.ToDateTime(time.ToString("D")),
            Convert.ToDateTime(time.AddDays(1).ToString("D")).AddSeconds(-1)
        };
    }

    /// <summary>
    /// Get day of week
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string GetWeekByDate(DateTime dt)
    {
        var day = new[] { "Sunday", "Monday", "weekTwo", "wednesday", "weekFour", "Friday", "Saturday" };
        return day[Convert.ToInt32(dt.DayOfWeek.ToString("d"))];
    }

    /// <summary>
    /// Get the week number of this month
    /// </summary>
    /// <param name="daytime"></param>
    /// <returns></returns>
    public static int GetWeekNumInMonth(DateTime daytime)
    {
        int dayInMonth = daytime.Day;
        // first day of month
        DateTime firstDay = daytime.AddDays(1 - daytime.Day);
        // What day of the week is the first day of this month?
        int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;
        // How many days are there in the first week of this month?
        int firstWeekEndDay = 7 - (weekday - 1);
        // The difference between the current date and the first week
        int diffday = dayInMonth - firstWeekEndDay;
        diffday = diffday > 0 ? diffday : 1;
        // The current week is the current week. If it is divided by 7, then subtract one day.
        return ((diffday % 7) == 0 ? (diffday / 7 - 1) : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);
    }

    /// <summary>
    /// Get today's time range
    /// </summary>
    /// <returns>Returns a tuple containing the start time and end time</returns>
    public (DateTime Start, DateTime End) GetTodayRange()
    {
        var start = Date.Date; // Start time of the day
        var end = start.AddDays(1).AddSeconds(-1); // end of day
        return (start, end);
    }

    /// <summary>
    /// Get this month's time range
    /// </summary>
    /// <returns>Returns a tuple containing the start time and end time</returns>
    public (DateTime Start, DateTime End) GetMonthRange()
    {
        return (GetFirstDayOfMonth(), GetLastDayOfMonth());
    }

    /// <summary>
    /// Get the start time of the first day of this month
    /// </summary>
    /// <returns>Returns the first day of the month</returns>
    public DateTime GetFirstDayOfMonth()
    {
        return new DateTime(Date.Year, Date.Month, 1);
    }

    /// <summary>
    /// Get the end time of the last day of this month
    /// </summary>
    /// <returns>Returns the last day of the month</returns>
    public DateTime GetLastDayOfMonth()
    {
        var firstDayOfNextMonth = new DateTime(Date.Year, Date.Month, 1).AddMonths(1);
        return firstDayOfNextMonth.AddSeconds(-1);
    }

    /// <summary>
    /// Get this year's time range
    /// </summary>
    public (DateTime Start, DateTime End) GetYearRange()
    {
        return (GetFirstDayOfYear(), GetLastDayOfYear());
    }

    /// <summary>
    /// Get the first day of the year time range
    /// </summary>
    public DateTime GetFirstDayOfYear()
    {
        return new DateTime(Date.Year, 1, 1);
    }

    /// <summary>
    /// Get the last day of the year time range
    /// </summary>
    public DateTime GetLastDayOfYear()
    {
        return new DateTime(Date.Year, 12, 31, 23, 59, 59);
    }

    /// <summary>
    /// Get the time range of the day before yesterday
    /// </summary>
    public (DateTime Start, DateTime End) GetDayBeforeYesterdayRange()
    {
        var start = Date.Date.AddDays(-2); // Start time the day before yesterday
        var end = start.AddDays(1).AddSeconds(-1); // end time the day before yesterday
        return (start, end);
    }

    /// <summary>
    /// Get yesterday's time range
    /// </summary>
    public (DateTime Start, DateTime End) GetYesterdayRange()
    {
        var start = Date.Date.AddDays(-1); // Yesterday's start time
        var end = start.AddDays(1).AddSeconds(-1); // Yesterday's end time
        return (start, end);
    }

    /// <summary>
    /// Get the time range of the previous week
    /// </summary>
    public (DateTime Start, DateTime End) GetLastWeekRange()
    {
        // Calculate the difference in days from last week
        var daysToSubtract = (int)Date.DayOfWeek + 7; // Make sure Sundays are also calculated correctly
        var start = Date.Date.AddDays(-daysToSubtract); // first day of last week
        var end = start.AddDays(7).AddSeconds(-1); // last day of last week
        return (start, end);
    }

    /// <summary>
    /// Get this week's time range
    /// </summary>
    public (DateTime Start, DateTime End) GetThisWeekRange()
    {
        // Calculate the difference in days of the week
        var daysToSubtract = (int)Date.DayOfWeek;
        var start = Date.Date.AddDays(-daysToSubtract); // first day of week
        var end = start.AddDays(7).AddSeconds(-1); // last day of week
        return (start, end);
    }

    /// <summary>
    /// Get the time range of the previous month
    /// </summary>
    public (DateTime Start, DateTime End) GetLastMonthRange()
    {
        var firstDayOfLastMonth = new DateTime(Date.Year, Date.Month, 1).AddMonths(-1); // first day of last month
        var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddSeconds(-1); // last day of last month
        return (firstDayOfLastMonth, lastDayOfLastMonth);
    }

    /// <summary>
    /// Get the time range of the last 3 days
    /// </summary>
    public (DateTime Start, DateTime End) GetLast3DaysRange()
    {
        var start = Date.Date.AddDays(-2); // Start time 3 days ago
        var end = Date.Date.AddDays(1).AddSeconds(-1); // end time of current date
        return (start, end);
    }

    /// <summary>
    /// Get the time range of the last 7 days
    /// </summary>
    public (DateTime Start, DateTime End) GetLast7DaysRange()
    {
        var start = Date.Date.AddDays(-6); // Start time 7 days ago
        var end = Date.Date.AddDays(1).AddSeconds(-1); // end time of current date
        return (start, end);
    }

    /// <summary>
    /// Get the time range of the last 15 days
    /// </summary>
    public (DateTime Start, DateTime End) GetLast15DaysRange()
    {
        var start = Date.Date.AddDays(-14); // Start time 15 days ago
        var end = Date.Date.AddDays(1).AddSeconds(-1); // end time of current date
        return (start, end);
    }

    /// <summary>
    /// Get the time range of the last 3 months
    /// </summary>
    public (DateTime Start, DateTime End) GetLast3MonthsRange()
    {
        var start = Date.Date.AddMonths(-3); // Start time 3 months ago
        var end = Date.Date.AddDays(1).AddSeconds(-1); // end time of current date
        return (start, end);
    }

    /// <summary>
    /// Get the first half of the year time range
    /// </summary>
    public (DateTime Start, DateTime End) GetFirstHalfYearRange()
    {
        var start = new DateTime(Date.Year, 1, 1); // Start time of first half
        var end = new DateTime(Date.Year, 6, 30, 23, 59, 59); // End of the first half of the year
        return (start, end);
    }

    /// <summary>
    /// Get the time range for the second half of the year
    /// </summary>
    public (DateTime Start, DateTime End) GetSecondHalfYearRange()
    {
        var start = new DateTime(Date.Year, 7, 1); // Start time of the second half of the year
        var end = new DateTime(Date.Year, 12, 31, 23, 59, 59); // The end of the second half of the year
        return (start, end);
    }
}
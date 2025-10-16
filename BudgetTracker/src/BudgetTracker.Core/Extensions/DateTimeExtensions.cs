namespace BudgetTracker.Core.Extensions;

/// <summary>
/// Extension methods for DateTime type
/// Phase 6.18: Extension Methods Implementation
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Get the start of the month for this date
    /// </summary>
    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// Get the end of the month for this date
    /// </summary>
    public static DateTime EndOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// Get the start of the week (Monday) for this date
    /// </summary>
    public static DateTime StartOfWeek(this DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    /// <summary>
    /// Get the end of the week (Sunday) for this date
    /// </summary>
    public static DateTime EndOfWeek(this DateTime date)
    {
        return date.StartOfWeek().AddDays(6);
    }

    /// <summary>
    /// Get the start of the year for this date
    /// </summary>
    public static DateTime StartOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 1, 1);
    }

    /// <summary>
    /// Get the end of the year for this date
    /// </summary>
    public static DateTime EndOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 12, 31);
    }

    /// <summary>
    /// Get the start of the quarter for this date
    /// </summary>
    public static DateTime StartOfQuarter(this DateTime date)
    {
        var quarterMonth = ((date.Month - 1) / 3) * 3 + 1;
        return new DateTime(date.Year, quarterMonth, 1);
    }

    /// <summary>
    /// Get the end of the quarter for this date
    /// </summary>
    public static DateTime EndOfQuarter(this DateTime date)
    {
        return date.StartOfQuarter().AddMonths(3).AddDays(-1);
    }

    /// <summary>
    /// Check if date is in current month
    /// </summary>
    public static bool IsCurrentMonth(this DateTime date)
    {
        var now = DateTime.Today;
        return date.Year == now.Year && date.Month == now.Month;
    }

    /// <summary>
    /// Check if date is in current year
    /// </summary>
    public static bool IsCurrentYear(this DateTime date)
    {
        return date.Year == DateTime.Today.Year;
    }

    /// <summary>
    /// Check if date is in current week
    /// </summary>
    public static bool IsCurrentWeek(this DateTime date)
    {
        var now = DateTime.Today;
        return date >= now.StartOfWeek() && date <= now.EndOfWeek();
    }

    /// <summary>
    /// Check if date is today
    /// </summary>
    public static bool IsToday(this DateTime date)
    {
        return date.Date == DateTime.Today;
    }

    /// <summary>
    /// Check if date is in the past
    /// </summary>
    public static bool IsPast(this DateTime date)
    {
        return date.Date < DateTime.Today;
    }

    /// <summary>
    /// Check if date is in the future
    /// </summary>
    public static bool IsFuture(this DateTime date)
    {
        return date.Date > DateTime.Today;
    }

    /// <summary>
    /// Get friendly relative date string
    /// Example: "Today", "Yesterday", "2 days ago", "Jan 15, 2024"
    /// </summary>
    public static string ToFriendlyString(this DateTime date)
    {
        var today = DateTime.Today;
        var days = (today - date.Date).Days;

        return days switch
        {
            0 => "Today",
            1 => "Yesterday",
            -1 => "Tomorrow",
            > 0 and <= 7 => $"{days} days ago",
            < 0 and >= -7 => $"In {Math.Abs(days)} days",
            _ => date.ToString("MMM dd, yyyy")
        };
    }

    /// <summary>
    /// Get month name
    /// </summary>
    public static string ToMonthName(this DateTime date)
    {
        return date.ToString("MMMM");
    }

    /// <summary>
    /// Get short month name
    /// </summary>
    public static string ToShortMonthName(this DateTime date)
    {
        return date.ToString("MMM");
    }

    /// <summary>
    /// Get month and year string
    /// Example: "January 2024"
    /// </summary>
    public static string ToMonthYear(this DateTime date)
    {
        return date.ToString("MMMM yyyy");
    }

    /// <summary>
    /// Get short month and year string
    /// Example: "Jan 2024"
    /// </summary>
    public static string ToShortMonthYear(this DateTime date)
    {
        return date.ToString("MMM yyyy");
    }

    /// <summary>
    /// Get quarter number (1-4)
    /// </summary>
    public static int GetQuarter(this DateTime date)
    {
        return (date.Month - 1) / 3 + 1;
    }

    /// <summary>
    /// Get quarter name
    /// Example: "Q1 2024"
    /// </summary>
    public static string ToQuarterString(this DateTime date)
    {
        return $"Q{date.GetQuarter()} {date.Year}";
    }

    /// <summary>
    /// Get number of days until this date
    /// </summary>
    public static int DaysUntil(this DateTime date)
    {
        return (date.Date - DateTime.Today).Days;
    }

    /// <summary>
    /// Get number of days since this date
    /// </summary>
    public static int DaysSince(this DateTime date)
    {
        return (DateTime.Today - date.Date).Days;
    }

    /// <summary>
    /// Add business days (skipping weekends)
    /// </summary>
    public static DateTime AddBusinessDays(this DateTime date, int days)
    {
        var result = date;
        var increment = days > 0 ? 1 : -1;
        var daysToAdd = Math.Abs(days);

        while (daysToAdd > 0)
        {
            result = result.AddDays(increment);
            if (result.DayOfWeek != DayOfWeek.Saturday && result.DayOfWeek != DayOfWeek.Sunday)
            {
                daysToAdd--;
            }
        }

        return result;
    }

    /// <summary>
    /// Check if date is weekend
    /// </summary>
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Check if date is weekday
    /// </summary>
    public static bool IsWeekday(this DateTime date)
    {
        return !date.IsWeekend();
    }
}

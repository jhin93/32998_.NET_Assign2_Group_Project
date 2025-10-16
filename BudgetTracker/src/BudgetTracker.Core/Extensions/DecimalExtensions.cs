using System.Globalization;

namespace BudgetTracker.Core.Extensions;

/// <summary>
/// Extension methods for decimal type
/// Phase 6.18: Extension Methods Implementation
/// Demonstrates Extension Methods requirement
/// </summary>
public static class DecimalExtensions
{
    /// <summary>
    /// Convert decimal to currency string format
    /// Example: 1234.56m.ToCurrency() => "$1,234.56"
    /// </summary>
    public static string ToCurrency(this decimal amount, string currencySymbol = "$")
    {
        return $"{currencySymbol}{amount:N2}";
    }

    /// <summary>
    /// Convert decimal to currency string with specified currency code
    /// Example: 1234.56m.ToCurrencyWithCode("USD") => "$1,234.56 USD"
    /// </summary>
    public static string ToCurrencyWithCode(this decimal amount, string currencyCode = "USD")
    {
        var symbol = GetCurrencySymbol(currencyCode);
        return $"{symbol}{amount:N2} {currencyCode}";
    }

    /// <summary>
    /// Convert decimal to compact currency format (K, M, B)
    /// Example: 1500m.ToCompactCurrency() => "$1.5K"
    /// </summary>
    public static string ToCompactCurrency(this decimal amount, string currencySymbol = "$")
    {
        if (Math.Abs(amount) >= 1000000000)
            return $"{currencySymbol}{amount / 1000000000:N2}B";
        if (Math.Abs(amount) >= 1000000)
            return $"{currencySymbol}{amount / 1000000:N2}M";
        if (Math.Abs(amount) >= 1000)
            return $"{currencySymbol}{amount / 1000:N2}K";
        return $"{currencySymbol}{amount:N2}";
    }

    /// <summary>
    /// Format decimal as percentage
    /// Example: 0.85m.ToPercentage() => "85.00%"
    /// </summary>
    public static string ToPercentage(this decimal value, int decimalPlaces = 2)
    {
        var format = $"F{decimalPlaces}";
        return $"{(value * 100).ToString(format)}%";
    }

    /// <summary>
    /// Round to currency precision (2 decimal places)
    /// </summary>
    public static decimal ToCurrencyRounded(this decimal amount)
    {
        return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Check if amount is positive
    /// </summary>
    public static bool IsPositive(this decimal amount)
    {
        return amount > 0;
    }

    /// <summary>
    /// Check if amount is negative
    /// </summary>
    public static bool IsNegative(this decimal amount)
    {
        return amount < 0;
    }

    /// <summary>
    /// Check if amount is zero
    /// </summary>
    public static bool IsZero(this decimal amount)
    {
        return amount == 0;
    }

    /// <summary>
    /// Get absolute value as currency string
    /// </summary>
    public static string ToAbsoluteCurrency(this decimal amount, string currencySymbol = "$")
    {
        return $"{currencySymbol}{Math.Abs(amount):N2}";
    }

    /// <summary>
    /// Format with sign indicator (+ or -)
    /// Example: 100m.ToSignedCurrency() => "+$100.00"
    /// </summary>
    public static string ToSignedCurrency(this decimal amount, string currencySymbol = "$")
    {
        var sign = amount >= 0 ? "+" : "-";
        return $"{sign}{currencySymbol}{Math.Abs(amount):N2}";
    }

    /// <summary>
    /// Convert to accounting format (negative in parentheses)
    /// Example: -100m.ToAccountingFormat() => "($100.00)"
    /// </summary>
    public static string ToAccountingFormat(this decimal amount, string currencySymbol = "$")
    {
        if (amount < 0)
            return $"({currencySymbol}{Math.Abs(amount):N2})";
        return $"{currencySymbol}{amount:N2}";
    }

    /// <summary>
    /// Format as currency with color indicator for positive/negative
    /// Returns tuple of (formattedString, colorIndicator)
    /// </summary>
    public static (string text, string color) ToCurrencyWithColor(this decimal amount, string currencySymbol = "$")
    {
        var text = $"{currencySymbol}{Math.Abs(amount):N2}";
        var color = amount >= 0 ? "green" : "red";
        return (text, color);
    }

    /// <summary>
    /// Calculate percentage of total
    /// Example: 250m.PercentageOf(1000m) => 25.00
    /// </summary>
    public static decimal PercentageOf(this decimal amount, decimal total)
    {
        if (total == 0)
            return 0;
        return (amount / total) * 100;
    }

    /// <summary>
    /// Calculate what amount is X% of
    /// Example: 250m.IsWhatPercentageOf() where result * 0.25 = 250 => 1000
    /// </summary>
    public static decimal FromPercentage(this decimal percentage, decimal total)
    {
        return (percentage / 100) * total;
    }

    /// <summary>
    /// Add percentage to amount
    /// Example: 100m.AddPercentage(10) => 110.00
    /// </summary>
    public static decimal AddPercentage(this decimal amount, decimal percentage)
    {
        return amount + (amount * percentage / 100);
    }

    /// <summary>
    /// Subtract percentage from amount
    /// Example: 100m.SubtractPercentage(10) => 90.00
    /// </summary>
    public static decimal SubtractPercentage(this decimal amount, decimal percentage)
    {
        return amount - (amount * percentage / 100);
    }

    /// <summary>
    /// Helper method to get currency symbol from currency code
    /// </summary>
    private static string GetCurrencySymbol(string currencyCode)
    {
        return currencyCode.ToUpper() switch
        {
            "USD" => "$",
            "EUR" => "€",
            "GBP" => "£",
            "JPY" => "¥",
            "AUD" => "A$",
            "CAD" => "C$",
            "CHF" => "Fr",
            "CNY" => "¥",
            "INR" => "₹",
            "KRW" => "₩",
            _ => "$"
        };
    }
}

namespace BudgetTracker.Domain.Enums;

/// <summary>
/// Defines the frequency for recurring transactions
/// </summary>
public enum Frequency
{
    /// <summary>
    /// One-time transaction (no recurrence)
    /// </summary>
    None = 0,

    /// <summary>
    /// Occurs every day
    /// </summary>
    Daily = 1,

    /// <summary>
    /// Occurs every week
    /// </summary>
    Weekly = 2,

    /// <summary>
    /// Occurs every two weeks
    /// </summary>
    BiWeekly = 3,

    /// <summary>
    /// Occurs every month
    /// </summary>
    Monthly = 4,

    /// <summary>
    /// Occurs every three months
    /// </summary>
    Quarterly = 5,

    /// <summary>
    /// Occurs every year
    /// </summary>
    Yearly = 6
}

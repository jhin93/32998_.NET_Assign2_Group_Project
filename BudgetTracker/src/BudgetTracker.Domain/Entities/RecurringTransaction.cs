using BudgetTracker.Domain.Enums;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities;

/// <summary>
/// Represents a recurring transaction template
/// Phase 6.17: Recurring Transaction Feature
/// Demonstrates IRecurring interface usage
/// </summary>
public class RecurringTransaction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Money Amount { get; set; } = null!;
    public int CategoryId { get; set; }
    public bool IsIncome { get; set; }  // true = Income, false = Expense
    public DateTime StartDate { get; set; }
    public DateTime LastGenerated { get; set; }
    public bool IsActive { get; set; }

    // Recurring properties
    public Frequency RecurrenceFrequency { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }

    public bool IsRecurring => RecurrenceFrequency != Frequency.None;

    public RecurringTransaction()
    {
        StartDate = DateTime.Today;
        LastGenerated = DateTime.Today.AddDays(-1); // So it can generate immediately
        IsActive = true;
    }

    public RecurringTransaction(
        string name,
        string description,
        Money amount,
        int categoryId,
        bool isIncome,
        DateTime startDate,
        Frequency frequency,
        DateTime? endDate = null)
    {
        Name = name;
        Description = description;
        Amount = amount;
        CategoryId = categoryId;
        IsIncome = isIncome;
        StartDate = startDate;
        RecurrenceFrequency = frequency;
        RecurrenceEndDate = endDate;
        LastGenerated = startDate.AddDays(-1); // So it can generate immediately
        IsActive = true;
    }

    /// <summary>
    /// Sets up the recurrence pattern
    /// </summary>
    public void SetRecurrence(Frequency frequency, DateTime? endDate = null)
    {
        RecurrenceFrequency = frequency;
        RecurrenceEndDate = endDate;
    }

    /// <summary>
    /// Calculates the next occurrence date based on frequency
    /// </summary>
    public DateTime? GetNextOccurrence()
    {
        if (!IsActive || RecurrenceFrequency == Frequency.None)
            return null;

        var nextDate = CalculateNextDate(LastGenerated);

        // Check if next occurrence is beyond end date
        if (RecurrenceEndDate.HasValue && nextDate > RecurrenceEndDate.Value)
            return null;

        return nextDate;
    }

    /// <summary>
    /// Calculate next date based on frequency
    /// </summary>
    private DateTime CalculateNextDate(DateTime fromDate)
    {
        return RecurrenceFrequency switch
        {
            Frequency.Daily => fromDate.AddDays(1),
            Frequency.Weekly => fromDate.AddDays(7),
            Frequency.BiWeekly => fromDate.AddDays(14),
            Frequency.Monthly => fromDate.AddMonths(1),
            Frequency.Quarterly => fromDate.AddMonths(3),
            Frequency.Yearly => fromDate.AddYears(1),
            _ => fromDate.AddDays(1)
        };
    }

    /// <summary>
    /// Check if transaction should be generated for a given date
    /// </summary>
    public bool ShouldGenerate(DateTime currentDate)
    {
        if (!IsActive)
            return false;

        if (currentDate < StartDate)
            return false;

        if (RecurrenceEndDate.HasValue && currentDate > RecurrenceEndDate.Value)
            return false;

        var nextOccurrence = GetNextOccurrence();
        return nextOccurrence.HasValue && currentDate >= nextOccurrence.Value;
    }

    /// <summary>
    /// Create a transaction instance from this recurring template
    /// </summary>
    public Transaction GenerateTransaction(DateTime date)
    {
        Transaction transaction = IsIncome
            ? new Income(Description, Amount, date, CategoryId)
            : new Expense(Description, Amount, date, CategoryId);

        LastGenerated = date;
        return transaction;
    }

    /// <summary>
    /// Stop this recurring transaction
    /// </summary>
    public void Stop()
    {
        IsActive = false;
    }

    /// <summary>
    /// Resume this recurring transaction
    /// </summary>
    public void Resume()
    {
        IsActive = true;
    }
}

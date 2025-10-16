using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

/// <summary>
/// Abstract base class for all financial transactions
/// Demonstrates POLYMORPHISM - base class for Expense and Income
/// </summary>
public abstract class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; }
    public Money Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public string? Notes { get; set; }
    public string? Account { get; set; }  // Bank account, Cash, Credit Card, etc.
    public Frequency RecurrenceFrequency { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
    public bool IsRecurring => RecurrenceFrequency != Frequency.None;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    protected Transaction()
    {
        Description = string.Empty;
        Amount = Money.Zero();
        Date = DateTime.Today;
        RecurrenceFrequency = Frequency.None;
        CreatedAt = DateTime.UtcNow;
    }

    protected Transaction(string description, Money amount, DateTime date, int categoryId, string? notes = null, string? account = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        if (amount.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        Description = description;
        Amount = amount;
        Date = date;
        CategoryId = categoryId;
        Notes = notes;
        Account = account;
        RecurrenceFrequency = Frequency.None;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Abstract method to get transaction type - must be implemented by derived classes
    /// </summary>
    public abstract string GetTransactionType();

    /// <summary>
    /// Abstract method to calculate impact on balance
    /// Expenses reduce balance (negative), Income increases balance (positive)
    /// </summary>
    public abstract Money GetBalanceImpact();

    /// <summary>
    /// Virtual method that can be overridden by derived classes
    /// </summary>
    public virtual string GetFormattedAmount()
    {
        return Amount.ToFormattedString();
    }

    /// <summary>
    /// Sets up recurring transaction
    /// </summary>
    public void SetRecurrence(Frequency frequency, DateTime? endDate = null)
    {
        if (frequency == Frequency.None)
        {
            RecurrenceFrequency = Frequency.None;
            RecurrenceEndDate = null;
            return;
        }

        if (endDate.HasValue && endDate.Value <= Date)
            throw new ArgumentException("Recurrence end date must be after transaction date", nameof(endDate));

        RecurrenceFrequency = frequency;
        RecurrenceEndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the next occurrence date based on frequency
    /// </summary>
    public DateTime? GetNextOccurrence()
    {
        if (!IsRecurring) return null;

        DateTime nextDate = RecurrenceFrequency switch
        {
            Frequency.Daily => Date.AddDays(1),
            Frequency.Weekly => Date.AddDays(7),
            Frequency.BiWeekly => Date.AddDays(14),
            Frequency.Monthly => Date.AddMonths(1),
            Frequency.Quarterly => Date.AddMonths(3),
            Frequency.Yearly => Date.AddYears(1),
            _ => Date
        };

        // Check if next occurrence is beyond end date
        if (RecurrenceEndDate.HasValue && nextDate > RecurrenceEndDate.Value)
            return null;

        return nextDate;
    }

    /// <summary>
    /// Updates transaction details
    /// </summary>
    public virtual void Update(string description, Money amount, DateTime date, int categoryId, string? notes = null, string? account = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        if (amount.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        Description = description;
        Amount = amount;
        Date = date;
        CategoryId = categoryId;
        Notes = notes;
        Account = account;
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"{GetTransactionType()}: {Description} - {GetFormattedAmount()} on {Date:yyyy-MM-dd}";
    }
}

using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities;

/// <summary>
/// Represents a budget for a specific category and time period
/// </summary>
public class Budget
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Money Amount { get; set; }
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Money CurrentSpent { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Calculated properties
    public Money Remaining => Amount - CurrentSpent;
    public decimal PercentageUsed => Amount.Amount > 0 ? (CurrentSpent.Amount / Amount.Amount) * 100 : 0;
    public bool IsExceeded => CurrentSpent > Amount;
    public bool IsWarning => PercentageUsed >= 80 && !IsExceeded;  // 80% threshold for warning

    public Budget()
    {
        Name = string.Empty;
        Amount = Money.Zero();
        CurrentSpent = Money.Zero();
        StartDate = DateTime.Today;
        EndDate = DateTime.Today.AddMonths(1);
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Budget(string name, Money amount, int categoryId, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Budget name cannot be null or empty", nameof(name));

        if (amount.Amount <= 0)
            throw new ArgumentException("Budget amount must be greater than zero", nameof(amount));

        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        Name = name;
        Amount = amount;
        CategoryId = categoryId;
        StartDate = startDate;
        EndDate = endDate;
        CurrentSpent = Money.Zero(amount.Currency);
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a monthly budget
    /// </summary>
    public static Budget CreateMonthly(string name, decimal amount, int categoryId, DateTime startDate, string currency = "USD")
    {
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return new Budget(name, new Money(amount, currency), categoryId, startDate, endDate);
    }

    /// <summary>
    /// Factory method to create a yearly budget
    /// </summary>
    public static Budget CreateYearly(string name, decimal amount, int categoryId, DateTime startDate, string currency = "USD")
    {
        var endDate = startDate.AddYears(1).AddDays(-1);
        return new Budget(name, new Money(amount, currency), categoryId, startDate, endDate);
    }

    /// <summary>
    /// Updates the budget amount
    /// </summary>
    public void UpdateAmount(Money newAmount)
    {
        if (newAmount.Amount <= 0)
            throw new ArgumentException("Budget amount must be greater than zero", nameof(newAmount));

        if (newAmount.Currency != Amount.Currency)
            throw new InvalidOperationException("Cannot change budget currency");

        Amount = newAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the budget period
    /// </summary>
    public void UpdatePeriod(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds spending to the budget
    /// </summary>
    public void AddSpending(Money amount)
    {
        if (amount.Currency != CurrentSpent.Currency)
            throw new InvalidOperationException("Currency mismatch");

        CurrentSpent += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes spending from the budget (e.g., when transaction is deleted)
    /// </summary>
    public void RemoveSpending(Money amount)
    {
        if (amount.Currency != CurrentSpent.Currency)
            throw new InvalidOperationException("Currency mismatch");

        CurrentSpent -= amount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Resets the current spent amount (useful for new period)
    /// </summary>
    public void Reset()
    {
        CurrentSpent = Money.Zero(Amount.Currency);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the budget is currently active (within date range)
    /// </summary>
    public bool IsCurrentlyActive()
    {
        var today = DateTime.Today;
        return IsActive && today >= StartDate && today <= EndDate;
    }

    /// <summary>
    /// Deactivates the budget
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the budget
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the status message for the budget
    /// </summary>
    public string GetStatusMessage()
    {
        if (IsExceeded)
            return $"Over budget by {(CurrentSpent - Amount).ToFormattedString()}";

        if (IsWarning)
            return $"Warning: {PercentageUsed:F1}% used";

        return $"{Remaining.ToFormattedString()} remaining";
    }

    public override string ToString()
    {
        return $"{Name}: {CurrentSpent.ToFormattedString()} / {Amount.ToFormattedString()} ({PercentageUsed:F1}%)";
    }
}

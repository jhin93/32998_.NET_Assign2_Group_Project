using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities;

/// <summary>
/// Represents an income transaction (money coming in)
/// Demonstrates INHERITANCE - inherits from Transaction base class
/// </summary>
public class Income : Transaction
{
    public string? Source { get; set; }  // Source of income: Salary, Bonus, Investment, etc.

    public Income() : base()
    {
    }

    public Income(string description, Money amount, DateTime date, int categoryId, string? notes = null, string? account = null, string? source = null)
        : base(description, amount, date, categoryId, notes, account)
    {
        Source = source;
    }

    /// <summary>
    /// Override: Returns the transaction type as "Income"
    /// </summary>
    public override string GetTransactionType()
    {
        return "Income";
    }

    /// <summary>
    /// Override: Income has positive impact on balance
    /// </summary>
    public override Money GetBalanceImpact()
    {
        // Return positive value to indicate money coming in
        return Amount;
    }

    /// <summary>
    /// Override: Format amount with positive sign for income
    /// </summary>
    public override string GetFormattedAmount()
    {
        return $"+{Amount.ToFormattedString()}";
    }

    /// <summary>
    /// Updates income with source information
    /// </summary>
    public void UpdateWithSource(string description, Money amount, DateTime date, int categoryId, string? notes = null, string? account = null, string? source = null)
    {
        base.Update(description, amount, date, categoryId, notes, account);
        Source = source;
    }

    /// <summary>
    /// Factory method to create a new income
    /// </summary>
    public static Income Create(string description, decimal amount, DateTime date, int categoryId, string? notes = null, string? account = null, string? source = null, string currency = "USD")
    {
        return new Income(description, new Money(amount, currency), date, categoryId, notes, account, source);
    }
}

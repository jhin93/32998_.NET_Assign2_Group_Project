using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities;

/// <summary>
/// Represents an expense transaction (money going out)
/// Demonstrates INHERITANCE - inherits from Transaction base class
/// </summary>
public class Expense : Transaction
{
    public Expense() : base()
    {
    }

    public Expense(string description, Money amount, DateTime date, int categoryId, string? notes = null, string? account = null)
        : base(description, amount, date, categoryId, notes, account)
    {
    }

    /// <summary>
    /// Override: Returns the transaction type as "Expense"
    /// </summary>
    public override string GetTransactionType()
    {
        return "Expense";
    }

    /// <summary>
    /// Override: Expenses have negative impact on balance
    /// </summary>
    public override Money GetBalanceImpact()
    {
        // Return negative value to indicate money going out
        return new Money(-Amount.Amount, Amount.Currency);
    }

    /// <summary>
    /// Override: Format amount with negative sign for expenses
    /// </summary>
    public override string GetFormattedAmount()
    {
        return $"-{Amount.ToFormattedString()}";
    }

    /// <summary>
    /// Factory method to create a new expense
    /// </summary>
    public static Expense Create(string description, decimal amount, DateTime date, int categoryId, string? notes = null, string? account = null, string currency = "USD")
    {
        return new Expense(description, new Money(amount, currency), date, categoryId, notes, account);
    }
}

using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Core.Events;

/// <summary>
/// Event arguments for transaction-related events
/// Phase 4.13: Delegates & Events Implementation
/// </summary>
public class TransactionEventArgs : EventArgs
{
    public Transaction Transaction { get; set; }
    public TransactionEventType EventType { get; set; }

    public TransactionEventArgs(Transaction transaction, TransactionEventType eventType)
    {
        Transaction = transaction;
        EventType = eventType;
    }
}

/// <summary>
/// Event arguments for budget-related events
/// </summary>
public class BudgetEventArgs : EventArgs
{
    public Budget Budget { get; set; }
    public BudgetEventType EventType { get; set; }

    public BudgetEventArgs(Budget budget, BudgetEventType eventType)
    {
        Budget = budget;
        EventType = eventType;
    }
}

/// <summary>
/// Event arguments for category-related events
/// </summary>
public class CategoryEventArgs : EventArgs
{
    public Category Category { get; set; }
    public CategoryEventType EventType { get; set; }

    public CategoryEventArgs(Category category, CategoryEventType eventType)
    {
        Category = category;
        EventType = eventType;
    }
}

/// <summary>
/// Transaction event types
/// </summary>
public enum TransactionEventType
{
    Added,
    Updated,
    Deleted
}

/// <summary>
/// Budget event types
/// </summary>
public enum BudgetEventType
{
    Added,
    Updated,
    Deleted,
    Exceeded,
    Warning
}

/// <summary>
/// Category event types
/// </summary>
public enum CategoryEventType
{
    Added,
    Updated,
    Deleted
}

/// <summary>
/// Delegates for event handling
/// Demonstrates Delegates requirement
/// </summary>
public delegate void TransactionChangedEventHandler(object sender, TransactionEventArgs e);
public delegate void BudgetChangedEventHandler(object sender, BudgetEventArgs e);
public delegate void CategoryChangedEventHandler(object sender, CategoryEventArgs e);

/// <summary>
/// Event manager for inter-form communication
/// Implements Observer pattern using C# events
/// </summary>
public static class EventManager
{
    // Transaction events
    public static event TransactionChangedEventHandler? TransactionAdded;
    public static event TransactionChangedEventHandler? TransactionUpdated;
    public static event TransactionChangedEventHandler? TransactionDeleted;

    // Budget events
    public static event BudgetChangedEventHandler? BudgetAdded;
    public static event BudgetChangedEventHandler? BudgetUpdated;
    public static event BudgetChangedEventHandler? BudgetDeleted;

    // Category events
    public static event CategoryChangedEventHandler? CategoryAdded;
    public static event CategoryChangedEventHandler? CategoryUpdated;
    public static event CategoryChangedEventHandler? CategoryDeleted;

    // Raise transaction events
    public static void OnTransactionAdded(Transaction transaction)
    {
        TransactionAdded?.Invoke(null, new TransactionEventArgs(transaction, TransactionEventType.Added));
    }

    public static void OnTransactionUpdated(Transaction transaction)
    {
        TransactionUpdated?.Invoke(null, new TransactionEventArgs(transaction, TransactionEventType.Updated));
    }

    public static void OnTransactionDeleted(Transaction transaction)
    {
        TransactionDeleted?.Invoke(null, new TransactionEventArgs(transaction, TransactionEventType.Deleted));
    }

    // Raise budget events
    public static void OnBudgetAdded(Budget budget)
    {
        BudgetAdded?.Invoke(null, new BudgetEventArgs(budget, BudgetEventType.Added));
    }

    public static void OnBudgetUpdated(Budget budget)
    {
        BudgetUpdated?.Invoke(null, new BudgetEventArgs(budget, BudgetEventType.Updated));
    }

    public static void OnBudgetDeleted(Budget budget)
    {
        BudgetDeleted?.Invoke(null, new BudgetEventArgs(budget, BudgetEventType.Deleted));
    }

    // Raise category events
    public static void OnCategoryAdded(Category category)
    {
        CategoryAdded?.Invoke(null, new CategoryEventArgs(category, CategoryEventType.Added));
    }

    public static void OnCategoryUpdated(Category category)
    {
        CategoryUpdated?.Invoke(null, new CategoryEventArgs(category, CategoryEventType.Updated));
    }

    public static void OnCategoryDeleted(Category category)
    {
        CategoryDeleted?.Invoke(null, new CategoryEventArgs(category, CategoryEventType.Deleted));
    }

    /// <summary>
    /// Clear all event subscriptions (useful for testing)
    /// </summary>
    public static void ClearAllEvents()
    {
        TransactionAdded = null;
        TransactionUpdated = null;
        TransactionDeleted = null;
        BudgetAdded = null;
        BudgetUpdated = null;
        BudgetDeleted = null;
        CategoryAdded = null;
        CategoryUpdated = null;
        CategoryDeleted = null;
    }
}

using BudgetTracker.Core.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Core.Services;

/// <summary>
/// Service for managing recurring transactions
/// Phase 6.17: Recurring Transaction Feature
/// Implements IRecurring interface usage
/// </summary>
public class RecurringTransactionService
{
    private readonly IRepository<RecurringTransaction> _recurringRepository;
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Category> _categoryRepository;

    public RecurringTransactionService(
        IRepository<RecurringTransaction> recurringRepository,
        IRepository<Transaction> transactionRepository,
        IRepository<Category> categoryRepository)
    {
        _recurringRepository = recurringRepository;
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Create a new recurring transaction
    /// </summary>
    public RecurringTransaction CreateRecurring(
        string name,
        string description,
        decimal amount,
        string currency,
        int categoryId,
        bool isIncome,
        DateTime startDate,
        Frequency frequency,
        DateTime? endDate = null)
    {
        var money = new Money(amount, currency);
        var recurring = new RecurringTransaction(
            name,
            description,
            money,
            categoryId,
            isIncome,
            startDate,
            frequency,
            endDate
        );

        _recurringRepository.Add(recurring);
        _recurringRepository.SaveChanges();

        return recurring;
    }

    /// <summary>
    /// Process all recurring transactions and generate new transactions if needed
    /// This method should be called periodically (e.g., daily)
    /// </summary>
    public List<Transaction> ProcessRecurringTransactions(DateTime currentDate)
    {
        var generatedTransactions = new List<Transaction>();
        var recurringTransactions = _recurringRepository.GetAll()
            .Where(r => r.IsActive)
            .ToList();

        foreach (var recurring in recurringTransactions)
        {
            if (recurring.ShouldGenerate(currentDate))
            {
                var transaction = recurring.GenerateTransaction(currentDate);
                _transactionRepository.Add(transaction);
                generatedTransactions.Add(transaction);

                // Update the recurring transaction's LastGenerated date
                _recurringRepository.Update(recurring);
            }
        }

        if (generatedTransactions.Any())
        {
            _transactionRepository.SaveChanges();
            _recurringRepository.SaveChanges();
        }

        return generatedTransactions;
    }

    /// <summary>
    /// Get all active recurring transactions
    /// </summary>
    public List<RecurringTransaction> GetActiveRecurringTransactions()
    {
        return _recurringRepository.GetAll()
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToList();
    }

    /// <summary>
    /// Get all recurring transactions (active and inactive)
    /// </summary>
    public List<RecurringTransaction> GetAllRecurringTransactions()
    {
        return _recurringRepository.GetAll()
            .OrderBy(r => r.Name)
            .ToList();
    }

    /// <summary>
    /// Get recurring transactions by frequency using LINQ
    /// </summary>
    public List<RecurringTransaction> GetRecurringTransactionsByFrequency(Frequency frequency)
    {
        return _recurringRepository.GetAll()
            .Where(r => r.RecurrenceFrequency == frequency)
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToList();
    }

    /// <summary>
    /// Get recurring transactions by type using LINQ
    /// </summary>
    public List<RecurringTransaction> GetRecurringTransactionsByType(bool isIncome)
    {
        return _recurringRepository.GetAll()
            .Where(r => r.IsIncome == isIncome)
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToList();
    }

    /// <summary>
    /// Get recurring transactions by category using LINQ
    /// </summary>
    public List<RecurringTransaction> GetRecurringTransactionsByCategory(int categoryId)
    {
        return _recurringRepository.GetAll()
            .Where(r => r.CategoryId == categoryId)
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToList();
    }

    /// <summary>
    /// Update a recurring transaction
    /// </summary>
    public void UpdateRecurringTransaction(RecurringTransaction recurring)
    {
        _recurringRepository.Update(recurring);
        _recurringRepository.SaveChanges();
    }

    /// <summary>
    /// Stop a recurring transaction
    /// </summary>
    public void StopRecurringTransaction(int recurringId)
    {
        var recurring = _recurringRepository.GetById(recurringId);
        if (recurring != null)
        {
            recurring.Stop();
            _recurringRepository.Update(recurring);
            _recurringRepository.SaveChanges();
        }
    }

    /// <summary>
    /// Resume a recurring transaction
    /// </summary>
    public void ResumeRecurringTransaction(int recurringId)
    {
        var recurring = _recurringRepository.GetById(recurringId);
        if (recurring != null)
        {
            recurring.Resume();
            _recurringRepository.Update(recurring);
            _recurringRepository.SaveChanges();
        }
    }

    /// <summary>
    /// Delete a recurring transaction
    /// </summary>
    public void DeleteRecurringTransaction(int recurringId)
    {
        var recurring = _recurringRepository.GetById(recurringId);
        if (recurring != null)
        {
            _recurringRepository.Remove(recurring);
            _recurringRepository.SaveChanges();
        }
    }

    /// <summary>
    /// Get upcoming occurrences for a recurring transaction
    /// </summary>
    public List<DateTime> GetUpcomingOccurrences(int recurringId, int count = 10)
    {
        var recurring = _recurringRepository.GetById(recurringId);
        if (recurring == null || !recurring.IsActive)
            return new List<DateTime>();

        var occurrences = new List<DateTime>();
        var currentDate = recurring.LastGenerated;

        for (int i = 0; i < count; i++)
        {
            var nextDate = recurring.GetNextOccurrence();
            if (!nextDate.HasValue)
                break;

            occurrences.Add(nextDate.Value);

            // Temporarily update LastGenerated to calculate next occurrence
            var originalLastGenerated = recurring.LastGenerated;
            recurring.LastGenerated = nextDate.Value;

            // Check if we should continue
            if (recurring.RecurrenceEndDate.HasValue &&
                nextDate.Value >= recurring.RecurrenceEndDate.Value)
            {
                recurring.LastGenerated = originalLastGenerated;
                break;
            }
        }

        return occurrences;
    }

    /// <summary>
    /// Get summary of recurring transactions
    /// </summary>
    public RecurringSummary GetRecurringSummary()
    {
        var allRecurring = _recurringRepository.GetAll().ToList();
        var active = allRecurring.Where(r => r.IsActive).ToList();

        var monthlyIncome = active
            .Where(r => r.IsIncome)
            .Sum(r => CalculateMonthlyAmount(r));

        var monthlyExpense = active
            .Where(r => !r.IsIncome)
            .Sum(r => CalculateMonthlyAmount(r));

        return new RecurringSummary
        {
            TotalRecurring = allRecurring.Count,
            ActiveRecurring = active.Count,
            InactiveRecurring = allRecurring.Count - active.Count,
            MonthlyIncome = monthlyIncome,
            MonthlyExpense = monthlyExpense,
            NetMonthlyAmount = monthlyIncome - monthlyExpense,
            IncomeCount = active.Count(r => r.IsIncome),
            ExpenseCount = active.Count(r => !r.IsIncome)
        };
    }

    /// <summary>
    /// Calculate approximate monthly amount for a recurring transaction
    /// </summary>
    private decimal CalculateMonthlyAmount(RecurringTransaction recurring)
    {
        var multiplier = recurring.RecurrenceFrequency switch
        {
            Frequency.Daily => 30m,
            Frequency.Weekly => 4.33m,
            Frequency.BiWeekly => 2.17m,
            Frequency.Monthly => 1m,
            Frequency.Quarterly => 0.33m,
            Frequency.Yearly => 0.08m,
            _ => 1m
        };

        return recurring.Amount.Amount * multiplier;
    }

    /// <summary>
    /// Generate transactions for a date range (useful for backfilling)
    /// </summary>
    public List<Transaction> GenerateTransactionsForPeriod(
        int recurringId,
        DateTime startDate,
        DateTime endDate)
    {
        var recurring = _recurringRepository.GetById(recurringId);
        if (recurring == null || !recurring.IsActive)
            return new List<Transaction>();

        var transactions = new List<Transaction>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            if (recurring.ShouldGenerate(currentDate))
            {
                var transaction = recurring.GenerateTransaction(currentDate);
                _transactionRepository.Add(transaction);
                transactions.Add(transaction);
            }

            currentDate = currentDate.AddDays(1);
        }

        if (transactions.Any())
        {
            _transactionRepository.SaveChanges();
            _recurringRepository.Update(recurring);
            _recurringRepository.SaveChanges();
        }

        return transactions;
    }
}

/// <summary>
/// Recurring transactions summary data transfer object
/// </summary>
public class RecurringSummary
{
    public int TotalRecurring { get; set; }
    public int ActiveRecurring { get; set; }
    public int InactiveRecurring { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpense { get; set; }
    public decimal NetMonthlyAmount { get; set; }
    public int IncomeCount { get; set; }
    public int ExpenseCount { get; set; }
}

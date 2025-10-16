using BudgetTracker.Core.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Core.Services;

/// <summary>
/// Service for managing budgets and calculating spending
/// Phase 3.10: BudgetService Implementation
/// Implements LINQ Aggregation requirement
/// </summary>
public class BudgetService
{
    private readonly IRepository<Budget> _budgetRepository;
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Category> _categoryRepository;

    public BudgetService(
        IRepository<Budget> budgetRepository,
        IRepository<Transaction> transactionRepository,
        IRepository<Category> categoryRepository)
    {
        _budgetRepository = budgetRepository;
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Calculate actual spending vs budget using LINQ Aggregation
    /// </summary>
    public BudgetSummary CalculateSpendingVsBudget(int budgetId)
    {
        var budget = _budgetRepository.GetById(budgetId);
        if (budget == null)
        {
            throw new ArgumentException($"Budget with ID {budgetId} not found.");
        }

        // Using LINQ to calculate spending
        var actualSpending = _transactionRepository.GetAll()
            .Where(t => t is Expense)
            .Where(t => t.CategoryId == budget.CategoryId)
            .Where(t => t.Date >= budget.StartDate && t.Date <= budget.EndDate)
            .Sum(t => t.Amount.Amount);

        var budgetAmount = budget.Amount.Amount;
        var remaining = budgetAmount - actualSpending;
        var percentageUsed = budgetAmount > 0 ? (actualSpending / budgetAmount) * 100 : 0;
        var category = _categoryRepository.GetById(budget.CategoryId);

        return new BudgetSummary
        {
            BudgetId = budget.Id,
            BudgetName = budget.Name,
            CategoryName = category?.Name ?? "Unknown",
            BudgetAmount = budgetAmount,
            ActualSpending = actualSpending,
            Remaining = remaining,
            PercentageUsed = percentageUsed,
            IsExceeded = actualSpending > budgetAmount,
            IsWarning = percentageUsed >= 80 && actualSpending <= budgetAmount,
            TransactionCount = _transactionRepository.GetAll()
                .Count(t => t is Expense &&
                       t.CategoryId == budget.CategoryId &&
                       t.Date >= budget.StartDate &&
                       t.Date <= budget.EndDate)
        };
    }

    /// <summary>
    /// Get all active budgets with spending calculations
    /// </summary>
    public List<BudgetSummary> GetAllActiveBudgetsWithSpending()
    {
        var activeBudgets = _budgetRepository.GetAll()
            .Where(b => b.IsCurrentlyActive())
            .ToList();

        var summaries = new List<BudgetSummary>();

        foreach (var budget in activeBudgets)
        {
            var summary = CalculateSpendingVsBudget(budget.Id);
            summaries.Add(summary);
        }

        return summaries;
    }

    /// <summary>
    /// Get budgets that have exceeded their limit using LINQ
    /// </summary>
    public List<BudgetAlert> GetExceededBudgets()
    {
        var activeBudgets = _budgetRepository.GetAll()
            .Where(b => b.IsCurrentlyActive())
            .ToList();

        var exceededBudgets = new List<BudgetAlert>();

        foreach (var budget in activeBudgets)
        {
            var summary = CalculateSpendingVsBudget(budget.Id);

            if (summary.IsExceeded)
            {
                var category = _categoryRepository.GetById(budget.CategoryId);

                exceededBudgets.Add(new BudgetAlert
                {
                    BudgetId = budget.Id,
                    BudgetName = budget.Name,
                    CategoryName = category?.Name ?? "Unknown",
                    BudgetAmount = summary.BudgetAmount,
                    ActualSpending = summary.ActualSpending,
                    OverAmount = summary.ActualSpending - summary.BudgetAmount,
                    AlertLevel = AlertLevel.Critical,
                    Message = $"Budget '{budget.Name}' exceeded by ${summary.ActualSpending - summary.BudgetAmount:N2}"
                });
            }
        }

        return exceededBudgets.OrderByDescending(b => b.OverAmount).ToList();
    }

    /// <summary>
    /// Get budgets that are approaching their limit (warning threshold)
    /// </summary>
    public List<BudgetAlert> GetWarningBudgets()
    {
        var activeBudgets = _budgetRepository.GetAll()
            .Where(b => b.IsCurrentlyActive())
            .ToList();

        var warningBudgets = new List<BudgetAlert>();

        foreach (var budget in activeBudgets)
        {
            var summary = CalculateSpendingVsBudget(budget.Id);

            if (summary.IsWarning)
            {
                var category = _categoryRepository.GetById(budget.CategoryId);

                warningBudgets.Add(new BudgetAlert
                {
                    BudgetId = budget.Id,
                    BudgetName = budget.Name,
                    CategoryName = category?.Name ?? "Unknown",
                    BudgetAmount = summary.BudgetAmount,
                    ActualSpending = summary.ActualSpending,
                    OverAmount = 0,
                    AlertLevel = AlertLevel.Warning,
                    Message = $"Budget '{budget.Name}' is at {summary.PercentageUsed:F1}% ({summary.Remaining:C} remaining)"
                });
            }
        }

        return warningBudgets.OrderByDescending(b => b.ActualSpending).ToList();
    }

    /// <summary>
    /// Get all budget alerts (exceeded + warning) using LINQ
    /// </summary>
    public List<BudgetAlert> GetAllBudgetAlerts()
    {
        var exceededAlerts = GetExceededBudgets();
        var warningAlerts = GetWarningBudgets();

        return exceededAlerts.Concat(warningAlerts)
            .OrderByDescending(a => a.AlertLevel)
            .ThenByDescending(a => a.ActualSpending)
            .ToList();
    }

    /// <summary>
    /// Calculate spending by category for a given period using LINQ GroupBy
    /// </summary>
    public List<CategorySpending> GetSpendingByCategory(DateTime startDate, DateTime endDate)
    {
        var transactions = _transactionRepository.GetAll()
            .Where(t => t is Expense)
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToList();

        // LINQ GroupBy aggregation
        var categorySpending = transactions
            .GroupBy(t => t.CategoryId)
            .Select(g => new CategorySpending
            {
                CategoryId = g.Key,
                CategoryName = _categoryRepository.GetById(g.Key)?.Name ?? "Unknown",
                TotalSpending = g.Sum(t => t.Amount.Amount),
                TransactionCount = g.Count(),
                AverageTransaction = g.Average(t => t.Amount.Amount)
            })
            .OrderByDescending(cs => cs.TotalSpending)
            .ToList();

        return categorySpending;
    }

    /// <summary>
    /// Get budget utilization report for all active budgets
    /// </summary>
    public BudgetUtilizationReport GetBudgetUtilizationReport()
    {
        var activeBudgets = GetAllActiveBudgetsWithSpending();

        var totalBudgeted = activeBudgets.Sum(b => b.BudgetAmount);
        var totalSpent = activeBudgets.Sum(b => b.ActualSpending);
        var totalRemaining = totalBudgeted - totalSpent;

        return new BudgetUtilizationReport
        {
            TotalBudgeted = totalBudgeted,
            TotalSpent = totalSpent,
            TotalRemaining = totalRemaining,
            OverallPercentageUsed = totalBudgeted > 0 ? (totalSpent / totalBudgeted) * 100 : 0,
            BudgetCount = activeBudgets.Count,
            ExceededCount = activeBudgets.Count(b => b.IsExceeded),
            WarningCount = activeBudgets.Count(b => b.IsWarning),
            HealthyCount = activeBudgets.Count(b => !b.IsExceeded && !b.IsWarning),
            BudgetSummaries = activeBudgets
        };
    }

    /// <summary>
    /// Update budget spending based on transactions
    /// </summary>
    public void UpdateBudgetSpending(Budget budget)
    {
        // Reset current spent
        budget.Reset();

        // Calculate spending using LINQ
        var spending = _transactionRepository.GetAll()
            .Where(t => t is Expense)
            .Where(t => t.CategoryId == budget.CategoryId)
            .Where(t => t.Date >= budget.StartDate && t.Date <= budget.EndDate)
            .Sum(t => t.Amount.Amount);

        if (spending > 0)
        {
            budget.AddSpending(new Money(spending, budget.Amount.Currency));
        }
    }

    /// <summary>
    /// Update all active budgets with current spending
    /// </summary>
    public void UpdateAllActiveBudgets()
    {
        var activeBudgets = _budgetRepository.GetAll()
            .Where(b => b.IsActive)
            .ToList();

        foreach (var budget in activeBudgets)
        {
            UpdateBudgetSpending(budget);
            _budgetRepository.Update(budget);
        }

        _budgetRepository.SaveChanges();
    }

    /// <summary>
    /// Check if adding a transaction would exceed budget
    /// </summary>
    public bool WouldExceedBudget(int categoryId, decimal amount, DateTime transactionDate)
    {
        var activeBudget = _budgetRepository.GetAll()
            .FirstOrDefault(b => b.CategoryId == categoryId &&
                                b.IsCurrentlyActive() &&
                                transactionDate >= b.StartDate &&
                                transactionDate <= b.EndDate);

        if (activeBudget == null)
            return false;

        var summary = CalculateSpendingVsBudget(activeBudget.Id);
        var projectedSpending = summary.ActualSpending + amount;

        return projectedSpending > summary.BudgetAmount;
    }
}

/// <summary>
/// Budget summary data transfer object
/// </summary>
public class BudgetSummary
{
    public int BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal ActualSpending { get; set; }
    public decimal Remaining { get; set; }
    public decimal PercentageUsed { get; set; }
    public bool IsExceeded { get; set; }
    public bool IsWarning { get; set; }
    public int TransactionCount { get; set; }
}

/// <summary>
/// Budget alert data transfer object
/// </summary>
public class BudgetAlert
{
    public int BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal ActualSpending { get; set; }
    public decimal OverAmount { get; set; }
    public AlertLevel AlertLevel { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Category spending data transfer object
/// </summary>
public class CategorySpending
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalSpending { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTransaction { get; set; }
}

/// <summary>
/// Budget utilization report
/// </summary>
public class BudgetUtilizationReport
{
    public decimal TotalBudgeted { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalRemaining { get; set; }
    public decimal OverallPercentageUsed { get; set; }
    public int BudgetCount { get; set; }
    public int ExceededCount { get; set; }
    public int WarningCount { get; set; }
    public int HealthyCount { get; set; }
    public List<BudgetSummary> BudgetSummaries { get; set; } = new List<BudgetSummary>();
}

/// <summary>
/// Alert level enumeration
/// </summary>
public enum AlertLevel
{
    Info = 0,
    Warning = 1,
    Critical = 2
}

using BudgetTracker.Core.Interfaces;
using BudgetTracker.Domain.Entities;
using System.Text;

namespace BudgetTracker.Core.Services;

/// <summary>
/// Service for generating reports and analytics
/// Phase 5.15: ReportService Implementation
/// Implements LINQ + Lambda Expression requirement
/// </summary>
public class ReportService
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Budget> _budgetRepository;

    public ReportService(
        IRepository<Transaction> transactionRepository,
        IRepository<Category> categoryRepository,
        IRepository<Budget> budgetRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _budgetRepository = budgetRepository;
    }

    /// <summary>
    /// Generate category spending report using LINQ GroupBy
    /// Demonstrates LINQ + Lambda Expression requirement
    /// </summary>
    public List<CategoryReport> GenerateCategorySpendingReport(DateTime startDate, DateTime endDate)
    {
        // Get all expenses in date range
        var expenses = _transactionRepository.GetAll()
            .Where(t => t is Expense)
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToList();

        if (!expenses.Any())
            return new List<CategoryReport>();

        var totalSpending = expenses.Sum(t => t.Amount.Amount);

        // LINQ GroupBy with Lambda Expression
        var categoryReport = expenses
            .GroupBy(t => t.CategoryId)
            .Select(g => new CategoryReport
            {
                CategoryId = g.Key,
                CategoryName = _categoryRepository.GetById(g.Key)?.Name ?? "Unknown",
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(t => t.Amount.Amount),
                AverageAmount = g.Average(t => t.Amount.Amount),
                MinAmount = g.Min(t => t.Amount.Amount),
                MaxAmount = g.Max(t => t.Amount.Amount),
                Percentage = (g.Sum(t => t.Amount.Amount) / totalSpending) * 100,
                StartDate = startDate,
                EndDate = endDate
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();

        return categoryReport;
    }

    /// <summary>
    /// Generate income report using LINQ GroupBy
    /// </summary>
    public List<CategoryReport> GenerateIncomeReport(DateTime startDate, DateTime endDate)
    {
        var incomes = _transactionRepository.GetAll()
            .Where(t => t is Income)
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToList();

        if (!incomes.Any())
            return new List<CategoryReport>();

        var totalIncome = incomes.Sum(t => t.Amount.Amount);

        var incomeReport = incomes
            .GroupBy(t => t.CategoryId)
            .Select(g => new CategoryReport
            {
                CategoryId = g.Key,
                CategoryName = _categoryRepository.GetById(g.Key)?.Name ?? "Unknown",
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(t => t.Amount.Amount),
                AverageAmount = g.Average(t => t.Amount.Amount),
                MinAmount = g.Min(t => t.Amount.Amount),
                MaxAmount = g.Max(t => t.Amount.Amount),
                Percentage = (g.Sum(t => t.Amount.Amount) / totalIncome) * 100,
                StartDate = startDate,
                EndDate = endDate
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();

        return incomeReport;
    }

    /// <summary>
    /// Generate monthly trend report using LINQ GroupBy
    /// </summary>
    public List<TrendReport> GenerateMonthlyTrendReport(DateTime startDate, DateTime endDate)
    {
        var transactions = _transactionRepository.GetAll()
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToList();

        var monthlyTrends = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new TrendReport
            {
                Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                PeriodStart = new DateTime(g.Key.Year, g.Key.Month, 1),
                PeriodEnd = new DateTime(g.Key.Year, g.Key.Month, 1).AddMonths(1).AddDays(-1),
                TotalIncome = g.Where(t => t is Income).Sum(t => t.Amount.Amount),
                TotalExpense = g.Where(t => t is Expense).Sum(t => t.Amount.Amount),
                NetAmount = g.Where(t => t is Income).Sum(t => t.Amount.Amount) -
                           g.Where(t => t is Expense).Sum(t => t.Amount.Amount),
                TransactionCount = g.Count(),
                IncomeCount = g.Count(t => t is Income),
                ExpenseCount = g.Count(t => t is Expense)
            })
            .OrderBy(x => x.PeriodStart)
            .ToList();

        return monthlyTrends;
    }

    /// <summary>
    /// Generate weekly trend report using LINQ GroupBy
    /// </summary>
    public List<TrendReport> GenerateWeeklyTrendReport(DateTime startDate, DateTime endDate)
    {
        var transactions = _transactionRepository.GetAll()
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToList();

        var weeklyTrends = transactions
            .GroupBy(t => new
            {
                t.Date.Year,
                Week = GetWeekNumber(t.Date)
            })
            .Select(g => new TrendReport
            {
                Period = $"{g.Key.Year}-W{g.Key.Week:D2}",
                PeriodStart = g.Min(t => t.Date),
                PeriodEnd = g.Max(t => t.Date),
                TotalIncome = g.Where(t => t is Income).Sum(t => t.Amount.Amount),
                TotalExpense = g.Where(t => t is Expense).Sum(t => t.Amount.Amount),
                NetAmount = g.Where(t => t is Income).Sum(t => t.Amount.Amount) -
                           g.Where(t => t is Expense).Sum(t => t.Amount.Amount),
                TransactionCount = g.Count(),
                IncomeCount = g.Count(t => t is Income),
                ExpenseCount = g.Count(t => t is Expense)
            })
            .OrderBy(x => x.PeriodStart)
            .ToList();

        return weeklyTrends;
    }

    /// <summary>
    /// Generate daily trend report using LINQ GroupBy
    /// </summary>
    public List<TrendReport> GenerateDailyTrendReport(DateTime startDate, DateTime endDate)
    {
        var transactions = _transactionRepository.GetAll()
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToList();

        var dailyTrends = transactions
            .GroupBy(t => t.Date.Date)
            .Select(g => new TrendReport
            {
                Period = g.Key.ToString("yyyy-MM-dd"),
                PeriodStart = g.Key,
                PeriodEnd = g.Key,
                TotalIncome = g.Where(t => t is Income).Sum(t => t.Amount.Amount),
                TotalExpense = g.Where(t => t is Expense).Sum(t => t.Amount.Amount),
                NetAmount = g.Where(t => t is Income).Sum(t => t.Amount.Amount) -
                           g.Where(t => t is Expense).Sum(t => t.Amount.Amount),
                TransactionCount = g.Count(),
                IncomeCount = g.Count(t => t is Income),
                ExpenseCount = g.Count(t => t is Expense)
            })
            .OrderBy(x => x.PeriodStart)
            .ToList();

        return dailyTrends;
    }

    /// <summary>
    /// Generate comprehensive financial summary using LINQ
    /// </summary>
    public FinancialSummary GenerateFinancialSummary(DateTime startDate, DateTime endDate)
    {
        var transactions = _transactionRepository.GetAll()
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToList();

        var incomes = transactions.Where(t => t is Income).ToList();
        var expenses = transactions.Where(t => t is Expense).ToList();

        return new FinancialSummary
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalIncome = incomes.Sum(t => t.Amount.Amount),
            TotalExpense = expenses.Sum(t => t.Amount.Amount),
            NetAmount = incomes.Sum(t => t.Amount.Amount) - expenses.Sum(t => t.Amount.Amount),
            TotalTransactions = transactions.Count,
            IncomeTransactions = incomes.Count,
            ExpenseTransactions = expenses.Count,
            AverageIncome = incomes.Any() ? incomes.Average(t => t.Amount.Amount) : 0,
            AverageExpense = expenses.Any() ? expenses.Average(t => t.Amount.Amount) : 0,
            LargestIncome = incomes.Any() ? incomes.Max(t => t.Amount.Amount) : 0,
            LargestExpense = expenses.Any() ? expenses.Max(t => t.Amount.Amount) : 0,
            SmallestIncome = incomes.Any() ? incomes.Min(t => t.Amount.Amount) : 0,
            SmallestExpense = expenses.Any() ? expenses.Min(t => t.Amount.Amount) : 0
        };
    }

    /// <summary>
    /// Generate budget comparison report using LINQ
    /// </summary>
    public List<BudgetComparisonReport> GenerateBudgetComparisonReport()
    {
        var budgetService = new BudgetService(
            _budgetRepository,
            _transactionRepository,
            _categoryRepository
        );

        var budgetSummaries = budgetService.GetAllActiveBudgetsWithSpending();

        var comparisonReport = budgetSummaries
            .Select(b => new BudgetComparisonReport
            {
                BudgetId = b.BudgetId,
                BudgetName = b.BudgetName,
                CategoryName = b.CategoryName,
                BudgetedAmount = b.BudgetAmount,
                ActualAmount = b.ActualSpending,
                Difference = b.Remaining,
                PercentageUsed = b.PercentageUsed,
                Status = b.IsExceeded ? "Exceeded" : b.IsWarning ? "Warning" : "Healthy",
                TransactionCount = b.TransactionCount
            })
            .OrderByDescending(b => b.PercentageUsed)
            .ToList();

        return comparisonReport;
    }

    /// <summary>
    /// Get top spending categories using LINQ
    /// </summary>
    public List<CategoryReport> GetTopSpendingCategories(int topN, DateTime startDate, DateTime endDate)
    {
        var categoryReport = GenerateCategorySpendingReport(startDate, endDate);

        return categoryReport
            .OrderByDescending(c => c.TotalAmount)
            .Take(topN)
            .ToList();
    }

    /// <summary>
    /// Get top income sources using LINQ
    /// </summary>
    public List<CategoryReport> GetTopIncomeSources(int topN, DateTime startDate, DateTime endDate)
    {
        var incomeReport = GenerateIncomeReport(startDate, endDate);

        return incomeReport
            .OrderByDescending(c => c.TotalAmount)
            .Take(topN)
            .ToList();
    }

    /// <summary>
    /// Export report data to CSV format
    /// Phase 5.16: Export Functionality
    /// </summary>
    public string ExportCategoryReportToCsv(List<CategoryReport> reports)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Category,Transaction Count,Total Amount,Average Amount,Min Amount,Max Amount,Percentage");

        foreach (var report in reports)
        {
            csv.AppendLine($"\"{report.CategoryName}\",{report.TransactionCount},{report.TotalAmount:F2},{report.AverageAmount:F2},{report.MinAmount:F2},{report.MaxAmount:F2},{report.Percentage:F2}%");
        }

        return csv.ToString();
    }

    /// <summary>
    /// Export trend report to CSV format
    /// </summary>
    public string ExportTrendReportToCsv(List<TrendReport> reports)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Period,Income,Expense,Net Amount,Transaction Count");

        foreach (var report in reports)
        {
            csv.AppendLine($"\"{report.Period}\",{report.TotalIncome:F2},{report.TotalExpense:F2},{report.NetAmount:F2},{report.TransactionCount}");
        }

        return csv.ToString();
    }

    /// <summary>
    /// Export budget comparison report to CSV format
    /// </summary>
    public string ExportBudgetComparisonToCsv(List<BudgetComparisonReport> reports)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Budget Name,Category,Budgeted,Actual,Difference,% Used,Status,Transactions");

        foreach (var report in reports)
        {
            csv.AppendLine($"\"{report.BudgetName}\",\"{report.CategoryName}\",{report.BudgetedAmount:F2},{report.ActualAmount:F2},{report.Difference:F2},{report.PercentageUsed:F2}%,\"{report.Status}\",{report.TransactionCount}");
        }

        return csv.ToString();
    }

    /// <summary>
    /// Export financial summary to CSV format
    /// </summary>
    public string ExportFinancialSummaryToCsv(FinancialSummary summary)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"\"Period\",\"{summary.StartDate:yyyy-MM-dd} to {summary.EndDate:yyyy-MM-dd}\"");
        csv.AppendLine($"\"Total Income\",{summary.TotalIncome:F2}");
        csv.AppendLine($"\"Total Expense\",{summary.TotalExpense:F2}");
        csv.AppendLine($"\"Net Amount\",{summary.NetAmount:F2}");
        csv.AppendLine($"\"Total Transactions\",{summary.TotalTransactions}");
        csv.AppendLine($"\"Income Transactions\",{summary.IncomeTransactions}");
        csv.AppendLine($"\"Expense Transactions\",{summary.ExpenseTransactions}");
        csv.AppendLine($"\"Average Income\",{summary.AverageIncome:F2}");
        csv.AppendLine($"\"Average Expense\",{summary.AverageExpense:F2}");
        csv.AppendLine($"\"Largest Income\",{summary.LargestIncome:F2}");
        csv.AppendLine($"\"Largest Expense\",{summary.LargestExpense:F2}");

        return csv.ToString();
    }

    /// <summary>
    /// Helper method to calculate week number
    /// </summary>
    private int GetWeekNumber(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var dateTimeFormat = culture.DateTimeFormat;

        return calendar.GetWeekOfYear(date, dateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);
    }
}

/// <summary>
/// Category report data transfer object
/// </summary>
public class CategoryReport
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// Trend report data transfer object
/// </summary>
public class TrendReport
{
    public string Period { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetAmount { get; set; }
    public int TransactionCount { get; set; }
    public int IncomeCount { get; set; }
    public int ExpenseCount { get; set; }
}

/// <summary>
/// Budget comparison report data transfer object
/// </summary>
public class BudgetComparisonReport
{
    public int BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal Difference { get; set; }
    public decimal PercentageUsed { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
}

/// <summary>
/// Financial summary data transfer object
/// </summary>
public class FinancialSummary
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetAmount { get; set; }
    public int TotalTransactions { get; set; }
    public int IncomeTransactions { get; set; }
    public int ExpenseTransactions { get; set; }
    public decimal AverageIncome { get; set; }
    public decimal AverageExpense { get; set; }
    public decimal LargestIncome { get; set; }
    public decimal LargestExpense { get; set; }
    public decimal SmallestIncome { get; set; }
    public decimal SmallestExpense { get; set; }
}

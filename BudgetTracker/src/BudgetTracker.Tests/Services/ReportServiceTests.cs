using BudgetTracker.Core.Services;
using BudgetTracker.Data;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Tests.Services;

/// <summary>
/// NUnit tests for ReportService
/// Phase 6.19: NUnit Test Writing (Required)
/// Tests LINQ queries, lambda expressions, and reporting functionality
/// </summary>
[TestFixture]
public class ReportServiceTests
{
    private InMemoryRepository<Transaction> _transactionRepository = null!;
    private InMemoryRepository<Category> _categoryRepository = null!;
    private InMemoryRepository<Budget> _budgetRepository = null!;
    private ReportService _reportService = null!;
    private Category _foodCategory = null!;
    private Category _salaryCategory = null!;
    private Category _transportCategory = null!;

    [SetUp]
    public void Setup()
    {
        // Initialize repositories
        _transactionRepository = new InMemoryRepository<Transaction>();
        _categoryRepository = new InMemoryRepository<Category>();
        _budgetRepository = new InMemoryRepository<Budget>();

        // Create test categories
        _foodCategory = new Category { Id = 1, Name = "Food", Description = "Food and groceries" };
        _salaryCategory = new Category { Id = 2, Name = "Salary", Description = "Income from salary" };
        _transportCategory = new Category { Id = 3, Name = "Transportation", Description = "Transport expenses" };

        _categoryRepository.Add(_foodCategory);
        _categoryRepository.Add(_salaryCategory);
        _categoryRepository.Add(_transportCategory);

        // Initialize service
        _reportService = new ReportService(
            _transactionRepository,
            _categoryRepository,
            _budgetRepository
        );
    }

    [Test]
    public void GenerateCategorySpendingReport_WithNoExpenses_ReturnsEmptyList()
    {
        // Act
        var report = _reportService.GenerateCategorySpendingReport(
            DateTime.Today.AddDays(-30),
            DateTime.Today
        );

        // Assert
        Assert.That(report, Is.Empty);
    }

    [Test]
    public void GenerateCategorySpendingReport_WithExpenses_ReturnsCorrectReport()
    {
        // Arrange
        _transactionRepository.Add(new Expense("Groceries", new Money(100, "USD"), DateTime.Today.AddDays(-5), _foodCategory.Id));
        _transactionRepository.Add(new Expense("Restaurant", new Money(50, "USD"), DateTime.Today.AddDays(-3), _foodCategory.Id));
        _transactionRepository.Add(new Expense("Gas", new Money(45, "USD"), DateTime.Today.AddDays(-2), _transportCategory.Id));

        // Act
        var report = _reportService.GenerateCategorySpendingReport(
            DateTime.Today.AddDays(-30),
            DateTime.Today
        );

        // Assert
        Assert.That(report.Count, Is.EqualTo(2));

        var foodReport = report.First(r => r.CategoryId == _foodCategory.Id);
        Assert.That(foodReport.TotalAmount, Is.EqualTo(150));
        Assert.That(foodReport.TransactionCount, Is.EqualTo(2));
        Assert.That(foodReport.AverageAmount, Is.EqualTo(75));
        Assert.That(foodReport.MinAmount, Is.EqualTo(50));
        Assert.That(foodReport.MaxAmount, Is.EqualTo(100));

        var transportReport = report.First(r => r.CategoryId == _transportCategory.Id);
        Assert.That(transportReport.TotalAmount, Is.EqualTo(45));
        Assert.That(transportReport.TransactionCount, Is.EqualTo(1));
    }

    [Test]
    public void GenerateCategorySpendingReport_CalculatesPercentagesCorrectly()
    {
        // Arrange
        _transactionRepository.Add(new Expense("Groceries", new Money(100, "USD"), DateTime.Today, _foodCategory.Id));
        _transactionRepository.Add(new Expense("Gas", new Money(50, "USD"), DateTime.Today, _transportCategory.Id));

        // Act
        var report = _reportService.GenerateCategorySpendingReport(
            DateTime.Today.AddDays(-7),
            DateTime.Today
        );

        // Assert
        var foodReport = report.First(r => r.CategoryId == _foodCategory.Id);
        Assert.That(foodReport.Percentage, Is.EqualTo(66.67).Within(0.01));

        var transportReport = report.First(r => r.CategoryId == _transportCategory.Id);
        Assert.That(transportReport.Percentage, Is.EqualTo(33.33).Within(0.01));
    }

    [Test]
    public void GenerateCategorySpendingReport_OrdersByAmountDescending()
    {
        // Arrange
        _transactionRepository.Add(new Expense("Small expense", new Money(10, "USD"), DateTime.Today, _transportCategory.Id));
        _transactionRepository.Add(new Expense("Large expense", new Money(500, "USD"), DateTime.Today, _foodCategory.Id));

        // Act
        var report = _reportService.GenerateCategorySpendingReport(
            DateTime.Today.AddDays(-7),
            DateTime.Today
        );

        // Assert
        Assert.That(report[0].CategoryId, Is.EqualTo(_foodCategory.Id));
        Assert.That(report[1].CategoryId, Is.EqualTo(_transportCategory.Id));
    }

    [Test]
    public void GenerateIncomeReport_WithIncomes_ReturnsCorrectReport()
    {
        // Arrange
        _transactionRepository.Add(new Income("Monthly Salary", new Money(5000, "USD"), DateTime.Today.AddDays(-5), _salaryCategory.Id));
        _transactionRepository.Add(new Income("Bonus", new Money(1000, "USD"), DateTime.Today.AddDays(-2), _salaryCategory.Id));

        // Act
        var report = _reportService.GenerateIncomeReport(
            DateTime.Today.AddDays(-30),
            DateTime.Today
        );

        // Assert
        Assert.That(report.Count, Is.EqualTo(1));

        var salaryReport = report.First();
        Assert.That(salaryReport.TotalAmount, Is.EqualTo(6000));
        Assert.That(salaryReport.TransactionCount, Is.EqualTo(2));
        Assert.That(salaryReport.AverageAmount, Is.EqualTo(3000));
    }

    [Test]
    public void GenerateIncomeReport_WithNoIncome_ReturnsEmptyList()
    {
        // Act
        var report = _reportService.GenerateIncomeReport(
            DateTime.Today.AddDays(-30),
            DateTime.Today
        );

        // Assert
        Assert.That(report, Is.Empty);
    }

    [Test]
    public void GenerateMonthlyTrendReport_ReturnsCorrectMonthlyData()
    {
        // Arrange
        var currentMonth = DateTime.Today;
        var lastMonth = DateTime.Today.AddMonths(-1);

        _transactionRepository.Add(new Income("Current Salary", new Money(5000, "USD"), currentMonth, _salaryCategory.Id));
        _transactionRepository.Add(new Expense("Current Expense", new Money(2000, "USD"), currentMonth, _foodCategory.Id));
        _transactionRepository.Add(new Income("Last Salary", new Money(5000, "USD"), lastMonth, _salaryCategory.Id));
        _transactionRepository.Add(new Expense("Last Expense", new Money(1500, "USD"), lastMonth, _foodCategory.Id));

        // Act
        var report = _reportService.GenerateMonthlyTrendReport(
            DateTime.Today.AddMonths(-2),
            DateTime.Today
        );

        // Assert
        Assert.That(report.Count, Is.GreaterThanOrEqualTo(2));

        var currentMonthReport = report.FirstOrDefault(r => r.PeriodStart.Month == currentMonth.Month);
        Assert.That(currentMonthReport, Is.Not.Null);
        Assert.That(currentMonthReport.TotalIncome, Is.EqualTo(5000));
        Assert.That(currentMonthReport.TotalExpense, Is.EqualTo(2000));
        Assert.That(currentMonthReport.NetAmount, Is.EqualTo(3000));
    }

    [Test]
    public void GenerateWeeklyTrendReport_ReturnsCorrectWeeklyData()
    {
        // Arrange
        _transactionRepository.Add(new Income("Income", new Money(1000, "USD"), DateTime.Today, _salaryCategory.Id));
        _transactionRepository.Add(new Expense("Expense", new Money(500, "USD"), DateTime.Today, _foodCategory.Id));

        // Act
        var report = _reportService.GenerateWeeklyTrendReport(
            DateTime.Today.AddDays(-14),
            DateTime.Today
        );

        // Assert
        Assert.That(report, Is.Not.Empty);
        Assert.That(report.Any(r => r.TotalIncome > 0), Is.True);
    }

    [Test]
    public void GenerateDailyTrendReport_ReturnsCorrectDailyData()
    {
        // Arrange
        _transactionRepository.Add(new Income("Daily Income", new Money(100, "USD"), DateTime.Today, _salaryCategory.Id));
        _transactionRepository.Add(new Expense("Daily Expense", new Money(50, "USD"), DateTime.Today, _foodCategory.Id));

        // Act
        var report = _reportService.GenerateDailyTrendReport(
            DateTime.Today.AddDays(-7),
            DateTime.Today
        );

        // Assert
        Assert.That(report, Is.Not.Empty);

        var todayReport = report.FirstOrDefault(r => r.PeriodStart.Date == DateTime.Today.Date);
        Assert.That(todayReport, Is.Not.Null);
        Assert.That(todayReport.TotalIncome, Is.EqualTo(100));
        Assert.That(todayReport.TotalExpense, Is.EqualTo(50));
        Assert.That(todayReport.NetAmount, Is.EqualTo(50));
    }

    [Test]
    public void GenerateFinancialSummary_CalculatesAllMetricsCorrectly()
    {
        // Arrange
        _transactionRepository.Add(new Income("Salary 1", new Money(5000, "USD"), DateTime.Today.AddDays(-5), _salaryCategory.Id));
        _transactionRepository.Add(new Income("Salary 2", new Money(3000, "USD"), DateTime.Today.AddDays(-3), _salaryCategory.Id));
        _transactionRepository.Add(new Expense("Expense 1", new Money(2000, "USD"), DateTime.Today.AddDays(-4), _foodCategory.Id));
        _transactionRepository.Add(new Expense("Expense 2", new Money(1000, "USD"), DateTime.Today.AddDays(-2), _foodCategory.Id));

        // Act
        var summary = _reportService.GenerateFinancialSummary(
            DateTime.Today.AddDays(-7),
            DateTime.Today
        );

        // Assert
        Assert.That(summary.TotalIncome, Is.EqualTo(8000));
        Assert.That(summary.TotalExpense, Is.EqualTo(3000));
        Assert.That(summary.NetAmount, Is.EqualTo(5000));
        Assert.That(summary.TotalTransactions, Is.EqualTo(4));
        Assert.That(summary.IncomeTransactions, Is.EqualTo(2));
        Assert.That(summary.ExpenseTransactions, Is.EqualTo(2));
        Assert.That(summary.AverageIncome, Is.EqualTo(4000));
        Assert.That(summary.AverageExpense, Is.EqualTo(1500));
        Assert.That(summary.LargestIncome, Is.EqualTo(5000));
        Assert.That(summary.LargestExpense, Is.EqualTo(2000));
        Assert.That(summary.SmallestIncome, Is.EqualTo(3000));
        Assert.That(summary.SmallestExpense, Is.EqualTo(1000));
    }

    [Test]
    public void GenerateFinancialSummary_WithNoTransactions_ReturnsZeros()
    {
        // Act
        var summary = _reportService.GenerateFinancialSummary(
            DateTime.Today.AddDays(-30),
            DateTime.Today
        );

        // Assert
        Assert.That(summary.TotalIncome, Is.EqualTo(0));
        Assert.That(summary.TotalExpense, Is.EqualTo(0));
        Assert.That(summary.NetAmount, Is.EqualTo(0));
        Assert.That(summary.TotalTransactions, Is.EqualTo(0));
        Assert.That(summary.AverageIncome, Is.EqualTo(0));
        Assert.That(summary.AverageExpense, Is.EqualTo(0));
    }

    [Test]
    public void GenerateBudgetComparisonReport_ReturnsCorrectData()
    {
        // Arrange
        var budget = new Budget(
            "Food Budget",
            new Money(500, "USD"),
            _foodCategory.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(20)
        );
        _budgetRepository.Add(budget);

        _transactionRepository.Add(new Expense("Groceries", new Money(300, "USD"), DateTime.Today, _foodCategory.Id));

        // Act
        var report = _reportService.GenerateBudgetComparisonReport();

        // Assert
        Assert.That(report.Count, Is.EqualTo(1));

        var budgetReport = report.First();
        Assert.That(budgetReport.BudgetName, Is.EqualTo("Food Budget"));
        Assert.That(budgetReport.BudgetedAmount, Is.EqualTo(500));
        Assert.That(budgetReport.ActualAmount, Is.EqualTo(300));
        Assert.That(budgetReport.Difference, Is.EqualTo(200));
        Assert.That(budgetReport.PercentageUsed, Is.EqualTo(60));
        Assert.That(budgetReport.Status, Is.EqualTo("Healthy"));
    }

    [Test]
    public void GenerateBudgetComparisonReport_WithExceededBudget_ReturnsExceededStatus()
    {
        // Arrange
        var budget = new Budget(
            "Small Budget",
            new Money(100, "USD"),
            _foodCategory.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(20)
        );
        _budgetRepository.Add(budget);

        _transactionRepository.Add(new Expense("Large Expense", new Money(150, "USD"), DateTime.Today, _foodCategory.Id));

        // Act
        var report = _reportService.GenerateBudgetComparisonReport();

        // Assert
        Assert.That(report.Count, Is.EqualTo(1));
        Assert.That(report[0].Status, Is.EqualTo("Exceeded"));
    }

    [Test]
    public void GetTopSpendingCategories_ReturnsTopN()
    {
        // Arrange
        _transactionRepository.Add(new Expense("Food 1", new Money(500, "USD"), DateTime.Today, _foodCategory.Id));
        _transactionRepository.Add(new Expense("Food 2", new Money(300, "USD"), DateTime.Today, _foodCategory.Id));
        _transactionRepository.Add(new Expense("Transport", new Money(100, "USD"), DateTime.Today, _transportCategory.Id));

        // Act
        var topCategories = _reportService.GetTopSpendingCategories(1, DateTime.Today.AddDays(-7), DateTime.Today);

        // Assert
        Assert.That(topCategories.Count, Is.EqualTo(1));
        Assert.That(topCategories[0].CategoryId, Is.EqualTo(_foodCategory.Id));
        Assert.That(topCategories[0].TotalAmount, Is.EqualTo(800));
    }

    [Test]
    public void GetTopIncomeSources_ReturnsTopN()
    {
        // Arrange
        _transactionRepository.Add(new Income("Salary", new Money(5000, "USD"), DateTime.Today, _salaryCategory.Id));
        _transactionRepository.Add(new Income("Bonus", new Money(1000, "USD"), DateTime.Today, _salaryCategory.Id));

        // Act
        var topSources = _reportService.GetTopIncomeSources(1, DateTime.Today.AddDays(-7), DateTime.Today);

        // Assert
        Assert.That(topSources.Count, Is.EqualTo(1));
        Assert.That(topSources[0].CategoryId, Is.EqualTo(_salaryCategory.Id));
        Assert.That(topSources[0].TotalAmount, Is.EqualTo(6000));
    }

    [Test]
    public void ExportCategoryReportToCsv_GeneratesValidCsv()
    {
        // Arrange
        var reports = new List<CategoryReport>
        {
            new CategoryReport
            {
                CategoryName = "Food",
                TransactionCount = 5,
                TotalAmount = 500,
                AverageAmount = 100,
                MinAmount = 50,
                MaxAmount = 200,
                Percentage = 60
            }
        };

        // Act
        var csv = _reportService.ExportCategoryReportToCsv(reports);

        // Assert
        Assert.That(csv, Is.Not.Null);
        Assert.That(csv, Does.Contain("Category,Transaction Count,Total Amount"));
        Assert.That(csv, Does.Contain("Food"));
        Assert.That(csv, Does.Contain("500.00"));
    }

    [Test]
    public void ExportTrendReportToCsv_GeneratesValidCsv()
    {
        // Arrange
        var reports = new List<TrendReport>
        {
            new TrendReport
            {
                Period = "2024-01",
                TotalIncome = 5000,
                TotalExpense = 3000,
                NetAmount = 2000,
                TransactionCount = 10
            }
        };

        // Act
        var csv = _reportService.ExportTrendReportToCsv(reports);

        // Assert
        Assert.That(csv, Is.Not.Null);
        Assert.That(csv, Does.Contain("Period,Income,Expense"));
        Assert.That(csv, Does.Contain("2024-01"));
        Assert.That(csv, Does.Contain("5000.00"));
    }

    [Test]
    public void ExportBudgetComparisonToCsv_GeneratesValidCsv()
    {
        // Arrange
        var reports = new List<BudgetComparisonReport>
        {
            new BudgetComparisonReport
            {
                BudgetName = "Food Budget",
                CategoryName = "Food",
                BudgetedAmount = 500,
                ActualAmount = 300,
                Difference = 200,
                PercentageUsed = 60,
                Status = "Healthy",
                TransactionCount = 5
            }
        };

        // Act
        var csv = _reportService.ExportBudgetComparisonToCsv(reports);

        // Assert
        Assert.That(csv, Is.Not.Null);
        Assert.That(csv, Does.Contain("Budget Name,Category,Budgeted"));
        Assert.That(csv, Does.Contain("Food Budget"));
        Assert.That(csv, Does.Contain("Healthy"));
    }

    [Test]
    public void ExportFinancialSummaryToCsv_GeneratesValidCsv()
    {
        // Arrange
        var summary = new FinancialSummary
        {
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today,
            TotalIncome = 5000,
            TotalExpense = 3000,
            NetAmount = 2000,
            TotalTransactions = 10,
            IncomeTransactions = 4,
            ExpenseTransactions = 6,
            AverageIncome = 1250,
            AverageExpense = 500,
            LargestIncome = 2000,
            LargestExpense = 1000
        };

        // Act
        var csv = _reportService.ExportFinancialSummaryToCsv(summary);

        // Assert
        Assert.That(csv, Is.Not.Null);
        Assert.That(csv, Does.Contain("Metric,Value"));
        Assert.That(csv, Does.Contain("Total Income"));
        Assert.That(csv, Does.Contain("5000.00"));
        Assert.That(csv, Does.Contain("Net Amount"));
    }

    [Test]
    public void GenerateCategorySpendingReport_WithDateRange_FiltersCorrectly()
    {
        // Arrange
        _transactionRepository.Add(new Expense("Old Expense", new Money(100, "USD"), DateTime.Today.AddDays(-60), _foodCategory.Id));
        _transactionRepository.Add(new Expense("Recent Expense", new Money(200, "USD"), DateTime.Today.AddDays(-5), _foodCategory.Id));

        // Act
        var report = _reportService.GenerateCategorySpendingReport(
            DateTime.Today.AddDays(-30),
            DateTime.Today
        );

        // Assert
        Assert.That(report.Count, Is.EqualTo(1));
        Assert.That(report[0].TotalAmount, Is.EqualTo(200));
    }

    [Test]
    public void GenerateIncomeReport_WithDateRange_FiltersCorrectly()
    {
        // Arrange
        _transactionRepository.Add(new Income("Old Income", new Money(1000, "USD"), DateTime.Today.AddDays(-60), _salaryCategory.Id));
        _transactionRepository.Add(new Income("Recent Income", new Money(2000, "USD"), DateTime.Today.AddDays(-5), _salaryCategory.Id));

        // Act
        var report = _reportService.GenerateIncomeReport(
            DateTime.Today.AddDays(-30),
            DateTime.Today
        );

        // Assert
        Assert.That(report.Count, Is.EqualTo(1));
        Assert.That(report[0].TotalAmount, Is.EqualTo(2000));
    }

    [Test]
    public void GenerateCategorySpendingReport_DoesNotIncludeIncome()
    {
        // Arrange
        _transactionRepository.Add(new Income("Income", new Money(1000, "USD"), DateTime.Today, _salaryCategory.Id));
        _transactionRepository.Add(new Expense("Expense", new Money(100, "USD"), DateTime.Today, _foodCategory.Id));

        // Act
        var report = _reportService.GenerateCategorySpendingReport(
            DateTime.Today.AddDays(-7),
            DateTime.Today
        );

        // Assert
        Assert.That(report.Count, Is.EqualTo(1));
        Assert.That(report[0].CategoryId, Is.EqualTo(_foodCategory.Id));
    }

    [Test]
    public void GenerateIncomeReport_DoesNotIncludeExpenses()
    {
        // Arrange
        _transactionRepository.Add(new Income("Income", new Money(1000, "USD"), DateTime.Today, _salaryCategory.Id));
        _transactionRepository.Add(new Expense("Expense", new Money(100, "USD"), DateTime.Today, _foodCategory.Id));

        // Act
        var report = _reportService.GenerateIncomeReport(
            DateTime.Today.AddDays(-7),
            DateTime.Today
        );

        // Assert
        Assert.That(report.Count, Is.EqualTo(1));
        Assert.That(report[0].CategoryId, Is.EqualTo(_salaryCategory.Id));
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up is automatic with in-memory repositories
    }
}

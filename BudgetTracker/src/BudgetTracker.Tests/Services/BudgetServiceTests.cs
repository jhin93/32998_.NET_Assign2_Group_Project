using BudgetTracker.Core.Services;
using BudgetTracker.Data;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Tests.Services;

/// <summary>
/// NUnit tests for BudgetService
/// Phase 6.19: NUnit Test Writing (Required)
/// Demonstrates testing requirement
/// </summary>
[TestFixture]
public class BudgetServiceTests
{
    private InMemoryRepository<Budget> _budgetRepository = null!;
    private InMemoryRepository<Transaction> _transactionRepository = null!;
    private InMemoryRepository<Category> _categoryRepository = null!;
    private BudgetService _budgetService = null!;
    private Category _testCategory = null!;

    [SetUp]
    public void Setup()
    {
        // Initialize repositories
        _budgetRepository = new InMemoryRepository<Budget>();
        _transactionRepository = new InMemoryRepository<Transaction>();
        _categoryRepository = new InMemoryRepository<Category>();

        // Create test category
        _testCategory = new Category { Id = 1, Name = "Food", Description = "Food expenses" };
        _categoryRepository.Add(_testCategory);

        // Initialize service
        _budgetService = new BudgetService(
            _budgetRepository,
            _transactionRepository,
            _categoryRepository
        );
    }

    [Test]
    public void CalculateSpendingVsBudget_WithNoTransactions_ReturnsZeroSpending()
    {
        // Arrange
        var budget = new Budget(
            "Monthly Food Budget",
            new Money(500, "USD"),
            _testCategory.Id,
            DateTime.Today,
            DateTime.Today.AddMonths(1)
        );
        _budgetRepository.Add(budget);

        // Act
        var result = _budgetService.CalculateSpendingVsBudget(budget.Id);

        // Assert
        Assert.That(result.ActualSpending, Is.EqualTo(0));
        Assert.That(result.BudgetAmount, Is.EqualTo(500));
        Assert.That(result.Remaining, Is.EqualTo(500));
        Assert.That(result.IsExceeded, Is.False);
    }

    [Test]
    public void CalculateSpendingVsBudget_WithExpenses_CalculatesCorrectly()
    {
        // Arrange
        var budget = new Budget(
            "Monthly Food Budget",
            new Money(500, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(20)
        );
        _budgetRepository.Add(budget);

        // Add test expenses
        _transactionRepository.Add(new Expense("Groceries", new Money(100, "USD"), DateTime.Today.AddDays(-5), _testCategory.Id));
        _transactionRepository.Add(new Expense("Restaurant", new Money(50, "USD"), DateTime.Today.AddDays(-2), _testCategory.Id));

        // Act
        var result = _budgetService.CalculateSpendingVsBudget(budget.Id);

        // Assert
        Assert.That(result.ActualSpending, Is.EqualTo(150));
        Assert.That(result.Remaining, Is.EqualTo(350));
        Assert.That(result.PercentageUsed, Is.EqualTo(30).Within(0.01));
        Assert.That(result.IsExceeded, Is.False);
        Assert.That(result.IsWarning, Is.False);
    }

    [Test]
    public void CalculateSpendingVsBudget_WhenExceeded_ReturnsIsExceededTrue()
    {
        // Arrange
        var budget = new Budget(
            "Monthly Food Budget",
            new Money(100, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(20)
        );
        _budgetRepository.Add(budget);

        _transactionRepository.Add(new Expense("Large Purchase", new Money(150, "USD"), DateTime.Today, _testCategory.Id));

        // Act
        var result = _budgetService.CalculateSpendingVsBudget(budget.Id);

        // Assert
        Assert.That(result.ActualSpending, Is.EqualTo(150));
        Assert.That(result.IsExceeded, Is.True);
        Assert.That(result.Remaining, Is.EqualTo(-50));
    }

    [Test]
    public void CalculateSpendingVsBudget_AtWarningThreshold_ReturnsIsWarningTrue()
    {
        // Arrange
        var budget = new Budget(
            "Monthly Food Budget",
            new Money(100, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(20)
        );
        _budgetRepository.Add(budget);

        _transactionRepository.Add(new Expense("Purchase", new Money(85, "USD"), DateTime.Today, _testCategory.Id));

        // Act
        var result = _budgetService.CalculateSpendingVsBudget(budget.Id);

        // Assert
        Assert.That(result.PercentageUsed, Is.EqualTo(85));
        Assert.That(result.IsWarning, Is.True);
        Assert.That(result.IsExceeded, Is.False);
    }

    [Test]
    public void CalculateSpendingVsBudget_WithInvalidBudgetId_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            _budgetService.CalculateSpendingVsBudget(999);
        });
    }

    [Test]
    public void GetAllActiveBudgetsWithSpending_ReturnsOnlyActiveBudgets()
    {
        // Arrange
        var activeBudget = new Budget(
            "Active Budget",
            new Money(500, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-5),
            DateTime.Today.AddDays(25)
        );
        _budgetRepository.Add(activeBudget);

        var inactiveBudget = new Budget(
            "Inactive Budget",
            new Money(300, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-60),
            DateTime.Today.AddDays(-30)
        );
        _budgetRepository.Add(inactiveBudget);

        // Act
        var results = _budgetService.GetAllActiveBudgetsWithSpending();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0].BudgetName, Is.EqualTo("Active Budget"));
    }

    [Test]
    public void GetExceededBudgets_ReturnsOnlyExceededBudgets()
    {
        // Arrange
        var exceededBudget = new Budget(
            "Exceeded Budget",
            new Money(100, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-5),
            DateTime.Today.AddDays(25)
        );
        _budgetRepository.Add(exceededBudget);

        _transactionRepository.Add(new Expense("Large expense", new Money(150, "USD"), DateTime.Today, _testCategory.Id));

        var normalBudget = new Budget(
            "Normal Budget",
            new Money(500, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-5),
            DateTime.Today.AddDays(25)
        );
        _budgetRepository.Add(normalBudget);

        // Act
        var results = _budgetService.GetExceededBudgets();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0].BudgetName, Is.EqualTo("Exceeded Budget"));
        Assert.That(results[0].AlertLevel, Is.EqualTo(AlertLevel.Critical));
    }

    [Test]
    public void WouldExceedBudget_WithExceedingAmount_ReturnsTrue()
    {
        // Arrange
        var budget = new Budget(
            "Monthly Food Budget",
            new Money(100, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-5),
            DateTime.Today.AddDays(25)
        );
        _budgetRepository.Add(budget);

        _transactionRepository.Add(new Expense("Existing", new Money(60, "USD"), DateTime.Today.AddDays(-2), _testCategory.Id));

        // Act
        var result = _budgetService.WouldExceedBudget(_testCategory.Id, 50, DateTime.Today);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void WouldExceedBudget_WithNonExceedingAmount_ReturnsFalse()
    {
        // Arrange
        var budget = new Budget(
            "Monthly Food Budget",
            new Money(100, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-5),
            DateTime.Today.AddDays(25)
        );
        _budgetRepository.Add(budget);

        _transactionRepository.Add(new Expense("Existing", new Money(60, "USD"), DateTime.Today.AddDays(-2), _testCategory.Id));

        // Act
        var result = _budgetService.WouldExceedBudget(_testCategory.Id, 30, DateTime.Today);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetBudgetUtilizationReport_CalculatesCorrectTotals()
    {
        // Arrange
        var budget1 = new Budget("Budget 1", new Money(500, "USD"), _testCategory.Id, DateTime.Today.AddDays(-5), DateTime.Today.AddDays(25));
        var budget2 = new Budget("Budget 2", new Money(300, "USD"), _testCategory.Id, DateTime.Today.AddDays(-5), DateTime.Today.AddDays(25));
        _budgetRepository.Add(budget1);
        _budgetRepository.Add(budget2);

        _transactionRepository.Add(new Expense("Expense 1", new Money(200, "USD"), DateTime.Today, _testCategory.Id));

        // Act
        var report = _budgetService.GetBudgetUtilizationReport();

        // Assert
        Assert.That(report.TotalBudgeted, Is.EqualTo(800));
        // Both budgets share the same category, so the expense is counted for each budget
        // resulting in TotalSpent = 400 (200 * 2 budgets)
        Assert.That(report.TotalSpent, Is.EqualTo(400));
        Assert.That(report.TotalRemaining, Is.EqualTo(400));
        Assert.That(report.BudgetCount, Is.EqualTo(2));
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up is automatic with in-memory repositories
    }
}

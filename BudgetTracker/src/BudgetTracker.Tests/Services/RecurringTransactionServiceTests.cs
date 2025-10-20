using BudgetTracker.Core.Services;
using BudgetTracker.Data;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Tests.Services;

/// <summary>
/// NUnit tests for RecurringTransactionService
/// Phase 6.19: NUnit Test Writing (Required)
/// Tests recurring transaction feature and LINQ queries
/// </summary>
[TestFixture]
public class RecurringTransactionServiceTests
{
    private InMemoryRepository<RecurringTransaction> _recurringRepository = null!;
    private InMemoryRepository<Transaction> _transactionRepository = null!;
    private InMemoryRepository<Category> _categoryRepository = null!;
    private RecurringTransactionService _service = null!;
    private Category _testCategory = null!;

    [SetUp]
    public void Setup()
    {
        // Initialize repositories
        _recurringRepository = new InMemoryRepository<RecurringTransaction>();
        _transactionRepository = new InMemoryRepository<Transaction>();
        _categoryRepository = new InMemoryRepository<Category>();

        // Create test category
        _testCategory = new Category { Id = 1, Name = "Salary", Description = "Income from salary" };
        _categoryRepository.Add(_testCategory);

        // Initialize service
        _service = new RecurringTransactionService(
            _recurringRepository,
            _transactionRepository,
            _categoryRepository
        );
    }

    [Test]
    public void CreateRecurring_WithValidData_CreatesSuccessfully()
    {
        // Act
        var recurring = _service.CreateRecurring(
            "Monthly Salary",
            "Monthly salary payment",
            5000,
            "USD",
            _testCategory.Id,
            isIncome: true,
            DateTime.Today,
            Frequency.Monthly
        );

        // Assert
        Assert.That(recurring, Is.Not.Null);
        Assert.That(recurring.Name, Is.EqualTo("Monthly Salary"));
        Assert.That(recurring.Amount.Amount, Is.EqualTo(5000));
        Assert.That(recurring.RecurrenceFrequency, Is.EqualTo(Frequency.Monthly));
        Assert.That(recurring.IsIncome, Is.True);
        Assert.That(recurring.IsActive, Is.True);
        Assert.That(_recurringRepository.GetAll().Count(), Is.EqualTo(1));
    }

    [Test]
    public void CreateRecurring_WithEndDate_SetsEndDateCorrectly()
    {
        // Arrange
        var endDate = DateTime.Today.AddYears(1);

        // Act
        var recurring = _service.CreateRecurring(
            "Temporary Subscription",
            "Annual subscription",
            9.99m,
            "USD",
            _testCategory.Id,
            isIncome: false,
            DateTime.Today,
            Frequency.Monthly,
            endDate
        );

        // Assert
        Assert.That(recurring.RecurrenceEndDate, Is.EqualTo(endDate));
    }

    [Test]
    public void ProcessRecurringTransactions_GeneratesTransactionsWhenDue()
    {
        // Arrange
        var recurring = _service.CreateRecurring(
            "Daily Income",
            "Test daily income",
            100,
            "USD",
            _testCategory.Id,
            isIncome: true,
            DateTime.Today.AddDays(-2),
            Frequency.Daily
        );

        // Act
        var generatedTransactions = _service.ProcessRecurringTransactions(DateTime.Today);

        // Assert
        Assert.That(generatedTransactions, Is.Not.Empty);
        Assert.That(_transactionRepository.GetAll().Count(), Is.GreaterThan(0));
    }

    [Test]
    public void ProcessRecurringTransactions_DoesNotGenerateFutureTransactions()
    {
        // Arrange
        var recurring = _service.CreateRecurring(
            "Future Salary",
            "Future salary payment",
            5000,
            "USD",
            _testCategory.Id,
            isIncome: true,
            DateTime.Today.AddDays(10),
            Frequency.Monthly
        );

        // Act
        var generatedTransactions = _service.ProcessRecurringTransactions(DateTime.Today);

        // Assert
        Assert.That(generatedTransactions, Is.Empty);
        Assert.That(_transactionRepository.GetAll().Count(), Is.EqualTo(0));
    }

    [Test]
    public void ProcessRecurringTransactions_RespectsEndDate()
    {
        // Arrange
        var recurring = _service.CreateRecurring(
            "Past Subscription",
            "Expired subscription",
            10,
            "USD",
            _testCategory.Id,
            isIncome: false,
            DateTime.Today.AddDays(-60),
            Frequency.Monthly,
            DateTime.Today.AddDays(-30) // Ended 30 days ago
        );

        // Act
        var generatedTransactions = _service.ProcessRecurringTransactions(DateTime.Today);

        // Assert
        Assert.That(generatedTransactions, Is.Empty);
    }

    [Test]
    public void ProcessRecurringTransactions_DoesNotGenerateForInactiveRecurring()
    {
        // Arrange
        var recurring = _service.CreateRecurring(
            "Stopped Salary",
            "Stopped salary",
            5000,
            "USD",
            _testCategory.Id,
            isIncome: true,
            DateTime.Today.AddDays(-2),
            Frequency.Daily
        );
        _service.StopRecurringTransaction(recurring.Id);

        // Act
        var generatedTransactions = _service.ProcessRecurringTransactions(DateTime.Today);

        // Assert
        Assert.That(generatedTransactions, Is.Empty);
    }

    [Test]
    public void GetActiveRecurringTransactions_ReturnsOnlyActive()
    {
        // Arrange
        var active = _service.CreateRecurring("Active", "Active recurring", 100, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Monthly);
        var inactive = _service.CreateRecurring("Inactive", "Inactive recurring", 200, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Monthly);
        _service.StopRecurringTransaction(inactive.Id);

        // Act
        var activeTransactions = _service.GetActiveRecurringTransactions();

        // Assert
        Assert.That(activeTransactions.Count, Is.EqualTo(1));
        Assert.That(activeTransactions[0].Name, Is.EqualTo("Active"));
    }

    [Test]
    public void GetAllRecurringTransactions_ReturnsAllTransactions()
    {
        // Arrange
        var recurring1 = _service.CreateRecurring("Recurring 1", "First", 100, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Monthly);
        var recurring2 = _service.CreateRecurring("Recurring 2", "Second", 200, "USD", _testCategory.Id, false, DateTime.Today, Frequency.Weekly);
        _service.StopRecurringTransaction(recurring2.Id);

        // Act
        var allTransactions = _service.GetAllRecurringTransactions();

        // Assert
        Assert.That(allTransactions.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetRecurringTransactionsByFrequency_FiltersCorrectly()
    {
        // Arrange
        _service.CreateRecurring("Monthly 1", "Monthly", 100, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Monthly);
        _service.CreateRecurring("Weekly 1", "Weekly", 50, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Weekly);
        _service.CreateRecurring("Monthly 2", "Monthly", 150, "USD", _testCategory.Id, false, DateTime.Today, Frequency.Monthly);

        // Act
        var monthlyTransactions = _service.GetRecurringTransactionsByFrequency(Frequency.Monthly);

        // Assert
        Assert.That(monthlyTransactions.Count, Is.EqualTo(2));
        Assert.That(monthlyTransactions.All(r => r.RecurrenceFrequency == Frequency.Monthly), Is.True);
    }

    [Test]
    public void GetRecurringTransactionsByType_FiltersIncomeCorrectly()
    {
        // Arrange
        _service.CreateRecurring("Income 1", "Income", 100, "USD", _testCategory.Id, isIncome: true, DateTime.Today, Frequency.Monthly);
        _service.CreateRecurring("Expense 1", "Expense", 50, "USD", _testCategory.Id, isIncome: false, DateTime.Today, Frequency.Weekly);
        _service.CreateRecurring("Income 2", "Income", 200, "USD", _testCategory.Id, isIncome: true, DateTime.Today, Frequency.Monthly);

        // Act
        var incomeTransactions = _service.GetRecurringTransactionsByType(isIncome: true);

        // Assert
        Assert.That(incomeTransactions.Count, Is.EqualTo(2));
        Assert.That(incomeTransactions.All(r => r.IsIncome), Is.True);
    }

    [Test]
    public void GetRecurringTransactionsByType_FiltersExpenseCorrectly()
    {
        // Arrange
        _service.CreateRecurring("Income 1", "Income", 100, "USD", _testCategory.Id, isIncome: true, DateTime.Today, Frequency.Monthly);
        _service.CreateRecurring("Expense 1", "Expense", 50, "USD", _testCategory.Id, isIncome: false, DateTime.Today, Frequency.Weekly);
        _service.CreateRecurring("Expense 2", "Expense", 75, "USD", _testCategory.Id, isIncome: false, DateTime.Today, Frequency.Monthly);

        // Act
        var expenseTransactions = _service.GetRecurringTransactionsByType(isIncome: false);

        // Assert
        Assert.That(expenseTransactions.Count, Is.EqualTo(2));
        Assert.That(expenseTransactions.All(r => !r.IsIncome), Is.True);
    }

    [Test]
    public void GetRecurringTransactionsByCategory_FiltersCorrectly()
    {
        // Arrange
        var category2 = new Category { Id = 2, Name = "Rent", Description = "Rent expenses" };
        _categoryRepository.Add(category2);

        _service.CreateRecurring("Salary", "Salary", 5000, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Monthly);
        _service.CreateRecurring("Rent", "Rent", 1000, "USD", category2.Id, false, DateTime.Today, Frequency.Monthly);
        _service.CreateRecurring("Bonus", "Bonus", 1000, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Quarterly);

        // Act
        var salaryCategory = _service.GetRecurringTransactionsByCategory(_testCategory.Id);

        // Assert
        Assert.That(salaryCategory.Count, Is.EqualTo(2));
        Assert.That(salaryCategory.All(r => r.CategoryId == _testCategory.Id), Is.True);
    }

    [Test]
    public void StopRecurringTransaction_StopsSuccessfully()
    {
        // Arrange
        var recurring = _service.CreateRecurring("Test", "Test", 100, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Monthly);

        // Act
        _service.StopRecurringTransaction(recurring.Id);

        // Assert
        var updated = _recurringRepository.GetById(recurring.Id);
        Assert.That(updated.IsActive, Is.False);
    }

    [Test]
    public void ResumeRecurringTransaction_ResumesSuccessfully()
    {
        // Arrange
        var recurring = _service.CreateRecurring("Test", "Test", 100, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Monthly);
        _service.StopRecurringTransaction(recurring.Id);

        // Act
        _service.ResumeRecurringTransaction(recurring.Id);

        // Assert
        var updated = _recurringRepository.GetById(recurring.Id);
        Assert.That(updated.IsActive, Is.True);
    }

    [Test]
    public void DeleteRecurringTransaction_DeletesSuccessfully()
    {
        // Arrange
        var recurring = _service.CreateRecurring("Test", "Test", 100, "USD", _testCategory.Id, true, DateTime.Today, Frequency.Monthly);

        // Act
        _service.DeleteRecurringTransaction(recurring.Id);

        // Assert
        Assert.That(_recurringRepository.GetAll().Count(), Is.EqualTo(0));
    }

    [Test]
    public void GetUpcomingOccurrences_ReturnsCorrectDates()
    {
        // Arrange
        var recurring = _service.CreateRecurring(
            "Monthly Salary",
            "Salary",
            5000,
            "USD",
            _testCategory.Id,
            true,
            DateTime.Today,
            Frequency.Monthly
        );

        // Act
        var occurrences = _service.GetUpcomingOccurrences(recurring.Id, count: 3);

        // Assert
        Assert.That(occurrences.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(occurrences.Count, Is.LessThanOrEqualTo(3));
    }

    [Test]
    public void GetUpcomingOccurrences_RespectsEndDate()
    {
        // Arrange
        var endDate = DateTime.Today.AddMonths(2);
        var recurring = _service.CreateRecurring(
            "Short Term",
            "Short term recurring",
            100,
            "USD",
            _testCategory.Id,
            true,
            DateTime.Today,
            Frequency.Monthly,
            endDate
        );

        // Act
        var occurrences = _service.GetUpcomingOccurrences(recurring.Id, count: 10);

        // Assert
        Assert.That(occurrences.Count, Is.LessThanOrEqualTo(3));
    }

    [Test]
    public void GetRecurringSummary_CalculatesCorrectTotals()
    {
        // Arrange
        _service.CreateRecurring("Monthly Income", "Income", 5000, "USD", _testCategory.Id, isIncome: true, DateTime.Today, Frequency.Monthly);
        _service.CreateRecurring("Weekly Expense", "Expense", 100, "USD", _testCategory.Id, isIncome: false, DateTime.Today, Frequency.Weekly);
        _service.CreateRecurring("Inactive", "Inactive", 500, "USD", _testCategory.Id, isIncome: true, DateTime.Today, Frequency.Monthly);
        _service.StopRecurringTransaction(_recurringRepository.GetAll().Last().Id);

        // Act
        var summary = _service.GetRecurringSummary();

        // Assert
        Assert.That(summary.TotalRecurring, Is.EqualTo(3));
        Assert.That(summary.ActiveRecurring, Is.EqualTo(2));
        Assert.That(summary.InactiveRecurring, Is.EqualTo(1));
        Assert.That(summary.IncomeCount, Is.EqualTo(1));
        Assert.That(summary.ExpenseCount, Is.EqualTo(1));
        Assert.That(summary.MonthlyIncome, Is.GreaterThan(0));
        Assert.That(summary.MonthlyExpense, Is.GreaterThan(0));
    }

    [Test]
    public void GenerateTransactionsForPeriod_GeneratesCorrectTransactions()
    {
        // Arrange
        var recurring = _service.CreateRecurring(
            "Daily Income",
            "Daily test income",
            100,
            "USD",
            _testCategory.Id,
            isIncome: true,
            DateTime.Today.AddDays(-5),
            Frequency.Daily
        );

        // Act
        var transactions = _service.GenerateTransactionsForPeriod(
            recurring.Id,
            DateTime.Today.AddDays(-3),
            DateTime.Today
        );

        // Assert
        Assert.That(transactions.Count, Is.GreaterThan(0));
        Assert.That(transactions.All(t => t is Income), Is.True);
    }

    [Test]
    public void GenerateTransactionsForPeriod_WithInactiveRecurring_ReturnsEmpty()
    {
        // Arrange
        var recurring = _service.CreateRecurring(
            "Inactive",
            "Inactive recurring",
            100,
            "USD",
            _testCategory.Id,
            isIncome: true,
            DateTime.Today.AddDays(-5),
            Frequency.Daily
        );
        _service.StopRecurringTransaction(recurring.Id);

        // Act
        var transactions = _service.GenerateTransactionsForPeriod(
            recurring.Id,
            DateTime.Today.AddDays(-3),
            DateTime.Today
        );

        // Assert
        Assert.That(transactions, Is.Empty);
    }

    [Test]
    public void UpdateRecurringTransaction_UpdatesSuccessfully()
    {
        // Arrange
        var recurring = _service.CreateRecurring(
            "Original",
            "Original description",
            100,
            "USD",
            _testCategory.Id,
            true,
            DateTime.Today,
            Frequency.Monthly
        );

        // Act
        recurring.Name = "Updated Name";
        recurring.Amount = new Money(200, "USD");
        _service.UpdateRecurringTransaction(recurring);

        // Assert
        var updated = _recurringRepository.GetById(recurring.Id);
        Assert.That(updated.Name, Is.EqualTo("Updated Name"));
        Assert.That(updated.Amount.Amount, Is.EqualTo(200));
    }

    [Test]
    public void ProcessRecurringTransactions_GeneratesCorrectTransactionType()
    {
        // Arrange
        var incomeRecurring = _service.CreateRecurring(
            "Income",
            "Test income",
            100,
            "USD",
            _testCategory.Id,
            isIncome: true,
            DateTime.Today.AddDays(-1),
            Frequency.Daily
        );

        var expenseRecurring = _service.CreateRecurring(
            "Expense",
            "Test expense",
            50,
            "USD",
            _testCategory.Id,
            isIncome: false,
            DateTime.Today.AddDays(-1),
            Frequency.Daily
        );

        // Act
        var transactions = _service.ProcessRecurringTransactions(DateTime.Today);

        // Assert
        Assert.That(transactions.Count, Is.EqualTo(2));
        Assert.That(transactions.Any(t => t is Income), Is.True);
        Assert.That(transactions.Any(t => t is Expense), Is.True);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up is automatic with in-memory repositories
    }
}
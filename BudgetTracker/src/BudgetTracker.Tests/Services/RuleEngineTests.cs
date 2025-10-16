using BudgetTracker.Core.Interfaces.Rules;
using BudgetTracker.Core.Services;
using BudgetTracker.Data;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Tests.Services;

/// <summary>
/// NUnit tests for RuleEngine
/// Phase 6.19: NUnit Test Writing
/// Tests Strategy Pattern and business rules
/// </summary>
[TestFixture]
public class RuleEngineTests
{
    private RuleEngine _ruleEngine = null!;
    private InMemoryRepository<Budget> _budgetRepository = null!;
    private InMemoryRepository<Transaction> _transactionRepository = null!;
    private Category _testCategory = null!;

    [SetUp]
    public void Setup()
    {
        // Initialize repositories
        _budgetRepository = new InMemoryRepository<Budget>();
        _transactionRepository = new InMemoryRepository<Transaction>();

        // Create test category
        _testCategory = new Category { Id = 1, Name = "Food", Description = "Food expenses" };

        // Initialize rule engine with repositories
        _ruleEngine = new RuleEngine(_budgetRepository, _transactionRepository);
    }

    [Test]
    public void EvaluateTransaction_WithValidIncome_ReturnsSuccess()
    {
        // Arrange
        var transaction = new Income("Salary", new Money(5000, "USD"), DateTime.Today, 1);

        // Act
        var result = _ruleEngine.EvaluateTransaction(transaction);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.HasErrors, Is.False);
    }

    [Test]
    public void EvaluateTransaction_WithValidExpense_ReturnsSuccess()
    {
        // Arrange
        var transaction = new Expense("Groceries", new Money(150, "USD"), DateTime.Today, 1);

        // Act
        var result = _ruleEngine.EvaluateTransaction(transaction);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.HasErrors, Is.False);
    }

    [Test]
    public void EvaluateTransaction_WithEmptyDescription_VerifiesRuleExists()
    {
        // Arrange - Empty description throws in constructor due to validation
        // This test verifies the DescriptionRequiredRule exists in the engine

        // Act
        var rules = _ruleEngine.GetTransactionRules();

        // Assert
        Assert.That(rules.Any(r => r.RuleName == "DescriptionRequired"), Is.True);

        // Verify the constructor validates description (should throw)
        Assert.Throws<ArgumentException>(() =>
        {
            new Income("", new Money(100, "USD"), DateTime.Today, 1);
        });
    }

    [Test]
    public void EvaluateTransaction_WithFutureDate_ReturnsWarning()
    {
        // Arrange
        var futureTransaction = new Income("Future Salary", new Money(5000, "USD"), DateTime.Today.AddDays(10), 1);

        // Act
        var result = _ruleEngine.EvaluateTransaction(futureTransaction);

        // Assert
        Assert.That(result.HasWarnings, Is.True);
        Assert.That(result.Warnings.Any(w => w.Severity == RuleSeverity.Warning), Is.True);
    }

    [Test]
    public void EvaluateBudget_WithValidBudget_ReturnsSuccess()
    {
        // Arrange
        var budget = new Budget(
            "Monthly Food Budget",
            new Money(500, "USD"),
            1,
            DateTime.Today,
            DateTime.Today.AddMonths(1)
        );
        _budgetRepository.Add(budget);

        // Act
        var result = _ruleEngine.EvaluateBudget(budget);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void EvaluateBudget_WithInvalidDateRange_VerifiesRuleExists()
    {
        // Arrange - Invalid date range throws in constructor due to validation
        // This test verifies the ValidDateRange rule exists in the engine

        // Act
        var rules = _ruleEngine.GetBudgetRules();

        // Assert
        Assert.That(rules.Any(r => r.RuleName == "ValidDateRange"), Is.True);

        // Verify the constructor validates date range (should throw)
        Assert.Throws<ArgumentException>(() =>
        {
            new Budget(
                "Invalid Budget",
                new Money(500, "USD"),
                1,
                DateTime.Today,
                DateTime.Today.AddDays(-1) // End date before start date
            );
        });
    }

    [Test]
    public void EvaluateBudget_WithExceededBudget_ReturnsCriticalError()
    {
        // Arrange
        var budget = new Budget(
            "Food Budget",
            new Money(100, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(20)
        );
        _budgetRepository.Add(budget);

        // Add expenses that exceed budget
        _transactionRepository.Add(new Expense("Large Purchase", new Money(150, "USD"), DateTime.Today, _testCategory.Id));

        // Act
        var result = _ruleEngine.EvaluateBudget(budget);

        // Assert
        Assert.That(result.HasErrors, Is.True);
        var criticalErrors = result.Errors.Where(e => e.Severity == RuleSeverity.Critical);
        Assert.That(criticalErrors, Is.Not.Empty);
    }

    [Test]
    public void EvaluateBudget_AtWarningThreshold_ReturnsWarning()
    {
        // Arrange
        var budget = new Budget(
            "Food Budget",
            new Money(100, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(20)
        );
        _budgetRepository.Add(budget);

        // Add expenses at 85% of budget (warning threshold is 80%)
        _transactionRepository.Add(new Expense("Purchase", new Money(85, "USD"), DateTime.Today, _testCategory.Id));

        // Act
        var result = _ruleEngine.EvaluateBudget(budget);

        // Assert
        Assert.That(result.HasWarnings, Is.True);
        Assert.That(result.Warnings.Any(w => w.Severity == RuleSeverity.Warning), Is.True);
    }

    [Test]
    public void EvaluateBudget_WithNormalSpending_ReturnsNoWarnings()
    {
        // Arrange
        var budget = new Budget(
            "Food Budget",
            new Money(100, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(20)
        );
        _budgetRepository.Add(budget);

        // Add expenses at 50% of budget
        _transactionRepository.Add(new Expense("Purchase", new Money(50, "USD"), DateTime.Today, _testCategory.Id));

        // Act
        var result = _ruleEngine.EvaluateBudget(budget);

        // Assert
        Assert.That(result.IsValid, Is.True);
        // Should have some results but no critical warnings about exceeding threshold
        var criticalWarnings = result.Warnings.Where(w => w.Message.Contains("approaching") || w.Message.Contains("exceeded"));
        Assert.That(criticalWarnings, Is.Empty);
    }

    [Test]
    public void AddTransactionRule_AddsRuleSuccessfully()
    {
        // Arrange
        var customRule = new CustomTestTransactionRule();

        // Act
        _ruleEngine.AddTransactionRule(customRule);
        var rules = _ruleEngine.GetTransactionRules();

        // Assert
        Assert.That(rules, Does.Contain(customRule));
        Assert.That(rules.Any(r => r.RuleName == "CustomTestRule"), Is.True);
    }

    [Test]
    public void AddBudgetRule_AddsRuleSuccessfully()
    {
        // Arrange
        var customRule = new CustomTestBudgetRule();

        // Act
        _ruleEngine.AddBudgetRule(customRule);
        var rules = _ruleEngine.GetBudgetRules();

        // Assert
        Assert.That(rules, Does.Contain(customRule));
        Assert.That(rules.Any(r => r.RuleName == "CustomTestBudgetRule"), Is.True);
    }

    [Test]
    public void RemoveTransactionRule_RemovesRuleSuccessfully()
    {
        // Arrange
        var customRule = new CustomTestTransactionRule();
        _ruleEngine.AddTransactionRule(customRule);

        // Act
        _ruleEngine.RemoveTransactionRule("CustomTestRule");
        var rules = _ruleEngine.GetTransactionRules();

        // Assert
        Assert.That(rules, Does.Not.Contain(customRule));
        Assert.That(rules.Any(r => r.RuleName == "CustomTestRule"), Is.False);
    }

    [Test]
    public void RemoveBudgetRule_RemovesRuleSuccessfully()
    {
        // Arrange
        var customRule = new CustomTestBudgetRule();
        _ruleEngine.AddBudgetRule(customRule);

        // Act
        _ruleEngine.RemoveBudgetRule("CustomTestBudgetRule");
        var rules = _ruleEngine.GetBudgetRules();

        // Assert
        Assert.That(rules, Does.Not.Contain(customRule));
        Assert.That(rules.Any(r => r.RuleName == "CustomTestBudgetRule"), Is.False);
    }

    [Test]
    public void EvaluateTransactionAgainstBudgets_WithExceedingExpense_ReturnsWarning()
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

        // Add existing expense
        _transactionRepository.Add(new Expense("Existing", new Money(60, "USD"), DateTime.Today.AddDays(-2), _testCategory.Id));

        // Create new expense that would exceed budget
        var newExpense = new Expense("New Purchase", new Money(50, "USD"), DateTime.Today, _testCategory.Id);

        // Act
        var result = _ruleEngine.EvaluateTransactionAgainstBudgets(newExpense);

        // Assert
        Assert.That(result.Results, Is.Not.Empty);
        Assert.That(result.HasWarnings, Is.True);
    }

    [Test]
    public void EvaluateTransactionAgainstBudgets_WithNonExceedingExpense_Passes()
    {
        // Arrange
        var budget = new Budget(
            "Monthly Food Budget",
            new Money(500, "USD"),
            _testCategory.Id,
            DateTime.Today.AddDays(-5),
            DateTime.Today.AddDays(25)
        );
        _budgetRepository.Add(budget);

        // Create expense within budget
        var newExpense = new Expense("Small Purchase", new Money(50, "USD"), DateTime.Today, _testCategory.Id);

        // Act
        var result = _ruleEngine.EvaluateTransactionAgainstBudgets(newExpense);

        // Assert
        Assert.That(result.Results, Is.Not.Empty);
        var passResults = result.Results.Where(r => r.IsSuccess);
        Assert.That(passResults, Is.Not.Empty);
    }

    [Test]
    public void GetTransactionRules_ReturnsDefaultRules()
    {
        // Act
        var rules = _ruleEngine.GetTransactionRules();

        // Assert
        Assert.That(rules, Is.Not.Empty);
        Assert.That(rules.Any(r => r.RuleName == "PositiveAmount"), Is.True);
        Assert.That(rules.Any(r => r.RuleName == "FutureDateValidation"), Is.True);
        Assert.That(rules.Any(r => r.RuleName == "DescriptionRequired"), Is.True);
    }

    [Test]
    public void GetBudgetRules_ReturnsDefaultRules()
    {
        // Act
        var rules = _ruleEngine.GetBudgetRules();

        // Assert
        Assert.That(rules, Is.Not.Empty);
        Assert.That(rules.Any(r => r.RuleName == "BudgetExceeded"), Is.True);
        Assert.That(rules.Any(r => r.RuleName == "BudgetWarning"), Is.True);
        Assert.That(rules.Any(r => r.RuleName == "ValidDateRange"), Is.True);
    }

    [Test]
    public void RuleResult_Success_CreatesSuccessResult()
    {
        // Act
        var result = RuleResult.Success("Test passed");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("Test passed"));
        Assert.That(result.Severity, Is.EqualTo(RuleSeverity.Info));
    }

    [Test]
    public void RuleResult_Failure_CreatesFailureResult()
    {
        // Act
        var result = RuleResult.Failure("Test failed", RuleSeverity.Error);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("Test failed"));
        Assert.That(result.Severity, Is.EqualTo(RuleSeverity.Error));
    }

    [Test]
    public void RuleResult_Warning_CreatesWarningResult()
    {
        // Act
        var result = RuleResult.Warning("Test warning");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("Test warning"));
        Assert.That(result.Severity, Is.EqualTo(RuleSeverity.Warning));
    }

    // Helper class for testing custom transaction rules
    private class CustomTestTransactionRule : RuleBase<Transaction>
    {
        public CustomTestTransactionRule()
            : base("CustomTestRule", "Custom test rule for transactions", RuleSeverity.Info)
        {
        }

        protected override RuleResult ExecuteRule(Transaction entity)
        {
            return Pass("Custom rule passed");
        }
    }

    // Helper class for testing custom budget rules
    private class CustomTestBudgetRule : RuleBase<Budget>
    {
        public CustomTestBudgetRule()
            : base("CustomTestBudgetRule", "Custom test rule for budgets", RuleSeverity.Info)
        {
        }

        protected override RuleResult ExecuteRule(Budget entity)
        {
            return Pass("Custom budget rule passed");
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up
    }
}

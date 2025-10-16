using BudgetTracker.Core.Interfaces;
using BudgetTracker.Core.Interfaces.Rules;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Core.Services;

/// <summary>
/// Rule engine for executing business rules
/// Phase 3.11: RuleEngine Implementation
/// Implements Strategy Pattern - rules can be added/removed at runtime
/// </summary>
public class RuleEngine
{
    private readonly List<IRule<Transaction>> _transactionRules;
    private readonly List<IRule<Budget>> _budgetRules;
    private readonly IRepository<Budget> _budgetRepository;
    private readonly IRepository<Transaction> _transactionRepository;

    public RuleEngine(IRepository<Budget> budgetRepository, IRepository<Transaction> transactionRepository)
    {
        _transactionRules = new List<IRule<Transaction>>();
        _budgetRules = new List<IRule<Budget>>();
        _budgetRepository = budgetRepository;
        _transactionRepository = transactionRepository;

        // Register default rules
        RegisterDefaultRules();
    }

    /// <summary>
    /// Register default business rules
    /// </summary>
    private void RegisterDefaultRules()
    {
        // Transaction rules
        AddTransactionRule(new PositiveAmountRule());
        AddTransactionRule(new FutureDateValidationRule());
        AddTransactionRule(new DescriptionRequiredRule());

        // Budget rules
        AddBudgetRule(new BudgetExceededRule(_transactionRepository));
        AddBudgetRule(new BudgetWarningRule(_transactionRepository));
        AddBudgetRule(new ValidDateRangeRule());
    }

    /// <summary>
    /// Add a transaction rule to the engine
    /// </summary>
    public void AddTransactionRule(IRule<Transaction> rule)
    {
        _transactionRules.Add(rule);
    }

    /// <summary>
    /// Add a budget rule to the engine
    /// </summary>
    public void AddBudgetRule(IRule<Budget> rule)
    {
        _budgetRules.Add(rule);
    }

    /// <summary>
    /// Remove a rule by name
    /// </summary>
    public void RemoveTransactionRule(string ruleName)
    {
        _transactionRules.RemoveAll(r => r.RuleName == ruleName);
    }

    /// <summary>
    /// Remove a budget rule by name
    /// </summary>
    public void RemoveBudgetRule(string ruleName)
    {
        _budgetRules.RemoveAll(r => r.RuleName == ruleName);
    }

    /// <summary>
    /// Evaluate all transaction rules
    /// </summary>
    public RuleEngineResult<Transaction> EvaluateTransaction(Transaction transaction)
    {
        var result = new RuleEngineResult<Transaction> { Entity = transaction };

        foreach (var rule in _transactionRules.Where(r => r.IsEnabled))
        {
            var ruleResult = rule.Evaluate(transaction);
            result.Results.Add(ruleResult);

            if (!ruleResult.IsSuccess)
            {
                result.IsValid = false;
            }
        }

        return result;
    }

    /// <summary>
    /// Evaluate all budget rules
    /// </summary>
    public RuleEngineResult<Budget> EvaluateBudget(Budget budget)
    {
        var result = new RuleEngineResult<Budget> { Entity = budget };

        foreach (var rule in _budgetRules.Where(r => r.IsEnabled))
        {
            var ruleResult = rule.Evaluate(budget);
            result.Results.Add(ruleResult);

            if (!ruleResult.IsSuccess && ruleResult.Severity >= RuleSeverity.Error)
            {
                result.IsValid = false;
            }
        }

        return result;
    }

    /// <summary>
    /// Evaluate a transaction against budget rules before saving
    /// </summary>
    public RuleEngineResult<Transaction> EvaluateTransactionAgainstBudgets(Transaction transaction)
    {
        var result = new RuleEngineResult<Transaction> { Entity = transaction };

        // Find applicable budgets
        var applicableBudgets = _budgetRepository.GetAll()
            .Where(b => b.CategoryId == transaction.CategoryId &&
                       b.IsCurrentlyActive() &&
                       transaction.Date >= b.StartDate &&
                       transaction.Date <= b.EndDate)
            .ToList();

        foreach (var budget in applicableBudgets)
        {
            var budgetRule = new BudgetExceededByTransactionRule(_transactionRepository, budget);
            var ruleResult = budgetRule.Evaluate(transaction);
            result.Results.Add(ruleResult);
        }

        return result;
    }

    /// <summary>
    /// Get all registered transaction rules
    /// </summary>
    public IReadOnlyList<IRule<Transaction>> GetTransactionRules() => _transactionRules.AsReadOnly();

    /// <summary>
    /// Get all registered budget rules
    /// </summary>
    public IReadOnlyList<IRule<Budget>> GetBudgetRules() => _budgetRules.AsReadOnly();
}

/// <summary>
/// Result of rule engine evaluation
/// </summary>
public class RuleEngineResult<T> where T : class
{
    public T? Entity { get; set; }
    public bool IsValid { get; set; } = true;
    public List<RuleResult> Results { get; set; } = new List<RuleResult>();

    public List<RuleResult> Errors => Results.Where(r => !r.IsSuccess).ToList();
    public List<RuleResult> Warnings => Results.Where(r => r.IsSuccess && r.Severity == RuleSeverity.Warning).ToList();
    public bool HasErrors => Errors.Any();
    public bool HasWarnings => Warnings.Any();

    public string GetErrorMessage()
    {
        return string.Join("\n", Errors.Select(e => e.Message));
    }

    public string GetWarningMessage()
    {
        return string.Join("\n", Warnings.Select(w => w.Message));
    }
}

// ============================================================================
// TRANSACTION RULES
// ============================================================================

/// <summary>
/// Rule: Transaction amount must be positive
/// </summary>
public class PositiveAmountRule : RuleBase<Transaction>
{
    public PositiveAmountRule()
        : base("PositiveAmount", "Transaction amount must be greater than zero", RuleSeverity.Error)
    {
    }

    protected override RuleResult ExecuteRule(Transaction entity)
    {
        if (entity.Amount.Amount <= 0)
        {
            return Fail($"Amount must be greater than zero. Current: {entity.Amount.Amount}");
        }

        return Pass();
    }
}

/// <summary>
/// Rule: Transaction date cannot be in the future
/// </summary>
public class FutureDateValidationRule : RuleBase<Transaction>
{
    public FutureDateValidationRule()
        : base("FutureDateValidation", "Transaction date cannot be in the future", RuleSeverity.Warning)
    {
    }

    protected override RuleResult ExecuteRule(Transaction entity)
    {
        if (entity.Date > DateTime.Today)
        {
            return Warn($"Transaction date is in the future: {entity.Date:yyyy-MM-dd}");
        }

        return Pass();
    }
}

/// <summary>
/// Rule: Transaction must have a description
/// </summary>
public class DescriptionRequiredRule : RuleBase<Transaction>
{
    public DescriptionRequiredRule()
        : base("DescriptionRequired", "Transaction must have a description", RuleSeverity.Error)
    {
    }

    protected override RuleResult ExecuteRule(Transaction entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Description))
        {
            return Fail("Description is required");
        }

        return Pass();
    }
}

// ============================================================================
// BUDGET RULES
// ============================================================================

/// <summary>
/// Rule: Check if budget is exceeded
/// </summary>
public class BudgetExceededRule : RuleBase<Budget>
{
    private readonly IRepository<Transaction> _transactionRepository;

    public BudgetExceededRule(IRepository<Transaction> transactionRepository)
        : base("BudgetExceeded", "Check if budget has been exceeded", RuleSeverity.Critical)
    {
        _transactionRepository = transactionRepository;
    }

    protected override RuleResult ExecuteRule(Budget entity)
    {
        var spending = _transactionRepository.GetAll()
            .Where(t => t is Expense)
            .Where(t => t.CategoryId == entity.CategoryId)
            .Where(t => t.Date >= entity.StartDate && t.Date <= entity.EndDate)
            .Sum(t => t.Amount.Amount);

        if (spending > entity.Amount.Amount)
        {
            var overAmount = spending - entity.Amount.Amount;
            return Fail($"Budget exceeded by ${overAmount:N2}. Spent: ${spending:N2} / Budget: ${entity.Amount.Amount:N2}");
        }

        return Pass($"Budget is within limits. Spent: ${spending:N2} / Budget: ${entity.Amount.Amount:N2}");
    }
}

/// <summary>
/// Rule: Check if budget is approaching limit (warning threshold)
/// </summary>
public class BudgetWarningRule : RuleBase<Budget>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private const decimal WarningThreshold = 0.80m; // 80%

    public BudgetWarningRule(IRepository<Transaction> transactionRepository)
        : base("BudgetWarning", "Check if budget is approaching limit", RuleSeverity.Warning)
    {
        _transactionRepository = transactionRepository;
    }

    protected override RuleResult ExecuteRule(Budget entity)
    {
        var spending = _transactionRepository.GetAll()
            .Where(t => t is Expense)
            .Where(t => t.CategoryId == entity.CategoryId)
            .Where(t => t.Date >= entity.StartDate && t.Date <= entity.EndDate)
            .Sum(t => t.Amount.Amount);

        var percentageUsed = entity.Amount.Amount > 0 ? spending / entity.Amount.Amount : 0;

        if (percentageUsed >= WarningThreshold && spending <= entity.Amount.Amount)
        {
            var remaining = entity.Amount.Amount - spending;
            return Warn($"Budget is at {percentageUsed:P1}. ${remaining:N2} remaining.");
        }

        return Pass();
    }
}

/// <summary>
/// Rule: Budget must have valid date range
/// </summary>
public class ValidDateRangeRule : RuleBase<Budget>
{
    public ValidDateRangeRule()
        : base("ValidDateRange", "Budget end date must be after start date", RuleSeverity.Error)
    {
    }

    protected override RuleResult ExecuteRule(Budget entity)
    {
        if (entity.EndDate <= entity.StartDate)
        {
            return Fail($"End date ({entity.EndDate:yyyy-MM-dd}) must be after start date ({entity.StartDate:yyyy-MM-dd})");
        }

        return Pass();
    }
}

/// <summary>
/// Rule: Check if adding a transaction would exceed budget
/// </summary>
public class BudgetExceededByTransactionRule : RuleBase<Transaction>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly Budget _budget;

    public BudgetExceededByTransactionRule(IRepository<Transaction> transactionRepository, Budget budget)
        : base("BudgetExceededByTransaction",
               $"Check if transaction would exceed budget '{budget.Name}'",
               RuleSeverity.Warning)
    {
        _transactionRepository = transactionRepository;
        _budget = budget;
    }

    protected override RuleResult ExecuteRule(Transaction entity)
    {
        // Only apply to expenses
        if (entity is not Expense)
        {
            return Pass("Not an expense");
        }

        // Calculate current spending
        var currentSpending = _transactionRepository.GetAll()
            .Where(t => t is Expense)
            .Where(t => t.CategoryId == _budget.CategoryId)
            .Where(t => t.Date >= _budget.StartDate && t.Date <= _budget.EndDate)
            .Where(t => t.Id != entity.Id) // Exclude current transaction if editing
            .Sum(t => t.Amount.Amount);

        var projectedSpending = currentSpending + entity.Amount.Amount;

        if (projectedSpending > _budget.Amount.Amount)
        {
            var overAmount = projectedSpending - _budget.Amount.Amount;
            return Warn($"This transaction would exceed budget '{_budget.Name}' by ${overAmount:N2}");
        }

        var remaining = _budget.Amount.Amount - projectedSpending;
        return Pass($"Budget '{_budget.Name}' will have ${remaining:N2} remaining");
    }
}

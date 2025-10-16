namespace BudgetTracker.Core.Interfaces.Rules;

/// <summary>
/// Abstract base class for rules - provides common functionality
/// Demonstrates Template Method Pattern
/// </summary>
/// <typeparam name="T">The type of entity this rule validates</typeparam>
public abstract class RuleBase<T> : IRule<T> where T : class
{
    public string RuleName { get; protected set; }
    public string Description { get; protected set; }
    public RuleSeverity Severity { get; protected set; }
    public bool IsEnabled { get; set; }

    protected RuleBase(string ruleName, string description, RuleSeverity severity = RuleSeverity.Error)
    {
        RuleName = ruleName;
        Description = description;
        Severity = severity;
        IsEnabled = true;
    }

    /// <summary>
    /// Template method - defines the overall evaluation process
    /// </summary>
    public RuleResult Evaluate(T entity)
    {
        // If rule is disabled, return success
        if (!IsEnabled)
            return RuleResult.Success($"{RuleName} is disabled");

        // Validate input
        if (entity == null)
            return RuleResult.Failure("Entity cannot be null", RuleSeverity.Error);

        // Pre-evaluation hook (optional)
        var preResult = BeforeEvaluate(entity);
        if (preResult != null)
            return preResult;

        // Execute the actual rule logic (must be implemented by derived classes)
        var result = ExecuteRule(entity);

        // Post-evaluation hook (optional)
        AfterEvaluate(entity, result);

        return result;
    }

    /// <summary>
    /// Executes the actual rule logic - must be implemented by derived classes
    /// </summary>
    protected abstract RuleResult ExecuteRule(T entity);

    /// <summary>
    /// Hook method called before rule execution
    /// Override to add pre-validation logic
    /// </summary>
    protected virtual RuleResult? BeforeEvaluate(T entity)
    {
        return null;
    }

    /// <summary>
    /// Hook method called after rule execution
    /// Override to add logging, metrics, etc.
    /// </summary>
    protected virtual void AfterEvaluate(T entity, RuleResult result)
    {
        // Default: do nothing
    }

    /// <summary>
    /// Helper method to create a failure result with rule info
    /// </summary>
    protected RuleResult Fail(string message)
    {
        return RuleResult.Failure($"{RuleName}: {message}", Severity);
    }

    /// <summary>
    /// Helper method to create a success result with rule info
    /// </summary>
    protected RuleResult Pass(string message = "")
    {
        var msg = string.IsNullOrEmpty(message)
            ? $"{RuleName}: Passed"
            : $"{RuleName}: {message}";
        return RuleResult.Success(msg);
    }

    /// <summary>
    /// Helper method to create a warning result with rule info
    /// </summary>
    protected RuleResult Warn(string message)
    {
        return RuleResult.Warning($"{RuleName}: {message}");
    }
}

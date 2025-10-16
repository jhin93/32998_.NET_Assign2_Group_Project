namespace BudgetTracker.Core.Interfaces.Rules;

/// <summary>
/// Interface for business rules
/// Enables Strategy Pattern - different rules can be plugged in at runtime
/// </summary>
/// <typeparam name="T">The type of entity this rule validates</typeparam>
public interface IRule<T> where T : class
{
    /// <summary>
    /// Gets the name of the rule
    /// </summary>
    string RuleName { get; }

    /// <summary>
    /// Gets the description of what this rule checks
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the severity level of the rule
    /// </summary>
    RuleSeverity Severity { get; }

    /// <summary>
    /// Evaluates the rule against an entity
    /// </summary>
    /// <param name="entity">The entity to evaluate</param>
    /// <returns>Rule result with success/failure and message</returns>
    RuleResult Evaluate(T entity);

    /// <summary>
    /// Checks if the rule is enabled
    /// </summary>
    bool IsEnabled { get; set; }
}

/// <summary>
/// Severity levels for rules
/// </summary>
public enum RuleSeverity
{
    /// <summary>
    /// Informational message only
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning - should be reviewed but not blocking
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error - blocks the operation
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical - serious issue requiring immediate attention
    /// </summary>
    Critical = 3
}

/// <summary>
/// Result of a rule evaluation
/// </summary>
public class RuleResult
{
    /// <summary>
    /// Whether the rule passed
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Message describing the result
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The severity of the result
    /// </summary>
    public RuleSeverity Severity { get; set; }

    /// <summary>
    /// Additional metadata about the result
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    public RuleResult()
    {
        Message = string.Empty;
        Severity = RuleSeverity.Info;
    }

    public RuleResult(bool isSuccess, string message, RuleSeverity severity = RuleSeverity.Info)
    {
        IsSuccess = isSuccess;
        Message = message;
        Severity = severity;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static RuleResult Success(string message = "Rule passed")
    {
        return new RuleResult(true, message, RuleSeverity.Info);
    }

    /// <summary>
    /// Creates a failure result
    /// </summary>
    public static RuleResult Failure(string message, RuleSeverity severity = RuleSeverity.Error)
    {
        return new RuleResult(false, message, severity);
    }

    /// <summary>
    /// Creates a warning result
    /// </summary>
    public static RuleResult Warning(string message)
    {
        return new RuleResult(true, message, RuleSeverity.Warning);
    }
}

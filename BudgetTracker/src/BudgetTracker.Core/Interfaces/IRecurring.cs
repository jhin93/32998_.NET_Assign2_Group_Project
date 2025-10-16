using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Core.Interfaces;

/// <summary>
/// Interface for entities that support recurring/repeating behavior
/// </summary>
public interface IRecurring
{
    /// <summary>
    /// Gets or sets the frequency of recurrence
    /// </summary>
    Frequency RecurrenceFrequency { get; set; }

    /// <summary>
    /// Gets or sets the end date for recurrence (null = indefinite)
    /// </summary>
    DateTime? RecurrenceEndDate { get; set; }

    /// <summary>
    /// Gets whether this entity is recurring
    /// </summary>
    bool IsRecurring { get; }

    /// <summary>
    /// Sets up the recurrence pattern
    /// </summary>
    /// <param name="frequency">How often it recurs</param>
    /// <param name="endDate">Optional end date for recurrence</param>
    void SetRecurrence(Frequency frequency, DateTime? endDate = null);

    /// <summary>
    /// Calculates the next occurrence date
    /// </summary>
    /// <returns>Next occurrence date, or null if recurrence has ended</returns>
    DateTime? GetNextOccurrence();
}

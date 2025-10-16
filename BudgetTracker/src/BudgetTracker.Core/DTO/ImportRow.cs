namespace BudgetTracker.Core.DTO;

/// <summary>
/// Data Transfer Object for CSV import rows
/// Used to temporarily store transaction data from CSV before validation
/// </summary>
public class ImportRow
{
    public string Type { get; set; } = string.Empty;  // "Income" or "Expense"
    public string Description { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;  // For Income only

    // Validation properties
    public bool IsValid { get; set; }
    public string ValidationError { get; set; } = string.Empty;
    public int RowNumber { get; set; }
}

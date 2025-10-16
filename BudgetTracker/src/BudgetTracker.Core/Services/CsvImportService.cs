using BudgetTracker.Core.DTO;
using BudgetTracker.Core.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using System.Globalization;

namespace BudgetTracker.Core.Services;

/// <summary>
/// Service for importing transactions from CSV files
/// Phase 2.8: CSV Import Feature
/// Implements File I/O requirement
/// </summary>
public class CsvImportService
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Category> _categoryRepository;

    public CsvImportService(
        IRepository<Transaction> transactionRepository,
        IRepository<Category> categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Import transactions from a CSV file
    /// CSV Format: Type,Description,Amount,Date,Category,Account,Notes,Source
    /// </summary>
    public (int successCount, int errorCount, List<string> errors) ImportFromCsv(string filePath)
    {
        var errors = new List<string>();
        var successCount = 0;
        var errorCount = 0;

        try
        {
            // Validate file exists
            if (!File.Exists(filePath))
            {
                errors.Add("File not found.");
                return (0, 1, errors);
            }

            // Read all lines from CSV
            var lines = File.ReadAllLines(filePath);

            if (lines.Length <= 1)
            {
                errors.Add("CSV file is empty or contains only headers.");
                return (0, 1, errors);
            }

            // Parse header line (first line)
            var header = lines[0];

            // Parse data rows (skip header)
            var importRows = new List<ImportRow>();
            for (int i = 1; i < lines.Length; i++)
            {
                var row = ParseCsvLine(lines[i], i + 1);
                importRows.Add(row);
            }

            // Validate all rows
            ValidateImportRows(importRows);

            // Import valid rows
            foreach (var row in importRows.Where(r => r.IsValid))
            {
                try
                {
                    var transaction = CreateTransactionFromRow(row);
                    if (transaction != null)
                    {
                        _transactionRepository.Add(transaction);
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"Row {row.RowNumber}: Failed to create transaction.");
                        errorCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row.RowNumber}: {ex.Message}");
                    errorCount++;
                }
            }

            // Collect validation errors
            foreach (var row in importRows.Where(r => !r.IsValid))
            {
                errors.Add($"Row {row.RowNumber}: {row.ValidationError}");
                errorCount++;
            }

            // Save all changes
            if (successCount > 0)
            {
                _transactionRepository.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Error reading CSV file: {ex.Message}");
            errorCount++;
        }

        return (successCount, errorCount, errors);
    }

    /// <summary>
    /// Parse a CSV line into an ImportRow
    /// Handles quoted fields and commas within quotes
    /// </summary>
    private ImportRow ParseCsvLine(string line, int rowNumber)
    {
        var fields = new List<string>();
        var currentField = string.Empty;
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField.Trim());
                currentField = string.Empty;
            }
            else
            {
                currentField += c;
            }
        }

        // Add last field
        fields.Add(currentField.Trim());

        // Ensure we have at least 8 fields (pad with empty strings if needed)
        while (fields.Count < 8)
        {
            fields.Add(string.Empty);
        }

        return new ImportRow
        {
            Type = fields[0],
            Description = fields[1],
            Amount = fields[2],
            Date = fields[3],
            Category = fields[4],
            Account = fields[5],
            Notes = fields[6],
            Source = fields[7],
            RowNumber = rowNumber
        };
    }

    /// <summary>
    /// Validate import rows using LINQ
    /// </summary>
    private void ValidateImportRows(List<ImportRow> rows)
    {
        // Get all categories for validation
        var categories = _categoryRepository.GetAll().ToList();
        var categoryNames = categories.Select(c => c.Name.ToLower()).ToList();

        foreach (var row in rows)
        {
            // Validate Type
            if (string.IsNullOrWhiteSpace(row.Type))
            {
                row.IsValid = false;
                row.ValidationError = "Type is required.";
                continue;
            }

            if (row.Type.ToLower() != "income" && row.Type.ToLower() != "expense")
            {
                row.IsValid = false;
                row.ValidationError = $"Invalid type '{row.Type}'. Must be 'Income' or 'Expense'.";
                continue;
            }

            // Validate Description
            if (string.IsNullOrWhiteSpace(row.Description))
            {
                row.IsValid = false;
                row.ValidationError = "Description is required.";
                continue;
            }

            // Validate Amount
            if (!decimal.TryParse(row.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                row.IsValid = false;
                row.ValidationError = $"Invalid amount '{row.Amount}'. Must be a positive number.";
                continue;
            }

            // Validate Date
            if (!DateTime.TryParse(row.Date, out DateTime date))
            {
                row.IsValid = false;
                row.ValidationError = $"Invalid date '{row.Date}'. Use format: MM/DD/YYYY or YYYY-MM-DD.";
                continue;
            }

            // Validate Category
            if (string.IsNullOrWhiteSpace(row.Category))
            {
                row.IsValid = false;
                row.ValidationError = "Category is required.";
                continue;
            }

            if (!categoryNames.Contains(row.Category.ToLower()))
            {
                row.IsValid = false;
                row.ValidationError = $"Category '{row.Category}' not found. Please create it first.";
                continue;
            }

            // All validations passed
            row.IsValid = true;
        }
    }

    /// <summary>
    /// Create a Transaction entity from an ImportRow
    /// </summary>
    private Transaction? CreateTransactionFromRow(ImportRow row)
    {
        try
        {
            // Parse amount and date
            var amount = decimal.Parse(row.Amount, NumberStyles.Any, CultureInfo.InvariantCulture);
            var date = DateTime.Parse(row.Date);
            var money = new Money(amount, "USD");

            // Find category by name
            var category = _categoryRepository.GetAll()
                .FirstOrDefault(c => c.Name.Equals(row.Category, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                return null;
            }

            // Create transaction based on type
            if (row.Type.Equals("Income", StringComparison.OrdinalIgnoreCase))
            {
                return new Income(
                    row.Description,
                    money,
                    date,
                    category.Id,
                    row.Notes,
                    row.Account,
                    row.Source
                );
            }
            else
            {
                return new Expense(
                    row.Description,
                    money,
                    date,
                    category.Id,
                    row.Notes,
                    row.Account
                );
            }
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Generate a sample CSV template for users
    /// </summary>
    public static string GenerateSampleCsv()
    {
        var csv = "Type,Description,Amount,Date,Category,Account,Notes,Source\n";
        csv += "Income,Monthly Salary,5000.00,2024-01-01,Salary,Bank Account,January salary,Employer\n";
        csv += "Expense,Grocery Shopping,250.50,2024-01-05,Food,Credit Card,Weekly groceries,\n";
        csv += "Expense,Gas Station,45.00,2024-01-06,Transportation,Cash,Fuel,\n";
        csv += "Income,Freelance Project,1200.00,2024-01-10,Freelance,Bank Account,Web design project,Client ABC\n";
        csv += "Expense,Electric Bill,120.00,2024-01-15,Utilities,Bank Account,Monthly electric,\n";

        return csv;
    }

    /// <summary>
    /// Export transactions to CSV format
    /// </summary>
    public void ExportToCsv(string filePath, IEnumerable<Transaction> transactions)
    {
        try
        {
            var csv = "Type,Description,Amount,Date,Category,Account,Notes,Source\n";

            foreach (var transaction in transactions)
            {
                var category = _categoryRepository.GetById(transaction.CategoryId);
                var type = transaction is Income ? "Income" : "Expense";
                var source = transaction is Income income ? income.Source : string.Empty;

                // Escape fields that contain commas
                var description = EscapeCsvField(transaction.Description);
                var notes = EscapeCsvField(transaction.Notes ?? string.Empty);
                var account = EscapeCsvField(transaction.Account ?? string.Empty);
                var categoryName = EscapeCsvField(category?.Name ?? "Unknown");
                var sourceField = EscapeCsvField(source ?? string.Empty);

                csv += $"{type},{description},{transaction.Amount.Amount},{transaction.Date:yyyy-MM-dd},{categoryName},{account},{notes},{sourceField}\n";
            }

            File.WriteAllText(filePath, csv);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error exporting to CSV: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Escape CSV field if it contains special characters
    /// </summary>
    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;

        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}

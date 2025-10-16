using BudgetTracker.Core.Services;
using BudgetTracker.Data;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Tests.Services;

/// <summary>
/// NUnit tests for CsvImportService
/// Phase 6.19: NUnit Test Writing (Required)
/// Tests CSV import functionality and validation
/// </summary>
[TestFixture]
public class CsvImportTests
{
    private InMemoryRepository<Transaction> _transactionRepository = null!;
    private InMemoryRepository<Category> _categoryRepository = null!;
    private CsvImportService _csvImportService = null!;
    private string _testFilePath = null!;
    private Category _foodCategory = null!;
    private Category _salaryCategory = null!;
    private Category _transportCategory = null!;

    [SetUp]
    public void Setup()
    {
        // Initialize repositories
        _transactionRepository = new InMemoryRepository<Transaction>();
        _categoryRepository = new InMemoryRepository<Category>();

        // Create test categories
        _foodCategory = new Category { Id = 1, Name = "Food", Description = "Food and groceries" };
        _salaryCategory = new Category { Id = 2, Name = "Salary", Description = "Income from salary" };
        _transportCategory = new Category { Id = 3, Name = "Transportation", Description = "Transport expenses" };

        _categoryRepository.Add(_foodCategory);
        _categoryRepository.Add(_salaryCategory);
        _categoryRepository.Add(_transportCategory);

        // Initialize service
        _csvImportService = new CsvImportService(_transactionRepository, _categoryRepository);

        // Set up test file path
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.csv");
    }

    [Test]
    public void ImportFromCsv_WithValidData_ImportsSuccessfully()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Monthly Salary,5000.00,2024-01-01,Salary,Bank Account,January salary,Employer
Expense,Grocery Shopping,250.50,2024-01-05,Food,Credit Card,Weekly groceries,
Expense,Gas Station,45.00,2024-01-06,Transportation,Cash,Fuel,";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(3));
        Assert.That(result.errorCount, Is.EqualTo(0));
        Assert.That(result.errors, Is.Empty);
        Assert.That(_transactionRepository.GetAll().Count(), Is.EqualTo(3));
    }

    [Test]
    public void ImportFromCsv_WithEmptyFile_ReturnsError()
    {
        // Arrange
        var csvContent = "Type,Description,Amount,Date,Category,Account,Notes,Source\n";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors, Has.Count.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("empty"));
    }

    [Test]
    public void ImportFromCsv_WithNonExistentFile_ReturnsError()
    {
        // Act
        var result = _csvImportService.ImportFromCsv("nonexistent_file.csv");

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("not found"));
    }

    [Test]
    public void ImportFromCsv_WithInvalidType_ReturnsError()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
InvalidType,Test Transaction,100.00,2024-01-01,Food,Cash,Test,";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("Invalid type"));
    }

    [Test]
    public void ImportFromCsv_WithMissingDescription_ReturnsError()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,,1000.00,2024-01-01,Salary,Bank Account,Test,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("Description is required"));
    }

    [Test]
    public void ImportFromCsv_WithInvalidAmount_ReturnsError()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test Salary,invalid,2024-01-01,Salary,Bank Account,Test,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("Invalid amount"));
    }

    [Test]
    public void ImportFromCsv_WithNegativeAmount_ReturnsError()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test Salary,-1000.00,2024-01-01,Salary,Bank Account,Test,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("Invalid amount"));
    }

    [Test]
    public void ImportFromCsv_WithZeroAmount_ReturnsError()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test Salary,0,2024-01-01,Salary,Bank Account,Test,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("Invalid amount"));
    }

    [Test]
    public void ImportFromCsv_WithInvalidDate_ReturnsError()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test Salary,1000.00,invalid-date,Salary,Bank Account,Test,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("Invalid date"));
    }

    [Test]
    public void ImportFromCsv_WithMissingCategory_ReturnsError()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test Salary,1000.00,2024-01-01,,Bank Account,Test,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("Category is required"));
    }

    [Test]
    public void ImportFromCsv_WithNonExistentCategory_ReturnsError()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test Salary,1000.00,2024-01-01,NonExistentCategory,Bank Account,Test,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(0));
        Assert.That(result.errorCount, Is.EqualTo(1));
        Assert.That(result.errors[0], Does.Contain("not found"));
    }

    [Test]
    public void ImportFromCsv_WithMixedValidAndInvalidRows_ImportsOnlyValid()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Valid Salary,5000.00,2024-01-01,Salary,Bank Account,Valid,Employer
Income,Invalid Amount,invalid,2024-01-02,Salary,Bank Account,Invalid,Employer
Expense,Valid Grocery,250.00,2024-01-03,Food,Cash,Valid,
InvalidType,Invalid Type,100.00,2024-01-04,Food,Cash,Invalid,";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(2));
        Assert.That(result.errorCount, Is.EqualTo(2));
        Assert.That(result.errors, Has.Count.EqualTo(2));
        Assert.That(_transactionRepository.GetAll().Count(), Is.EqualTo(2));
    }

    [Test]
    public void ImportFromCsv_WithQuotedFields_ParsesCorrectly()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,""Salary with, comma"",5000.00,2024-01-01,Salary,Bank Account,Test,Employer
Expense,""Grocery with ""quotes"""",250.00,2024-01-05,Food,Cash,Test,";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(2));
        Assert.That(result.errorCount, Is.EqualTo(0));
        Assert.That(_transactionRepository.GetAll().Count(), Is.EqualTo(2));
    }

    [Test]
    public void ImportFromCsv_WithIncomeType_CreatesIncomeTransaction()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test Salary,5000.00,2024-01-01,Salary,Bank Account,Test notes,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(1));
        var transaction = _transactionRepository.GetAll().First();
        Assert.That(transaction, Is.InstanceOf<Income>());
        Assert.That(transaction.Description, Is.EqualTo("Test Salary"));
        Assert.That(transaction.Amount.Amount, Is.EqualTo(5000.00m));
    }

    [Test]
    public void ImportFromCsv_WithExpenseType_CreatesExpenseTransaction()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Expense,Test Grocery,250.00,2024-01-05,Food,Cash,Test notes,";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(1));
        var transaction = _transactionRepository.GetAll().First();
        Assert.That(transaction, Is.InstanceOf<Expense>());
        Assert.That(transaction.Description, Is.EqualTo("Test Grocery"));
        Assert.That(transaction.Amount.Amount, Is.EqualTo(250.00m));
    }

    [Test]
    public void ImportFromCsv_CaseInsensitiveType_WorksCorrectly()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
INCOME,Test Salary,5000.00,2024-01-01,Salary,Bank Account,Test,Employer
expense,Test Grocery,250.00,2024-01-05,Food,Cash,Test,";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(2));
        Assert.That(result.errorCount, Is.EqualTo(0));
    }

    [Test]
    public void ImportFromCsv_CaseInsensitiveCategory_WorksCorrectly()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test Salary,5000.00,2024-01-01,SALARY,Bank Account,Test,Employer
Expense,Test Grocery,250.00,2024-01-05,food,Cash,Test,";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(2));
        Assert.That(result.errorCount, Is.EqualTo(0));
    }

    [Test]
    public void ImportFromCsv_WithDifferentDateFormats_WorksCorrectly()
    {
        // Arrange
        var csvContent = @"Type,Description,Amount,Date,Category,Account,Notes,Source
Income,Test 1,5000.00,2024-01-01,Salary,Bank Account,Test,Employer
Income,Test 2,5000.00,2024-01-15,Salary,Bank Account,Test,Employer";
        File.WriteAllText(_testFilePath, csvContent);

        // Act
        var result = _csvImportService.ImportFromCsv(_testFilePath);

        // Assert
        Assert.That(result.successCount, Is.EqualTo(2));
        Assert.That(result.errorCount, Is.EqualTo(0));
    }

    [Test]
    public void GenerateSampleCsv_ReturnsValidCsvFormat()
    {
        // Act
        var sampleCsv = CsvImportService.GenerateSampleCsv();

        // Assert
        Assert.That(sampleCsv, Is.Not.Null);
        Assert.That(sampleCsv, Is.Not.Empty);
        Assert.That(sampleCsv, Does.StartWith("Type,Description,Amount,Date,Category"));
        Assert.That(sampleCsv, Does.Contain("Income"));
        Assert.That(sampleCsv, Does.Contain("Expense"));
    }

    [Test]
    public void ExportToCsv_WithTransactions_CreatesValidCsv()
    {
        // Arrange
        var income = new Income("Salary", new BudgetTracker.Domain.ValueObjects.Money(5000, "USD"), DateTime.Parse("2024-01-01"), _salaryCategory.Id);
        var expense = new Expense("Groceries", new BudgetTracker.Domain.ValueObjects.Money(250, "USD"), DateTime.Parse("2024-01-05"), _foodCategory.Id);
        _transactionRepository.Add(income);
        _transactionRepository.Add(expense);

        var exportPath = Path.Combine(Path.GetTempPath(), $"export_{Guid.NewGuid()}.csv");

        // Act
        _csvImportService.ExportToCsv(exportPath, _transactionRepository.GetAll());

        // Assert
        Assert.That(File.Exists(exportPath), Is.True);
        var content = File.ReadAllText(exportPath);
        Assert.That(content, Does.Contain("Type,Description,Amount,Date,Category"));
        Assert.That(content, Does.Contain("Income"));
        Assert.That(content, Does.Contain("Expense"));
        Assert.That(content, Does.Contain("Salary"));
        Assert.That(content, Does.Contain("Groceries"));

        // Cleanup
        File.Delete(exportPath);
    }

    [Test]
    public void ExportToCsv_WithFieldsContainingCommas_EscapesCorrectly()
    {
        // Arrange
        var income = new Income("Salary, with comma", new BudgetTracker.Domain.ValueObjects.Money(5000, "USD"), DateTime.Parse("2024-01-01"), _salaryCategory.Id);
        _transactionRepository.Add(income);

        var exportPath = Path.Combine(Path.GetTempPath(), $"export_{Guid.NewGuid()}.csv");

        // Act
        _csvImportService.ExportToCsv(exportPath, _transactionRepository.GetAll());

        // Assert
        Assert.That(File.Exists(exportPath), Is.True);
        var content = File.ReadAllText(exportPath);
        Assert.That(content, Does.Contain("\"Salary, with comma\""));

        // Cleanup
        File.Delete(exportPath);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test file
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }
}

## 32998 .NET Assign2 Group Project

This is the space for the assignment 2 group project.

## Getting Started

1. Open the solution in Visual Studio
2. Run the solution
3. Use the sample.csv file to test the import functionality

## Structure
```dotnet
BudgetTracker/
├─ BudgetTracker.sln
├─ src/
│  ├─ BudgetTracker.App/                # WinForms UI (desktop application)
│  │  ├─ Forms/
│  │  │  ├─ DashboardForm.cs
│  │  │  ├─ TransactionsForm.cs
│  │  │  ├─ TransactionDialog.cs
│  │  │  ├─ BudgetsForm.cs
│  │  │  ├─ BudgetDialog.cs
│  │  │  ├─ ReportsForm.cs
│  │  │  ├─ SettingsForm.cs
│  │  │  └─ CategoryDialog.cs
│  │  └─ Program.cs
│  ├─ BudgetTracker.Web/                # Blazor Web App
│  │  ├─ Components/
│  │  │  ├─ Layout/
│  │  │  │  ├─ MainLayout.razor
│  │  │  │  └─ NavMenu.razor
│  │  │  ├─ Pages/
│  │  │  │  ├─ Home.razor
│  │  │  │  ├─ Counter.razor
│  │  │  │  ├─ Weather.razor
│  │  │  │  └─ Error.razor
│  │  │  ├─ TransactionModal.razor
│  │  │  ├─ BudgetModal.razor
│  │  │  ├─ CategoryModal.razor
│  │  │  ├─ AccountModal.razor
│  │  │  ├─ CsvImportModal.razor
│  │  │  ├─ App.razor
│  │  │  ├─ Routes.razor
│  │  │  └─ _Imports.razor
│  │  ├─ wwwroot/
│  │  ├─ Program.cs
│  │  ├─ appsettings.json
│  │  └─ appsettings.Development.json
│  ├─ BudgetTracker.Domain/             # Entities, enums, value objects
│  │  ├─ Entities/
│  │  │  ├─ Transaction.cs
│  │  │  ├─ Expense.cs
│  │  │  ├─ Income.cs
│  │  │  ├─ Category.cs
│  │  │  ├─ Budget.cs
│  │  │  ├─ Account.cs
│  │  │  └─ RecurringTransaction.cs
│  │  ├─ ValueObjects/
│  │  │  └─ Money.cs
│  │  └─ Enums/
│  │     └─ Frequency.cs
│  ├─ BudgetTracker.Core/               # Interfaces + core services
│  │  ├─ Interfaces/
│  │  │  ├─ IRepository.cs
│  │  │  ├─ IRecurring.cs
│  │  │  ├─ IExportable.cs
│  │  │  └─ Rules/
│  │  │     ├─ IRule.cs
│  │  │     └─ RuleBase.cs
│  │  ├─ Services/
│  │  │  ├─ RuleEngine.cs
│  │  │  ├─ BudgetService.cs
│  │  │  ├─ ReportService.cs
│  │  │  ├─ CsvImportService.cs
│  │  │  └─ RecurringTransactionService.cs
│  │  ├─ Extensions/
│  │  │  ├─ DateTimeExtensions.cs
│  │  │  └─ DecimalExtensions.cs
│  │  ├─ Events/
│  │  │  └─ BudgetTrackerEvents.cs
│  │  └─ DTO/
│  │     └─ ImportRow.cs
│  ├─ BudgetTracker.Data/               # Persistence (in-memory)
│  │  ├─ InMemoryRepository.cs
│  │  └─ SeedData.cs
│  └─ BudgetTracker.Tests/
│     ├─ Domain/
│     │  └─ TransactionTests.cs
│     └─ Services/
│        ├─ BudgetServiceTests.cs
│        ├─ RuleEngineTests.cs
│        ├─ CsvImportTests.cs
│        ├─ RecurringTransactionServiceTests.cs
│        └─ ReportServiceTests.cs
```

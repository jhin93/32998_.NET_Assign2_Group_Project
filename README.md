## 32998 .NET Assign2 Group Project

This is the space for the assignment 2 group project.
Strucrture
```dotnet
BudgetTracker/
├─ BudgetTracker.sln
├─ src/
│  ├─ BudgetTracker.App/                # WinForms UI (startup project)
│  │  ├─ Forms/
│  │  │  ├─ DashboardForm.cs
│  │  │  ├─ TransactionsForm.cs
│  │  │  ├─ BudgetsForm.cs
│  │  │  ├─ ReportsForm.cs
│  │  │  └─ SettingsForm.cs
│  │  ├─ Program.cs
│  │  └─ App.config
│  ├─ BudgetTracker.Domain/             # Entities, enums, value objects
│  │  ├─ Entities/
│  │  │  ├─ Transaction.cs
│  │  │  ├─ Expense.cs
│  │  │  ├─ Income.cs
│  │  │  ├─ Category.cs
│  │  │  └─ Budget.cs
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
│  │  │  └─ CsvImportService.cs
│  │  └─ DTO/
│  │     └─ ImportRow.cs
│  ├─ BudgetTracker.Data/               # Persistence (start with in-memory; swap to EF later)
│  │  ├─ InMemoryRepository.cs
│  │  ├─ SeedData.cs
│  │  └─ (optional EF later)
│  │     ├─ AppDbContext.cs
│  │     └─ EfRepository.cs
└─ tests/
   └─ BudgetTracker.Tests/
      ├─ BudgetServiceTests.cs
      ├─ RuleEngineTests.cs
      └─ CsvImportTests.cs
```

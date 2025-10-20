## 32998 .NET Assign2 Group Project

Budget Tracker - A personal finance management application with both desktop (WinForms) and web (Blazor) interfaces.

## Prerequisites

### Required Software
1. **.NET 9.0 SDK or later**
   - Check your current version: `dotnet --version`
   - If version is less than 9.0, download and install from:
     - **Windows/Mac/Linux**: https://dotnet.microsoft.com/download/dotnet/9.0
   - After installation, verify: `dotnet --version` (should show 9.0.x or higher)

2. **IDE (Choose one)**
   - **Visual Studio 2022** (17.8 or later) - Recommended for Windows
     - Download: https://visualstudio.microsoft.com/downloads/
     - Select workloads: ".NET desktop development" and "ASP.NET and web development"
   - **Visual Studio Code** - For all platforms
     - Download: https://code.visualstudio.com/
     - Install C# extension: https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp
   - **JetBrains Rider** - Alternative IDE
     - Download: https://www.jetbrains.com/rider/

3. **Git** (for cloning the repository)
   - Download: https://git-scm.com/downloads

## Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd BudgetTracker
```

### 2. Restore Dependencies
```bash
# Navigate to the solution directory
cd BudgetTracker

# Restore all NuGet packages
dotnet restore
```

### 3. Build the Solution
```bash
# Build all projects
dotnet build

# Or build in Release mode
dotnet build --configuration Release
```

### 4. Run Tests (Optional but Recommended)
```bash
# Run all unit tests
dotnet test

# Expected output: All 115+ tests should pass
```

## Running the Applications

### Option A: Blazor Web Application (Recommended)

**Step-by-step instructions:**

1. **Navigate to the Web project folder:**
   ```bash
   cd src/BudgetTracker.Web
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Access the application:**
   - The application will start on: `https://localhost:5001` or `http://localhost:5000`
   - Open your web browser and navigate to the URL shown in the terminal
   - Example output:
     ```
     Now listening on: https://localhost:5001
     Now listening on: http://localhost:5000
     ```

4. **Stop the application:**
   - Press `Ctrl + C` in the terminal

**Alternative: Using Visual Studio**
1. Open `BudgetTracker.sln`
2. In Solution Explorer, right-click on `BudgetTracker.Web` project
3. Select "Set as Startup Project"
4. Press `F5` or click the green "Run" button
5. Your default browser will automatically open

### Option B: WinForms Desktop Application (Windows Only)

**Step-by-step instructions:**

1. **Navigate to the App project folder:**
   ```bash
   cd src/BudgetTracker.App
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **The desktop application window will appear automatically**

**Alternative: Using Visual Studio**
1. Open `BudgetTracker.sln`
2. In Solution Explorer, right-click on `BudgetTracker.App` project
3. Select "Set as Startup Project"
4. Press `F5` or click the green "Run" button

## Testing CSV Import Functionality

1. Locate the `sample.csv` file in the project root directory
2. In the application (Web or Desktop):
   - Navigate to "Transactions" or "Import" section
   - Click "Import CSV" button
   - Select the `sample.csv` file
   - Verify that transactions are imported successfully

## Troubleshooting

### Issue: "dotnet: command not found"
**Solution:**
1. Install .NET SDK 9.0 from https://dotnet.microsoft.com/download
2. Restart your terminal/command prompt
3. Verify installation: `dotnet --version`

### Issue: "The framework 'Microsoft.NETCore.App', version '9.0.x' was not found"
**Solution:**
1. Your .NET SDK version is too old
2. Update to .NET 9.0 or later: https://dotnet.microsoft.com/download/dotnet/9.0
3. Restart your terminal and try again

### Issue: Blazor app doesn't start or shows port conflict
**Solution:**
1. Check if port 5000/5001 is already in use
2. Stop the process using that port, or
3. Run with a different port:
   ```bash
   dotnet run --urls "https://localhost:7001;http://localhost:7000"
   ```

### Issue: WinForms app doesn't run on Mac/Linux
**Solution:**
- WinForms is Windows-only
- Use the Blazor Web Application instead (`BudgetTracker.Web`)

### Issue: Build errors after cloning
**Solution:**
1. Clean the solution:
   ```bash
   dotnet clean
   ```
2. Restore packages:
   ```bash
   dotnet restore
   ```
3. Rebuild:
   ```bash
   dotnet build
   ```

### Issue: Tests fail
**Solution:**
1. Ensure all dependencies are restored: `dotnet restore`
2. Rebuild the solution: `dotnet build`
3. Run tests with verbose output:
   ```bash
   dotnet test --verbosity detailed
   ```

## Project Architecture

- **BudgetTracker.Domain**: Core business entities and domain logic
- **BudgetTracker.Core**: Business services, interfaces, and rules
- **BudgetTracker.Data**: Data access layer (in-memory repository)
- **BudgetTracker.App**: WinForms desktop application
- **BudgetTracker.Web**: Blazor web application
- **BudgetTracker.Tests**: NUnit test project (115+ tests)

## Features

- Transaction management (Income/Expense)
- Budget creation and tracking
- Category management
- CSV import/export
- Recurring transactions
- Reports and analytics
- Budget alerts and warnings

## Structure
```dotnet
BudgetTracker/
├─ BudgetTracker.sln
└─ src/
   ├─ BudgetTracker.App/                # WinForms UI (desktop application)
   │  ├─ Forms/
   │  │  ├─ DashboardForm.cs
   │  │  ├─ TransactionsForm.cs
   │  │  ├─ TransactionDialog.cs
   │  │  ├─ BudgetsForm.cs
   │  │  ├─ BudgetDialog.cs
   │  │  ├─ ReportsForm.cs
   │  │  ├─ SettingsForm.cs
   │  │  └─ CategoryDialog.cs
   │  └─ Program.cs
   ├─ BudgetTracker.Web/                # Blazor Web App
   │  ├─ Components/
   │  │  ├─ Layout/
   │  │  │  ├─ MainLayout.razor
   │  │  │  └─ NavMenu.razor
   │  │  ├─ Pages/
   │  │  │  ├─ Home.razor
   │  │  │  ├─ Counter.razor
   │  │  │  ├─ Weather.razor
   │  │  │  └─ Error.razor
   │  │  ├─ TransactionModal.razor
   │  │  ├─ BudgetModal.razor
   │  │  ├─ CategoryModal.razor
   │  │  ├─ AccountModal.razor
   │  │  ├─ CsvImportModal.razor
   │  │  ├─ App.razor
   │  │  ├─ Routes.razor
   │  │  └─ _Imports.razor
   │  ├─ wwwroot/
   │  ├─ Program.cs
   │  ├─ appsettings.json
   │  └─ appsettings.Development.json
   ├─ BudgetTracker.Domain/             # Entities, enums, value objects
   │  ├─ Entities/
   │  │  ├─ Transaction.cs
   │  │  ├─ Expense.cs
   │  │  ├─ Income.cs
   │  │  ├─ Category.cs
   │  │  ├─ Budget.cs
   │  │  ├─ Account.cs
   │  │  └─ RecurringTransaction.cs
   │  ├─ ValueObjects/
   │  │  └─ Money.cs
   │  └─ Enums/
   │     └─ Frequency.cs
   ├─ BudgetTracker.Core/               # Interfaces + core services
   │  ├─ Interfaces/
   │  │  ├─ IRepository.cs
   │  │  ├─ IRecurring.cs
   │  │  ├─ IExportable.cs
   │  │  └─ Rules/
   │  │     ├─ IRule.cs
   │  │     └─ RuleBase.cs
   │  ├─ Services/
   │  │  ├─ RuleEngine.cs
   │  │  ├─ BudgetService.cs
   │  │  ├─ ReportService.cs
   │  │  ├─ CsvImportService.cs
   │  │  └─ RecurringTransactionService.cs
   │  ├─ Extensions/
   │  │  ├─ DateTimeExtensions.cs
   │  │  └─ DecimalExtensions.cs
   │  ├─ Events/
   │  │  └─ BudgetTrackerEvents.cs
   │  └─ DTO/
   │     └─ ImportRow.cs
   ├─ BudgetTracker.Data/               # Persistence (in-memory)
   │  ├─ InMemoryRepository.cs
   │  └─ SeedData.cs
   └─ BudgetTracker.Tests/
      ├─ Domain/
      │  └─ TransactionTests.cs
      └─ Services/
         ├─ BudgetServiceTests.cs
         ├─ RuleEngineTests.cs
         ├─ CsvImportTests.cs
         ├─ RecurringTransactionServiceTests.cs
         └─ ReportServiceTests.cs
```

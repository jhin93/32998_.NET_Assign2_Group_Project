using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Data;

/// <summary>
/// Provides seed data for testing and demonstration purposes
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Creates default categories
    /// </summary>
    public static List<Category> GetCategories()
    {
        return new List<Category>
        {
            new Category("Groceries", "Food and household items", "#4CAF50", "🛒") { Id = 1 },
            new Category("Transportation", "Gas, public transit, parking", "#2196F3", "🚗") { Id = 2 },
            new Category("Entertainment", "Movies, games, hobbies", "#9C27B0", "🎬") { Id = 3 },
            new Category("Utilities", "Electricity, water, internet", "#FF9800", "⚡") { Id = 4 },
            new Category("Healthcare", "Medical expenses, insurance", "#F44336", "🏥") { Id = 5 },
            new Category("Dining Out", "Restaurants, cafes", "#E91E63", "🍽️") { Id = 6 },
            new Category("Shopping", "Clothes, electronics", "#00BCD4", "🛍️") { Id = 7 },
            new Category("Housing", "Rent, mortgage", "#795548", "🏠") { Id = 8 },
            new Category("Salary", "Monthly salary", "#4CAF50", "💰") { Id = 9 },
            new Category("Investment", "Stock returns, dividends", "#8BC34A", "📈") { Id = 10 },
            new Category("Other", "Miscellaneous", "#9E9E9E", "📦") { Id = 11 }
        };
    }

    /// <summary>
    /// Creates sample transactions for the current month
    /// </summary>
    public static List<Transaction> GetTransactions()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        var transactions = new List<Transaction>();

        // Income transactions
        transactions.Add(new Income("Monthly Salary", new Money(5000, "USD"), startOfMonth.AddDays(1), 9, "Regular monthly salary", "Bank Account", "Employer")
        {
            Id = 1
        });

        transactions.Add(new Income("Freelance Project", new Money(800, "USD"), startOfMonth.AddDays(15), 9, "Web development project", "Bank Account", "Client")
        {
            Id = 2
        });

        // Expense transactions
        transactions.Add(new Expense("Weekly Groceries", new Money(150, "USD"), startOfMonth.AddDays(2), 1, "Supermarket shopping", "Credit Card")
        {
            Id = 3
        });

        transactions.Add(new Expense("Gas", new Money(60, "USD"), startOfMonth.AddDays(3), 2, "Fill up tank", "Credit Card")
        {
            Id = 4
        });

        transactions.Add(new Expense("Movie Night", new Money(45, "USD"), startOfMonth.AddDays(5), 3, "Cinema tickets and snacks", "Cash")
        {
            Id = 5
        });

        transactions.Add(new Expense("Electricity Bill", new Money(120, "USD"), startOfMonth.AddDays(7), 4, "Monthly electricity", "Bank Account")
        {
            Id = 6
        });

        transactions.Add(new Expense("Internet Bill", new Money(80, "USD"), startOfMonth.AddDays(7), 4, "Monthly internet", "Bank Account")
        {
            Id = 7
        });

        transactions.Add(new Expense("Lunch", new Money(25, "USD"), startOfMonth.AddDays(8), 6, "Restaurant lunch", "Credit Card")
        {
            Id = 8
        });

        transactions.Add(new Expense("Weekly Groceries", new Money(140, "USD"), startOfMonth.AddDays(9), 1, "Supermarket shopping", "Credit Card")
        {
            Id = 9
        });

        transactions.Add(new Expense("Doctor Visit", new Money(200, "USD"), startOfMonth.AddDays(10), 5, "Regular checkup", "Insurance")
        {
            Id = 10
        });

        transactions.Add(new Expense("Dinner Date", new Money(85, "USD"), startOfMonth.AddDays(12), 6, "Nice restaurant", "Credit Card")
        {
            Id = 11
        });

        transactions.Add(new Expense("New Shoes", new Money(120, "USD"), startOfMonth.AddDays(14), 7, "Running shoes", "Credit Card")
        {
            Id = 12
        });

        transactions.Add(new Expense("Weekly Groceries", new Money(155, "USD"), startOfMonth.AddDays(16), 1, "Supermarket shopping", "Credit Card")
        {
            Id = 13
        });

        transactions.Add(new Expense("Gas", new Money(65, "USD"), startOfMonth.AddDays(17), 2, "Fill up tank", "Credit Card")
        {
            Id = 14
        });

        transactions.Add(new Expense("Coffee", new Money(15, "USD"), startOfMonth.AddDays(18), 6, "Morning coffee", "Cash")
        {
            Id = 15
        });

        transactions.Add(new Expense("Gym Membership", new Money(50, "USD"), startOfMonth.AddDays(20), 5, "Monthly membership", "Bank Account")
        {
            Id = 16
        });

        transactions.Add(new Expense("Weekly Groceries", new Money(145, "USD"), startOfMonth.AddDays(23), 1, "Supermarket shopping", "Credit Card")
        {
            Id = 17
        });

        transactions.Add(new Expense("Rent", new Money(1500, "USD"), startOfMonth.AddDays(1), 8, "Monthly rent", "Bank Account")
        {
            Id = 18
        });

        return transactions;
    }

    /// <summary>
    /// Creates sample budgets for the current month
    /// </summary>
    public static List<Budget> GetBudgets()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        var budgets = new List<Budget>();

        budgets.Add(Budget.CreateMonthly("Groceries Budget", 600, 1, startOfMonth));
        budgets[0].Id = 1;

        budgets.Add(Budget.CreateMonthly("Transportation Budget", 200, 2, startOfMonth));
        budgets[1].Id = 2;

        budgets.Add(Budget.CreateMonthly("Entertainment Budget", 150, 3, startOfMonth));
        budgets[2].Id = 3;

        budgets.Add(Budget.CreateMonthly("Utilities Budget", 250, 4, startOfMonth));
        budgets[3].Id = 4;

        budgets.Add(Budget.CreateMonthly("Healthcare Budget", 300, 5, startOfMonth));
        budgets[4].Id = 5;

        budgets.Add(Budget.CreateMonthly("Dining Out Budget", 200, 6, startOfMonth));
        budgets[5].Id = 6;

        budgets.Add(Budget.CreateMonthly("Shopping Budget", 300, 7, startOfMonth));
        budgets[6].Id = 7;

        budgets.Add(Budget.CreateMonthly("Housing Budget", 1600, 8, startOfMonth));
        budgets[7].Id = 8;

        // Update current spent for each budget based on transactions
        var transactions = GetTransactions();
        foreach (var budget in budgets)
        {
            var categoryTransactions = transactions
                .Where(t => t is Expense && t.CategoryId == budget.CategoryId && t.Date >= budget.StartDate && t.Date <= budget.EndDate)
                .Cast<Expense>();

            foreach (var transaction in categoryTransactions)
            {
                budget.AddSpending(transaction.Amount);
            }
        }

        return budgets;
    }

    /// <summary>
    /// Seeds all data into repositories
    /// </summary>
    public static void SeedAllData(
        InMemoryRepository<Category> categoryRepo,
        InMemoryRepository<Transaction> transactionRepo,
        InMemoryRepository<Budget> budgetRepo)
    {
        // Clear existing data
        categoryRepo.Clear();
        transactionRepo.Clear();
        budgetRepo.Clear();

        // Add categories
        categoryRepo.AddRange(GetCategories());
        categoryRepo.SetNextId(12);

        // Add transactions
        transactionRepo.AddRange(GetTransactions());
        transactionRepo.SetNextId(19);

        // Add budgets
        budgetRepo.AddRange(GetBudgets());
        budgetRepo.SetNextId(9);

        categoryRepo.SaveChanges();
        transactionRepo.SaveChanges();
        budgetRepo.SaveChanges();
    }
}

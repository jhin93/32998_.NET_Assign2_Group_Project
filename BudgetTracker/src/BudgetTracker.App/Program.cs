using BudgetTracker.App.Forms;
using BudgetTracker.Data;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.App;

static class Program
{
    // Global repositories - accessible throughout the application
    public static InMemoryRepository<Category> CategoryRepository { get; private set; } = null!;
    public static InMemoryRepository<Transaction> TransactionRepository { get; private set; } = null!;
    public static InMemoryRepository<Budget> BudgetRepository { get; private set; } = null!;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Initialize repositories
        CategoryRepository = new InMemoryRepository<Category>();
        TransactionRepository = new InMemoryRepository<Transaction>();
        BudgetRepository = new InMemoryRepository<Budget>();

        // Seed initial data
        SeedData.SeedAllData(CategoryRepository, TransactionRepository, BudgetRepository);

        // Configure application
        ApplicationConfiguration.Initialize();

        // Run the main form (SettingsForm for now)
        Application.Run(new SettingsForm());
    }
}

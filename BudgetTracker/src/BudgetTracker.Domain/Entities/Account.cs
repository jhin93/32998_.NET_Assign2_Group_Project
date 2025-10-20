namespace BudgetTracker.Domain.Entities;

/// <summary>
/// Represents a financial account for tracking transactions
/// </summary>
public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AccountType { get; set; }
    public decimal InitialBalance { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property for Entity Framework
    public virtual ICollection<Transaction>? Transactions { get; set; }

    public Account()
    {
        Name = string.Empty;
        AccountType = "bank";
        InitialBalance = 0;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Account(string name, string accountType, decimal initialBalance, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name cannot be null or empty", nameof(name));

        Name = name;
        AccountType = accountType;
        InitialBalance = initialBalance;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates account information
    /// </summary>
    public void Update(string name, string accountType, decimal initialBalance, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name cannot be null or empty", nameof(name));

        Name = name;
        AccountType = accountType;
        InitialBalance = initialBalance;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the account (soft delete)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactivates the account
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return Name;
    }
}

namespace BudgetTracker.Domain.Entities;

/// <summary>
/// Represents a category for organizing transactions
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }  // Hex color code for UI display
    public string? Icon { get; set; }   // Icon name or emoji
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property for Entity Framework
    public virtual ICollection<Transaction>? Transactions { get; set; }

    public Category()
    {
        Name = string.Empty;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Category(string name, string? description = null, string? color = null, string? icon = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty", nameof(name));

        Name = name;
        Description = description;
        Color = color;
        Icon = icon;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates category information
    /// </summary>
    public void Update(string name, string? description = null, string? color = null, string? icon = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty", nameof(name));

        Name = name;
        Description = description;
        Color = color;
        Icon = icon;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the category (soft delete)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactivates the category
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

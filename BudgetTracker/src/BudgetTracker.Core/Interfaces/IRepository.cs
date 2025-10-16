using System.Linq.Expressions;

namespace BudgetTracker.Core.Interfaces;

/// <summary>
/// Generic Repository Pattern interface
/// Demonstrates GENERICS - works with any entity type T
/// Provides abstraction for data access operations
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IRepository<T> where T : class
{
    // Read operations
    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    T? GetById(int id);

    /// <summary>
    /// Gets all entities
    /// </summary>
    IEnumerable<T> GetAll();

    /// <summary>
    /// Finds entities matching a predicate using LINQ expressions
    /// </summary>
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets the first entity matching a predicate, or null if not found
    /// </summary>
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Checks if any entity matches the predicate
    /// </summary>
    bool Any(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets the count of all entities
    /// </summary>
    int Count();

    /// <summary>
    /// Gets the count of entities matching a predicate
    /// </summary>
    int Count(Expression<Func<T, bool>> predicate);

    // Write operations
    /// <summary>
    /// Adds a new entity
    /// </summary>
    void Add(T entity);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    void AddRange(IEnumerable<T> entities);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Removes an entity
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Removes multiple entities
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Saves all changes to the underlying data store
    /// </summary>
    void SaveChanges();

    /// <summary>
    /// Clears all entities (useful for testing)
    /// </summary>
    void Clear();
}

using System.Linq.Expressions;
using BudgetTracker.Core.Interfaces;

namespace BudgetTracker.Data;

/// <summary>
/// In-memory implementation of IRepository
/// Stores data in memory using List<T>
/// Useful for development, testing, and prototyping
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _data;
    private int _nextId = 1;

    public InMemoryRepository()
    {
        _data = new List<T>();
    }

    public InMemoryRepository(IEnumerable<T> seedData)
    {
        _data = new List<T>(seedData);
    }

    #region Read Operations

    public T? GetById(int id)
    {
        // Use reflection to find entity with matching Id property
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
            throw new InvalidOperationException($"Type {typeof(T).Name} does not have an Id property");

        return _data.FirstOrDefault(entity =>
        {
            var entityId = idProperty.GetValue(entity);
            return entityId != null && (int)entityId == id;
        });
    }

    public IEnumerable<T> GetAll()
    {
        return _data.ToList();
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        return _data.Where(compiledPredicate).ToList();
    }

    public T? FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        return _data.FirstOrDefault(compiledPredicate);
    }

    public bool Any(Expression<Func<T, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        return _data.Any(compiledPredicate);
    }

    public int Count()
    {
        return _data.Count;
    }

    public int Count(Expression<Func<T, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        return _data.Count(compiledPredicate);
    }

    #endregion

    #region Write Operations

    public void Add(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Auto-assign ID if entity has Id property and Id is 0
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            var currentId = idProperty.GetValue(entity);
            if (currentId != null && (int)currentId == 0)
            {
                idProperty.SetValue(entity, _nextId++);
            }
        }

        _data.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            Add(entity);
        }
    }

    public void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // In memory implementation doesn't need to do anything
        // since the entity reference is already in the list
        // Just verify it exists
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            var entityId = idProperty.GetValue(entity);
            if (entityId != null)
            {
                var existing = GetById((int)entityId);
                if (existing == null)
                    throw new InvalidOperationException($"Entity with ID {entityId} not found");
            }
        }
    }

    public void Remove(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _data.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities.ToList())
        {
            _data.Remove(entity);
        }
    }

    public void SaveChanges()
    {
        // In-memory implementation doesn't need to save
        // Data is already in memory
    }

    public void Clear()
    {
        _data.Clear();
        _nextId = 1;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the next available ID
    /// </summary>
    public int GetNextId()
    {
        return _nextId;
    }

    /// <summary>
    /// Sets the next ID to be assigned
    /// Useful for testing or when importing data
    /// </summary>
    public void SetNextId(int nextId)
    {
        _nextId = nextId;
    }

    #endregion
}

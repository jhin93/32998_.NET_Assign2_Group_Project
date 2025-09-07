using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalManagementSystem.Services
{
    // Generic repository class for data operations
    public class Repository<T> where T : class
    {
        private List<T> items = new List<T>();

        // Add item to repository
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            
            items.Add(item);
        }

        // Remove item from repository
        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        // Get all items
        public List<T> GetAll()
        {
            return new List<T>(items);
        }

        // Find items based on predicate
        public List<T> Find(Func<T, bool> predicate)
        {
            return items.Where(predicate).ToList();
        }

        // Get first item matching predicate
        public T FindFirst(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        // Check if repository contains item
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        // Get count of items
        public int Count()
        {
            return items.Count;
        }

        // Clear all items
        public void Clear()
        {
            items.Clear();
        }
    }
}
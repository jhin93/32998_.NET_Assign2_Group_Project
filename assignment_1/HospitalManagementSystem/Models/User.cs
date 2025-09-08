using System;

namespace HospitalManagementSystem.Models
{
    // Base class for all users in the system
    public abstract class User
    {
        // Properties
        public int Id { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Constructor
        protected User()
        {
            Id = Utils.GenerateId();
        }

        // Constructor with parameters (method overloading)
        protected User(string name, string email, string phone, string address, string password)
        {
            Id = Utils.GenerateId();
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            Password = password;
        }

        // Abstract method to be overridden by derived classes
        public abstract void ShowMenu();

        // Virtual method that can be overridden
        public override string ToString()
        {
            return $"{Id} | {Name} | {Email} | {Phone} | {Address}";
        }

        // Method to display user details
        public virtual void DisplayDetails()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Phone: {Phone}");
            Console.WriteLine($"Address: {Address}");
        }
    }
}
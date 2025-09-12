using System;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Base class for all users in the hospital management system
    /// </summary>
    public abstract class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the password for user authentication
        /// </summary>
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the full name of the user
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the email address of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the phone number of the user
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the physical address of the user
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the User class
        /// </summary>
        protected User()
        {
            // Do not generate ID here - it will be set when loading from file or when creating new user
        }

        /// <summary>
        /// Initializes a new instance of the User class with specified parameters
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <param name="email">The email address of the user</param>
        /// <param name="phone">The phone number of the user</param>
        /// <param name="address">The physical address of the user</param>
        /// <param name="password">The password for the user</param>
        protected User(string name, string email, string phone, string address, string password)
        {
            Id = Utils.GenerateId(); // Generate ID only for new users
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            Password = password;
        }

        /// <summary>
        /// Shows the menu specific to the user type
        /// </summary>
        public abstract void ShowMenu();

        /// <summary>
        /// Returns a string representation of the user
        /// </summary>
        /// <returns>A formatted string containing user information</returns>
        public override string ToString()
        {
            return $"{Id} | {Name} | {Email} | {Phone} | {Address}";
        }

        /// <summary>
        /// Displays the user details to the console
        /// </summary>
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
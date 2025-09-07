using System;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using HospitalManagementSystem.Extensions;

namespace HospitalManagementSystem
{
    class Program
    {
        private static NotificationService notificationService = new NotificationService();

        static void Main(string[] args)
        {
            // Register notification handlers (demonstrates delegates and anonymous methods)
            notificationService.RegisterDefaultHandlers();
            
            // Example of garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Main application loop
            bool exitApplication = false;
            
            while (!exitApplication)
            {
                try
                {
                    User currentUser = ShowLoginScreen();
                    
                    if (currentUser != null)
                    {
                        // Send login notification
                        notificationService.SendNotification($"{currentUser.Name} has logged in.");
                        
                        // Show appropriate menu based on user type
                        currentUser.ShowMenu();
                    }
                }
                catch (Exception ex)
                {
                    Utils.DisplayError($"An unexpected error occurred: {ex.Message}");
                }
            }
        }

        static User ShowLoginScreen()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("DOTNET Hospital Management System");
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("Login");
                Console.WriteLine(new string('-', 40));
                
                try
                {
                    Console.Write("\nID: ");
                    string idInput = Console.ReadLine();
                    
                    // Check for exit command
                    if (idInput.ToLower() == "exit")
                    {
                        Console.WriteLine("\nExiting application...");
                        Environment.Exit(0);
                    }
                    
                    // Validate ID using extension method
                    if (!idInput.IsValidId())
                    {
                        Console.WriteLine("\nInvalid ID format. ID must be between 5-8 digits.");
                        Console.WriteLine("Press any key to try again...");
                        Console.ReadKey();
                        continue;
                    }
                    
                    int id = int.Parse(idInput);
                    
                    Console.Write("Password: ");
                    string password = Utils.GetMaskedPassword();
                    
                    // Validate credentials
                    User user = FileManager.FindUser(id, password);
                    
                    if (user != null)
                    {
                        Console.WriteLine("\nValid Credentials");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return user;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nInvalid Credentials");
                        Console.ResetColor();
                        Console.WriteLine("Press any key to try again...");
                        Console.ReadKey();
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("\nInvalid input format. Please enter numeric ID.");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError during login: {ex.Message}");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                }
            }
        }
    }
}

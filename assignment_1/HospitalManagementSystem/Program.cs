using HospitalManagementSystem.Models;
using HospitalManagementSystem.Extensions;

namespace HospitalManagementSystem
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            // Main application loop - runs until Environment.Exit is called
            while (true)
            {
                try
                {
                    User? currentUser = ShowLoginScreen();
                    
                    if (currentUser != null)
                    {
                        // Show appropriate menu based on user type
                        currentUser.ShowMenu();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
                    Console.ResetColor();
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static User? ShowLoginScreen()
        {
            while (true)
            {
                Console.Clear();
                
                // Display header
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("DOTNET Hospital Management System");
                Console.SetCursorPosition(0, 1);
                Console.WriteLine(new string('-', 40));
                Console.SetCursorPosition(0, 2);
                Console.WriteLine("Login");
                Console.SetCursorPosition(0, 3);
                Console.WriteLine(new string('-', 40));
                
                // Display form layout
                Console.SetCursorPosition(5, 6);
                Console.Write("ID       : ");
                Console.SetCursorPosition(5, 8);
                Console.Write("Password : ");
                
                // Display instructions
                Console.SetCursorPosition(5, 11);
                Console.Write("(Type 'exit' as ID to quit)");
                
                try
                {
                    // Get ID input
                    Console.SetCursorPosition(16, 6);
                    string? idInput = Console.ReadLine();
                    
                    // Check for exit command
                    if (idInput?.ToLower() == "exit")
                    {
                        Console.SetCursorPosition(5, 13);
                        Console.WriteLine("Exiting application...");
                        Environment.Exit(0);
                    }
                    
                    // Validate ID using extension method
                    if (string.IsNullOrEmpty(idInput) || !idInput.IsValidId())
                    {
                        Console.SetCursorPosition(5, 13);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid ID format. ID must be between 5-8 digits.");
                        Console.ResetColor();
                        Console.SetCursorPosition(5, 14);
                        Console.WriteLine("Press any key to try again...");
                        Console.ReadKey();
                        continue;
                    }
                    
                    int id = int.Parse(idInput);
                    
                    // Get Password input
                    Console.SetCursorPosition(16, 8);
                    string password = Utils.GetMaskedPassword();
                    
                    // Validate credentials
                    User? user = FileManager.FindUser(id, password);
                    
                    if (user != null)
                    {
                        Console.SetCursorPosition(5, 13);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Valid Credentials");
                        Console.ResetColor();
                        Console.SetCursorPosition(5, 14);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return user;
                    }
                    else
                    {
                        Console.SetCursorPosition(5, 13);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid Credentials");
                        Console.ResetColor();
                        Console.SetCursorPosition(5, 14);
                        Console.WriteLine("Press any key to try again...");
                        Console.ReadKey();
                    }
                }
                catch (FormatException)
                {
                    Console.SetCursorPosition(5, 13);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input format. Please enter numeric ID.");
                    Console.ResetColor();
                    Console.SetCursorPosition(5, 14);
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.SetCursorPosition(5, 13);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error during login: {ex.Message}");
                    Console.ResetColor();
                    Console.SetCursorPosition(5, 14);
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                }
            }
        }
    }
}

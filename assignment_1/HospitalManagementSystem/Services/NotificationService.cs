using System;

namespace HospitalManagementSystem.Services
{
    // Delegate for notifications
    public delegate void NotificationHandler(string message);

    // Service for handling notifications (demonstrates delegates and anonymous methods)
    public class NotificationService
    {
        // Event using the delegate
        public event NotificationHandler? OnNotification;

        // Method to send notification
        public void SendNotification(string message)
        {
            OnNotification?.Invoke(message);
        }

        // Example of using anonymous method
        public void RegisterDefaultHandlers()
        {
            // Anonymous method example
            OnNotification += delegate(string msg)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Notification]: {msg}");
                Console.ResetColor();
            };
        }

        // Example of lambda expression (another form of anonymous method)
        public void RegisterEmailHandler()
        {
            OnNotification += (msg) =>
            {
                // Simulate email sending
                Console.WriteLine($"Email sent: {msg}");
            };
        }
    }
}
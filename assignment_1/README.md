32998 Hospital Management System
14657314 Jinkyung Kim
==================================

Assignment 1 - Spring 2025
31927 – Application Development with .NET
32998 - .NET Application Development

=========
HOW TO RUN THE APPLICATION
=========

1. Visual Studio Community Edition 2022:
   - Open the HospitalManagementSystem.sln file in Visual Studio 2022
   - Press F5 or click "Start" to run the application
   - The console application will start

2. Command Line (.NET CLI):
   - Navigate to the HospitalManagementSystem folder
   - Run: dotnet build
   - Run: dotnet run
   
=========
TEST CREDENTIALS
=========

PATIENTS:
- ID: 26685, Password: sam123 (Samuel Jackson)
- ID: 48775, Password: jem123 (Jeremy Irons)
- ID: 86559, Password: emily456 (Emily Blont)
- ID: 44658, Password: tom789 (Tom Holand)

DOCTORS:
- ID: 96588, Password: doc123 (Dr. Faust Lichtenstein)
- ID: 45675, Password: pass456 (Dr. Sam Smith)
- ID: 54665, Password: jack789 (Dr. Michael Jackson)

ADMINISTRATORS:
- ID: 12312, Password: mat123 (Matt Admin)
- ID: 15442, Password: sus456 (Susan Manager)

=========
FEATURES IMPLEMENTED
=========

1. OOP Principles:
   - Inheritance (User base class)
   - Method overloading (constructors)
   - Method overriding (ShowMenu, ToString)
   - Extension methods (StringExtensions)
   - Constructors
   - Garbage collection

2. Advanced Features:
   - Delegates (NotificationHandler)
   - Anonymous methods (notification handlers)
   - Generics (Repository<T>)
   - Exception handling throughout
   - File I/O operations

3. Functionality:
   - Login system with password masking
   - Patient menu with all required options
   - Doctor menu with all required options
   - Administrator menu with all required options
   - Data persistence using .txt files
   - Appointment booking system

=========
PROJECT STRUCTURE
=========

/HospitalManagementSystem
  /Models
    - User.cs (base class)
    - Patient.cs
    - Doctor.cs
    - Administrator.cs
    - Appointment.cs
  /Services
    - FileManager.cs
    - Utils.cs
    - NotificationService.cs
    - Repository.cs
  /Extensions
    - StringExtensions.cs
  /Data
    - patients.txt
    - doctors.txt
    - admins.txt
    - appointments.txt
    - usedIds.txt
  - Program.cs (main entry point)

=========
NOTES
=========

- The application uses console-based menus
- All data is stored in .txt files in the Data folder
- IDs are automatically generated (5-8 digits)
- Password input is masked with asterisks
- The application includes comprehensive error handling

using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalManagementSystem.Models
{
    // Administrator class inheriting from User
    public class Administrator : User
    {
        // Default constructor
        public Administrator() : base()
        {
        }

        // Parameterized constructor
        public Administrator(string name, string email, string phone, string address, string password) 
            : base(name, email, phone, address, password)
        {
        }

        // Override ToString method
        public override string ToString()
        {
            return $"{Name} | {Email} | {Phone} | {Address}";
        }

        // Override ShowMenu method
        public override void ShowMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                DisplayMenuHeader("Administrator Menu");
                Console.WriteLine($"Welcome to DOTNET Hospital Management System {Name}\n");
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. List all doctors");
                Console.WriteLine("2. Check doctor details");
                Console.WriteLine("3. List all patients");
                Console.WriteLine("4. Check patient details");
                Console.WriteLine("5. Add doctor");
                Console.WriteLine("6. Add patient");
                Console.WriteLine("7. Logout");
                Console.WriteLine("8. Exit");
                Console.Write("\nChoice: ");

                try
                {
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            ListAllDoctors();
                            break;
                        case "2":
                            CheckDoctorDetails();
                            break;
                        case "3":
                            ListAllPatients();
                            break;
                        case "4":
                            CheckPatientDetails();
                            break;
                        case "5":
                            AddDoctor();
                            break;
                        case "6":
                            AddPatient();
                            break;
                        case "7":
                            exit = true;
                            break;
                        case "8":
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("\nInvalid option. Please try again.");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.ReadKey();
                }
            }
        }

        // Administrator specific methods
        private void ListAllDoctors()
        {
            Console.Clear();
            DisplayMenuHeader("All Doctors");
            
            var doctors = FileManager.LoadDoctors();
            
            if (doctors.Count == 0)
            {
                Console.WriteLine("\nNo doctors registered in the system.");
            }
            else
            {
                Console.WriteLine($"\nAll doctors registered to the DOTNET Hospital Management System\n");
                Console.WriteLine($"{"Name",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                Console.WriteLine(new string('-', 100));
                
                foreach (var doctor in doctors)
                {
                    Console.WriteLine($"{doctor.Name,-20} | {doctor.Email,-30} | {doctor.Phone,-15} | {doctor.Address,-30}");
                }
            }
            
            Console.WriteLine("\nPress any key to return to Administrator Menu");
            Console.ReadKey();
        }

        private void CheckDoctorDetails()
        {
            Console.Clear();
            DisplayMenuHeader("Doctor Details");
            
            Console.Write("\nPlease enter the ID of the doctor who's details you are checking. Or press n to return to menu: ");
            string input = Console.ReadLine();
            
            if (input.ToLower() == "n")
            {
                return;
            }
            
            if (int.TryParse(input, out int doctorId))
            {
                var doctors = FileManager.LoadDoctors();
                var doctor = doctors.FirstOrDefault(d => d.Id == doctorId);
                
                if (doctor != null)
                {
                    Console.WriteLine($"\nDetails for Dr. {doctor.Name}\n");
                    Console.WriteLine($"{"Name",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine($"{doctor.Name,-20} | {doctor.Email,-30} | {doctor.Phone,-15} | {doctor.Address,-30}");
                }
                else
                {
                    Console.WriteLine($"\nNo doctor found with ID: {doctorId}");
                }
            }
            else
            {
                Console.WriteLine("\nInvalid ID format.");
            }
            
            Console.WriteLine("\nPress any key to return to Administrator Menu");
            Console.ReadKey();
        }

        private void ListAllPatients()
        {
            Console.Clear();
            DisplayMenuHeader("All Patients");
            
            var patients = FileManager.LoadPatients();
            
            if (patients.Count == 0)
            {
                Console.WriteLine("\nNo patients registered in the system.");
            }
            else
            {
                Console.WriteLine($"\nAll patients registered to the DOTNET Hospital Management System\n");
                Console.WriteLine($"{"Patient",-20} | {"Doctor",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                Console.WriteLine(new string('-', 120));
                
                var doctors = FileManager.LoadDoctors();
                foreach (var patient in patients)
                {
                    var doctor = doctors.FirstOrDefault(d => d.Id == patient.RegisteredDoctorId);
                    string doctorName = doctor != null ? doctor.Name : "Not assigned";
                    Console.WriteLine($"{patient.Name,-20} | {doctorName,-20} | {patient.Email,-30} | {patient.Phone,-15} | {patient.Address,-30}");
                }
            }
            
            Console.WriteLine("\nPress any key to return to Administrator Menu");
            Console.ReadKey();
        }

        private void CheckPatientDetails()
        {
            Console.Clear();
            DisplayMenuHeader("Patient Details");
            
            Console.Write("\nPlease enter the ID of the patient who's details you are checking. Or press n to return to menu: ");
            string input = Console.ReadLine();
            
            if (input.ToLower() == "n")
            {
                return;
            }
            
            if (int.TryParse(input, out int patientId))
            {
                var patients = FileManager.LoadPatients();
                var patient = patients.FirstOrDefault(p => p.Id == patientId);
                
                if (patient != null)
                {
                    Console.WriteLine($"\nDetails for {patient.Name}\n");
                    
                    var doctors = FileManager.LoadDoctors();
                    var doctor = doctors.FirstOrDefault(d => d.Id == patient.RegisteredDoctorId);
                    string doctorName = doctor != null ? doctor.Name : "Not assigned";
                    
                    Console.WriteLine($"{"Patient",-20} | {"Doctor",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                    Console.WriteLine(new string('-', 120));
                    Console.WriteLine($"{patient.Name,-20} | {doctorName,-20} | {patient.Email,-30} | {patient.Phone,-15} | {patient.Address,-30}");
                }
                else
                {
                    Console.WriteLine($"\nNo patient found with ID: {patientId}");
                }
            }
            else
            {
                Console.WriteLine("\nInvalid ID format.");
            }
            
            Console.WriteLine("\nPress any key to return to Administrator Menu");
            Console.ReadKey();
        }

        private void AddDoctor()
        {
            Console.Clear();
            DisplayMenuHeader("Add Doctor");
            
            Console.WriteLine("\nRegistering a new doctor with the DOTNET Hospital Management System");
            
            try
            {
                Console.Write("First Name: ");
                string firstName = Console.ReadLine();
                
                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();
                
                string fullName = $"{firstName} {lastName}";
                
                Console.Write("Email: ");
                string email = Console.ReadLine();
                
                Console.Write("Phone: ");
                string phone = Console.ReadLine();
                
                Console.Write("Street Number: ");
                string streetNumber = Console.ReadLine();
                
                Console.Write("Street: ");
                string street = Console.ReadLine();
                
                Console.Write("City: ");
                string city = Console.ReadLine();
                
                Console.Write("State: ");
                string state = Console.ReadLine();
                
                string address = $"{streetNumber} {street}, {city}, {state}";
                
                Console.Write("Password: ");
                string password = Console.ReadLine();
                
                // Validate input
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || 
                    string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address) || 
                    string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("\nAll fields are required.");
                    Console.ReadKey();
                    return;
                }
                
                // Create new doctor
                var doctor = new Doctor(fullName, email, phone, address, password);
                
                // Save to file
                var doctors = FileManager.LoadDoctors();
                doctors.Add(doctor);
                FileManager.SaveDoctors(doctors);
                
                Console.WriteLine($"\nDr. {fullName} added to the system!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError adding doctor: {ex.Message}");
            }
            
            Console.WriteLine("\nPress any key to return to Administrator Menu");
            Console.ReadKey();
        }

        private void AddPatient()
        {
            Console.Clear();
            DisplayMenuHeader("Add Patient");
            
            Console.WriteLine("\nRegistering a new patient with the DOTNET Hospital Management System");
            
            try
            {
                Console.Write("First Name: ");
                string firstName = Console.ReadLine();
                
                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();
                
                string fullName = $"{firstName} {lastName}";
                
                Console.Write("Email: ");
                string email = Console.ReadLine();
                
                Console.Write("Phone: ");
                string phone = Console.ReadLine();
                
                Console.Write("Street Number: ");
                string streetNumber = Console.ReadLine();
                
                Console.Write("Street: ");
                string street = Console.ReadLine();
                
                Console.Write("City: ");
                string city = Console.ReadLine();
                
                Console.Write("State: ");
                string state = Console.ReadLine();
                
                string address = $"{streetNumber} {street}, {city}, {state}";
                
                Console.Write("Password: ");
                string password = Console.ReadLine();
                
                // Validate input
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || 
                    string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address) || 
                    string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("\nAll fields are required.");
                    Console.ReadKey();
                    return;
                }
                
                // Create new patient
                var patient = new Patient(fullName, email, phone, address, password);
                
                // Save to file
                var patients = FileManager.LoadPatients();
                patients.Add(patient);
                FileManager.SavePatients(patients);
                
                Console.WriteLine($"\n{fullName} added to the system!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError adding patient: {ex.Message}");
            }
            
            Console.WriteLine("\nPress any key to return to Administrator Menu");
            Console.ReadKey();
        }

        private void DisplayMenuHeader(string title)
        {
            Console.WriteLine("DOTNET Hospital Management System");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine(title);
            Console.WriteLine(new string('-', 40));
        }
    }
}
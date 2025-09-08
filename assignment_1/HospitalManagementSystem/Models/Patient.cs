using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalManagementSystem.Models
{
    // Patient class inheriting from User
    public class Patient : User
    {
        // Properties specific to Patient
        public int? RegisteredDoctorId { get; set; }

        // Default constructor
        public Patient() : base()
        {
        }

        // Parameterized constructor
        public Patient(string name, string email, string phone, string address, string password) 
            : base(name, email, phone, address, password)
        {
        }

        // Override ToString method
        public override string ToString()
        {
            string doctorInfo = RegisteredDoctorId.HasValue ? RegisteredDoctorId.Value.ToString() : "None";
            return $"{Name} | {Id} | {Email} | {Phone} | {Address}";
        }

        // Override ShowMenu method
        public override void ShowMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                DisplayMenuHeader("Patient Menu");
                Console.WriteLine($"Welcome to DOTNET Hospital Management System {Name}\n");
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. List Patient Details");
                Console.WriteLine("2. List my doctor details");
                Console.WriteLine("3. List all appointments");
                Console.WriteLine("4. Book appointment");
                Console.WriteLine("5. Exit to login");
                Console.WriteLine("6. Exit System");
                Console.Write("\nChoice: ");

                try
                {
                    string choice = Utils.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            ListPatientDetails();
                            break;
                        case "2":
                            ListMyDoctorDetails();
                            break;
                        case "3":
                            ListAllAppointments();
                            break;
                        case "4":
                            BookAppointment();
                            break;
                        case "5":
                            exit = true;
                            break;
                        case "6":
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

        // Patient specific methods
        private void ListPatientDetails()
        {
            Console.Clear();
            DisplayMenuHeader("My Details");
            Console.WriteLine($"\n{Name}'s Details\n");
            Console.WriteLine($"Patient ID: {Id}");
            Console.WriteLine($"Full Name: {Name}");
            Console.WriteLine($"Address: {Address}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Phone: {Phone}");
            Console.WriteLine("\nPress any key to return to Patient Menu");
            Console.ReadKey();
        }

        private void ListMyDoctorDetails()
        {
            Console.Clear();
            DisplayMenuHeader("My Doctor");
            
            if (!RegisteredDoctorId.HasValue)
            {
                Console.WriteLine("\nYou are not registered with any doctor yet.");
            }
            else
            {
                var doctors = FileManager.LoadDoctors();
                var myDoctor = doctors.FirstOrDefault(d => d.Id == RegisteredDoctorId.Value);
                
                if (myDoctor != null)
                {
                    Console.WriteLine("\nYour doctor:\n");
                    Console.WriteLine($"{"Name",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine($"{myDoctor.Name,-20} | {myDoctor.Email,-30} | {myDoctor.Phone,-15} | {myDoctor.Address,-30}");
                }
                else
                {
                    Console.WriteLine("\nDoctor information not found.");
                }
            }
            
            Console.WriteLine("\nPress any key to return to Patient Menu");
            Console.ReadKey();
        }

        private void ListAllAppointments()
        {
            Console.Clear();
            DisplayMenuHeader("My Appointments");
            
            var appointments = FileManager.LoadAppointments();
            var myAppointments = appointments.Where(a => a.PatientId == Id).ToList();
            
            if (myAppointments.Count == 0)
            {
                Console.WriteLine("\nYou have no appointments.");
            }
            else
            {
                Console.WriteLine($"\nAppointments for {Name}\n");
                Console.WriteLine($"{"Doctor",-20} | {"Patient",-20} | {"Description",-40}");
                Console.WriteLine(new string('-', 85));
                
                var doctors = FileManager.LoadDoctors();
                foreach (var appointment in myAppointments)
                {
                    var doctor = doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
                    string doctorName = doctor != null ? doctor.Name : "Unknown";
                    Console.WriteLine($"{doctorName,-20} | {Name,-20} | {appointment.Description,-40}");
                }
            }
            
            Console.WriteLine("\nPress any key to return to Patient Menu");
            Console.ReadKey();
        }

        private void BookAppointment()
        {
            Console.Clear();
            DisplayMenuHeader("Book Appointment");
            
            try
            {
                // Check if patient has a registered doctor
                if (!RegisteredDoctorId.HasValue)
                {
                    Console.WriteLine("\nYou are not registered with any doctor! Please choose which doctor you would like to register with");
                    var doctors = FileManager.LoadDoctors();
                    
                    if (doctors.Count == 0)
                    {
                        Console.WriteLine("\nNo doctors available in the system.");
                        Console.ReadKey();
                        return;
                    }
                    
                    Console.WriteLine("\nAvailable Doctors:");
                    for (int i = 0; i < doctors.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. Dr. {doctors[i].Name} | Email: {doctors[i].Email} | Phone: {doctors[i].Phone}");
                    }
                    
                    Console.Write("\nPlease choose a doctor (enter number): ");
                    if (int.TryParse(Utils.ReadLine(), out int choice) && choice > 0 && choice <= doctors.Count)
                    {
                        RegisteredDoctorId = doctors[choice - 1].Id;
                        
                        // Update patient file
                        var patients = FileManager.LoadPatients();
                        var patientToUpdate = patients.FirstOrDefault(p => p.Id == Id);
                        if (patientToUpdate != null)
                        {
                            patientToUpdate.RegisteredDoctorId = RegisteredDoctorId;
                            FileManager.SavePatients(patients);
                        }
                        
                        Console.WriteLine($"\nYou are now registered with Dr. {doctors[choice - 1].Name}");
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid choice.");
                        Console.ReadKey();
                        return;
                    }
                }
                
                // Now book the appointment
                Console.WriteLine("\nYou are booking a new appointment with your registered doctor");
                Console.Write("Description of the appointment: ");
                string description = Utils.ReadLine();
                
                if (string.IsNullOrWhiteSpace(description))
                {
                    Console.WriteLine("\nDescription cannot be empty.");
                    Console.ReadKey();
                    return;
                }
                
                var appointment = new Appointment
                {
                    DoctorId = RegisteredDoctorId.Value,
                    PatientId = Id,
                    Description = description
                };
                
                // Save appointment
                var appointments = FileManager.LoadAppointments();
                appointments.Add(appointment);
                FileManager.SaveAppointments(appointments);
                
                Console.WriteLine("\nThe appointment has been booked successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError booking appointment: {ex.Message}");
            }
            
            Console.WriteLine("\nPress any key to return to Patient Menu");
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
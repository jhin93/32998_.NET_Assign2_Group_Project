namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a patient in the hospital management system
    /// </summary>
    public class Patient : User
    {
        /// <summary>
        /// Gets or sets the ID of the doctor the patient is registered with
        /// </summary>
        public int? RegisteredDoctorId { get; set; }

        /// <summary>
        /// Initializes a new instance of the Patient class
        /// </summary>
        public Patient() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Patient class with specified parameters
        /// </summary>
        /// <param name="name">The name of the patient</param>
        /// <param name="email">The email address of the patient</param>
        /// <param name="phone">The phone number of the patient</param>
        /// <param name="address">The physical address of the patient</param>
        /// <param name="password">The password for the patient</param>
        public Patient(string name, string email, string phone, string address, string password) 
            : base(name, email, phone, address, password)
        {
        }

        /// <summary>
        /// Returns a string representation of the patient
        /// </summary>
        /// <returns>A formatted string containing patient information</returns>
        public override string ToString()
        {
            string doctorInfo = RegisteredDoctorId.HasValue ? RegisteredDoctorId.Value.ToString() : "None";
            return $"{Name} | {Id} | {Email} | {Phone} | {Address}";
        }

        /// <summary>
        /// Shows the patient menu with available options
        /// </summary>
        public override void ShowMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                DisplayMenuHeader("Patient Menu");
                Console.WriteLine($"Welcome to Hospital Management System {Name}\n");
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
                // Patient can only register with a doctor once - cannot change later
                if (!RegisteredDoctorId.HasValue)
                {
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine("You are not registered with any doctor!");
                    Console.SetCursorPosition(0, 6);
                    Console.WriteLine("Please choose which doctor you would like to register with:");
                    
                    var doctors = FileManager.LoadDoctors();
                    
                    if (doctors.Count == 0)
                    {
                        Console.SetCursorPosition(5, 8);
                        Console.WriteLine("No doctors available in the system.");
                        Console.SetCursorPosition(5, 10);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        return;
                    }
                    
                    Console.SetCursorPosition(0, 8);
                    Console.WriteLine("Available Doctors:");
                    for (int i = 0; i < doctors.Count; i++)
                    {
                        Console.SetCursorPosition(5, 10 + i);
                        Console.WriteLine($"{i + 1}. Dr. {doctors[i].Name} | Email: {doctors[i].Email} | Phone: {doctors[i].Phone}");
                    }
                    
                    Console.SetCursorPosition(5, 11 + doctors.Count);
                    Console.Write("Please choose a doctor (enter number): ");
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
                        
                        Console.SetCursorPosition(5, 13 + doctors.Count);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"You are now registered with Dr. {doctors[choice - 1].Name}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.SetCursorPosition(5, 13 + doctors.Count);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice.");
                        Console.ResetColor();
                        Console.SetCursorPosition(5, 14 + doctors.Count);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        return;
                    }
                }
                
                // Now book the appointment - using form layout
                Console.Clear();
                DisplayMenuHeader("Book Appointment");
                Console.SetCursorPosition(0, 5);
                Console.WriteLine("You are booking a new appointment with your registered doctor");
                
                Console.SetCursorPosition(5, 8);
                Console.Write("Description of the appointment: ");
                Console.SetCursorPosition(5, 10);
                Console.Write("Enter details: ");
                Console.SetCursorPosition(20, 10);
                string description = Utils.ReadLine();
                
                if (string.IsNullOrWhiteSpace(description))
                {
                    Console.SetCursorPosition(5, 13);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Description cannot be empty.");
                    Console.ResetColor();
                    Console.SetCursorPosition(5, 14);
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }
                
                // Use parameterized constructor to generate new ID
                var appointment = new Appointment(RegisteredDoctorId.Value, Id, description);
                
                // Save appointment
                var appointments = FileManager.LoadAppointments();
                appointments.Add(appointment);
                FileManager.SaveAppointments(appointments);
                
                Console.SetCursorPosition(5, 13);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("The appointment has been booked successfully");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(5, 13);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error booking appointment: {ex.Message}");
                Console.ResetColor();
            }
            
            Console.SetCursorPosition(5, 15);
            Console.WriteLine("Press any key to return to Patient Menu");
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
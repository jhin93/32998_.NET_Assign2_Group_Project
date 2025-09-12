namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a doctor in the hospital management system
    /// </summary>
    public class Doctor : User
    {
        /// <summary>
        /// Initializes a new instance of the Doctor class
        /// </summary>
        public Doctor() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Doctor class with specified parameters
        /// </summary>
        /// <param name="name">The name of the doctor</param>
        /// <param name="email">The email address of the doctor</param>
        /// <param name="phone">The phone number of the doctor</param>
        /// <param name="address">The physical address of the doctor</param>
        /// <param name="password">The password for the doctor</param>
        public Doctor(string name, string email, string phone, string address, string password) 
            : base(name, email, phone, address, password)
        {
        }

        /// <summary>
        /// Returns a string representation of the doctor
        /// </summary>
        /// <returns>A formatted string containing doctor information</returns>
        public override string ToString()
        {
            return $"{Name} | {Email} | {Phone} | {Address}";
        }

        /// <summary>
        /// Shows the doctor menu with available options
        /// </summary>
        public override void ShowMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                DisplayMenuHeader("Doctor Menu");
                Console.WriteLine($"Welcome to Hospital Management System Dr. {Name}\n");
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. List doctor details");
                Console.WriteLine("2. List patients");
                Console.WriteLine("3. List appointments");
                Console.WriteLine("4. Check particular patient");
                Console.WriteLine("5. List appointments with patient");
                Console.WriteLine("6. Logout");
                Console.WriteLine("7. Exit");
                Console.Write("\nChoice: ");

                try
                {
                    string choice = Utils.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            ListDoctorDetails();
                            break;
                        case "2":
                            ListPatients();
                            break;
                        case "3":
                            ListAppointments();
                            break;
                        case "4":
                            CheckParticularPatient();
                            break;
                        case "5":
                            ListAppointmentsWithPatient();
                            break;
                        case "6":
                            exit = true;
                            break;
                        case "7":
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

        // Doctor specific methods
        private void ListDoctorDetails()
        {
            Console.Clear();
            DisplayMenuHeader("My Details");
            Console.WriteLine($"\n{"Name",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
            Console.WriteLine(new string('-', 100));
            Console.WriteLine($"{Name,-20} | {Email,-30} | {Phone,-15} | {Address,-30}");
            Console.WriteLine("\nPress any key to return to Doctor Menu");
            Console.ReadKey();
        }

        private void ListPatients()
        {
            Console.Clear();
            DisplayMenuHeader("My Patients");
            
            var patients = FileManager.LoadPatients();
            var myPatients = patients.Where(p => p.RegisteredDoctorId == Id).ToList();
            
            if (myPatients.Count == 0)
            {
                Console.WriteLine("\nNo patients registered to you.");
            }
            else
            {
                Console.WriteLine($"\nPatients assigned to Dr. {Name}:\n");
                Console.WriteLine($"{"Patient",-20} | {"Doctor",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                Console.WriteLine(new string('-', 120));
                
                foreach (var patient in myPatients)
                {
                    Console.WriteLine($"{patient.Name,-20} | {Name,-20} | {patient.Email,-30} | {patient.Phone,-15} | {patient.Address,-30}");
                }
            }
            
            Console.WriteLine("\nPress any key to return to Doctor Menu");
            Console.ReadKey();
        }

        private void ListAppointments()
        {
            Console.Clear();
            DisplayMenuHeader("All Appointments");
            
            var appointments = FileManager.LoadAppointments();
            var myAppointments = appointments.Where(a => a.DoctorId == Id).ToList();
            
            if (myAppointments.Count == 0)
            {
                Console.WriteLine("\nYou have no appointments.");
            }
            else
            {
                var patients = FileManager.LoadPatients();
                Console.WriteLine($"\n{"Doctor",-20} | {"Patient",-20} | {"Description",-40}");
                Console.WriteLine(new string('-', 85));
                
                foreach (var appointment in myAppointments)
                {
                    var patient = patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                    string patientName = patient != null ? patient.Name : "Unknown";
                    Console.WriteLine($"{Name,-20} | {patientName,-20} | {appointment.Description,-40}");
                }
            }
            
            Console.WriteLine("\nPress any key to return to Doctor Menu");
            Console.ReadKey();
        }

        private void CheckParticularPatient()
        {
            Console.Clear();
            DisplayMenuHeader("Check Patient Details");
            
            Console.SetCursorPosition(5, 6);
            Console.Write("Enter the ID of the patient to check: ");
            Console.SetCursorPosition(45, 6);
            if (int.TryParse(Utils.ReadLine(), out int patientId))
            {
                var patients = FileManager.LoadPatients();
                var patient = patients.FirstOrDefault(p => p.Id == patientId);
                
                if (patient != null)
                {
                    Console.SetCursorPosition(0, 9);
                    Console.WriteLine($"{"Patient",-20} | {"Doctor",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine(new string('-', 120));
                    
                    var doctors = FileManager.LoadDoctors();
                    var patientDoctor = doctors.FirstOrDefault(d => d.Id == patient.RegisteredDoctorId);
                    string doctorName = patientDoctor != null ? patientDoctor.Name : "Not assigned";
                    
                    Console.SetCursorPosition(0, 11);
                    Console.WriteLine($"{patient.Name,-20} | {doctorName,-20} | {patient.Email,-30} | {patient.Phone,-15} | {patient.Address,-30}");
                }
                else
                {
                    Console.SetCursorPosition(5, 9);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"No patient found with ID: {patientId}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.SetCursorPosition(5, 9);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid ID format.");
                Console.ResetColor();
            }
            
            Console.SetCursorPosition(5, 14);
            Console.WriteLine("Press any key to return to Doctor Menu");
            Console.ReadKey();
        }

        private void ListAppointmentsWithPatient()
        {
            Console.Clear();
            DisplayMenuHeader("Appointments With Patient");
            
            Console.SetCursorPosition(5, 6);
            Console.Write("Enter the ID of the patient: ");
            Console.SetCursorPosition(35, 6);
            if (int.TryParse(Utils.ReadLine(), out int patientId))
            {
                var patients = FileManager.LoadPatients();
                var patient = patients.FirstOrDefault(p => p.Id == patientId);
                
                if (patient != null)
                {
                    var appointments = FileManager.LoadAppointments();
                    var patientAppointments = appointments.Where(a => a.DoctorId == Id && a.PatientId == patientId).ToList();
                    
                    if (patientAppointments.Count == 0)
                    {
                        Console.SetCursorPosition(5, 9);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"No appointments found with patient ID: {patientId}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.SetCursorPosition(0, 9);
                        Console.WriteLine($"{"Doctor",-20} | {"Patient",-20} | {"Description",-40}");
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine(new string('-', 85));
                        
                        int row = 11;
                        foreach (var appointment in patientAppointments)
                        {
                            Console.SetCursorPosition(0, row);
                            Console.WriteLine($"{Name,-20} | {patient.Name,-20} | {appointment.Description,-40}");
                            row++;
                        }
                    }
                }
                else
                {
                    Console.SetCursorPosition(5, 9);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"No patient found with ID: {patientId}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.SetCursorPosition(5, 9);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid ID format.");
                Console.ResetColor();
            }
            
            Console.SetCursorPosition(5, 20);
            Console.WriteLine("Press any key to return to Doctor Menu");
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
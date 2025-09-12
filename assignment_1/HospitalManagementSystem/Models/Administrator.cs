namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents an administrator in the hospital management system
    /// </summary>
    public class Administrator : User
    {
        /// <summary>
        /// Initializes a new instance of the Administrator class
        /// </summary>
        public Administrator() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Administrator class with specified parameters
        /// </summary>
        /// <param name="name">The name of the administrator</param>
        /// <param name="email">The email address of the administrator</param>
        /// <param name="phone">The phone number of the administrator</param>
        /// <param name="address">The physical address of the administrator</param>
        /// <param name="password">The password for the administrator</param>
        public Administrator(string name, string email, string phone, string address, string password) 
            : base(name, email, phone, address, password)
        {
        }

        /// <summary>
        /// Returns a string representation of the administrator
        /// </summary>
        /// <returns>A formatted string containing administrator information</returns>
        public override string ToString()
        {
            return $"{Name} | {Email} | {Phone} | {Address}";
        }

        /// <summary>
        /// Shows the administrator menu with available options
        /// </summary>
        public override void ShowMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                DisplayMenuHeader("Administrator Menu");
                Console.WriteLine($"Welcome to Hospital Management System {Name}\n");
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
                    string choice = Utils.ReadLine();
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
            
            Console.SetCursorPosition(5, 6);
            Console.Write("Enter the ID of the doctor (or 'n' to return): ");
            Console.SetCursorPosition(55, 6);
            string input = Utils.ReadLine();
            
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
                    Console.SetCursorPosition(0, 9);
                    Console.WriteLine($"Details for Dr. {doctor.Name}");
                    Console.SetCursorPosition(0, 11);
                    Console.WriteLine($"{"Name",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                    Console.SetCursorPosition(0, 12);
                    Console.WriteLine(new string('-', 100));
                    Console.SetCursorPosition(0, 13);
                    Console.WriteLine($"{doctor.Name,-20} | {doctor.Email,-30} | {doctor.Phone,-15} | {doctor.Address,-30}");
                }
                else
                {
                    Console.SetCursorPosition(5, 9);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"No doctor found with ID: {doctorId}");
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
            
            Console.SetCursorPosition(5, 16);
            Console.WriteLine("Press any key to return to Administrator Menu");
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
            
            Console.SetCursorPosition(5, 6);
            Console.Write("Enter the ID of the patient (or 'n' to return): ");
            Console.SetCursorPosition(55, 6);
            string input = Utils.ReadLine();
            
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
                    Console.SetCursorPosition(0, 9);
                    Console.WriteLine($"Details for {patient.Name}");
                    
                    var doctors = FileManager.LoadDoctors();
                    var doctor = doctors.FirstOrDefault(d => d.Id == patient.RegisteredDoctorId);
                    string doctorName = doctor != null ? doctor.Name : "Not assigned";
                    
                    Console.SetCursorPosition(0, 11);
                    Console.WriteLine($"{"Patient",-20} | {"Doctor",-20} | {"Email Address",-30} | {"Phone",-15} | {"Address",-30}");
                    Console.SetCursorPosition(0, 12);
                    Console.WriteLine(new string('-', 120));
                    Console.SetCursorPosition(0, 13);
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
            
            Console.SetCursorPosition(5, 16);
            Console.WriteLine("Press any key to return to Administrator Menu");
            Console.ReadKey();
        }

        private void AddDoctor()
        {
            Console.Clear();
            DisplayMenuHeader("Add Doctor");
            
            Console.SetCursorPosition(0, 5);
            Console.WriteLine("Registering a new doctor with the DOTNET Hospital Management System");
            Console.SetCursorPosition(0, 6);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("(Press 'n' at any field to cancel and return to menu)");
            Console.ResetColor();
            
            // Display form layout
            Console.SetCursorPosition(5, 9);
            Console.Write("First Name     : ");
            Console.SetCursorPosition(5, 11);
            Console.Write("Last Name      : ");
            Console.SetCursorPosition(5, 13);
            Console.Write("Email          : ");
            Console.SetCursorPosition(5, 15);
            Console.Write("Phone          : ");
            Console.SetCursorPosition(5, 17);
            Console.Write("Street Number  : ");
            Console.SetCursorPosition(5, 19);
            Console.Write("Street         : ");
            Console.SetCursorPosition(5, 21);
            Console.Write("City           : ");
            Console.SetCursorPosition(5, 23);
            Console.Write("State          : ");
            Console.SetCursorPosition(5, 25);
            Console.Write("Password       : ");
            
            try
            {
                // Get inputs with cursor positioning
                Console.SetCursorPosition(22, 9);
                string firstName = Utils.ReadLine();
                if (firstName.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 11);
                string lastName = Utils.ReadLine();
                if (lastName.ToLower() == "n") return;
                
                string fullName = $"{firstName} {lastName}";
                
                Console.SetCursorPosition(22, 13);
                string email = Utils.ReadLine();
                if (email.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 15);
                string phone = Utils.ReadLine();
                if (phone.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 17);
                string streetNumber = Utils.ReadLine();
                if (streetNumber.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 19);
                string street = Utils.ReadLine();
                if (street.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 21);
                string city = Utils.ReadLine();
                if (city.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 23);
                string state = Utils.ReadLine();
                if (state.ToLower() == "n") return;
                
                string address = $"{streetNumber} {street}, {city}, {state}";
                
                Console.SetCursorPosition(22, 25);
                string password = Utils.ReadLine();
                if (password.ToLower() == "n") return;
                
                // Validate input
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || 
                    string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address) || 
                    string.IsNullOrWhiteSpace(password))
                {
                    Console.SetCursorPosition(5, 28);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("All fields are required.");
                    Console.ResetColor();
                    Console.SetCursorPosition(5, 29);
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }
                
                // Create new doctor
                var doctor = new Doctor(fullName, email, phone, address, password);
                
                // Save to file
                var doctors = FileManager.LoadDoctors();
                doctors.Add(doctor);
                FileManager.SaveDoctors(doctors);
                
                Console.SetCursorPosition(5, 28);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Dr. {fullName} added to the system!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(5, 28);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error adding doctor: {ex.Message}");
                Console.ResetColor();
            }
            
            Console.SetCursorPosition(5, 30);
            Console.WriteLine("Press any key to return to Administrator Menu");
            Console.ReadKey();
        }

        private void AddPatient()
        {
            Console.Clear();
            DisplayMenuHeader("Add Patient");
            
            Console.SetCursorPosition(0, 5);
            Console.WriteLine("Registering a new patient with the DOTNET Hospital Management System");
            Console.SetCursorPosition(0, 6);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("(Press 'n' at any field to cancel and return to menu)");
            Console.ResetColor();
            
            // Display form layout
            Console.SetCursorPosition(5, 9);
            Console.Write("First Name     : ");
            Console.SetCursorPosition(5, 11);
            Console.Write("Last Name      : ");
            Console.SetCursorPosition(5, 13);
            Console.Write("Email          : ");
            Console.SetCursorPosition(5, 15);
            Console.Write("Phone          : ");
            Console.SetCursorPosition(5, 17);
            Console.Write("Street Number  : ");
            Console.SetCursorPosition(5, 19);
            Console.Write("Street         : ");
            Console.SetCursorPosition(5, 21);
            Console.Write("City           : ");
            Console.SetCursorPosition(5, 23);
            Console.Write("State          : ");
            Console.SetCursorPosition(5, 25);
            Console.Write("Password       : ");
            
            try
            {
                // Get inputs with cursor positioning
                Console.SetCursorPosition(22, 9);
                string firstName = Utils.ReadLine();
                if (firstName.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 11);
                string lastName = Utils.ReadLine();
                if (lastName.ToLower() == "n") return;
                
                string fullName = $"{firstName} {lastName}";
                
                Console.SetCursorPosition(22, 13);
                string email = Utils.ReadLine();
                if (email.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 15);
                string phone = Utils.ReadLine();
                if (phone.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 17);
                string streetNumber = Utils.ReadLine();
                if (streetNumber.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 19);
                string street = Utils.ReadLine();
                if (street.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 21);
                string city = Utils.ReadLine();
                if (city.ToLower() == "n") return;
                
                Console.SetCursorPosition(22, 23);
                string state = Utils.ReadLine();
                if (state.ToLower() == "n") return;
                
                string address = $"{streetNumber} {street}, {city}, {state}";
                
                Console.SetCursorPosition(22, 25);
                string password = Utils.ReadLine();
                if (password.ToLower() == "n") return;
                
                // Validate input
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || 
                    string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address) || 
                    string.IsNullOrWhiteSpace(password))
                {
                    Console.SetCursorPosition(5, 28);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("All fields are required.");
                    Console.ResetColor();
                    Console.SetCursorPosition(5, 29);
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }
                
                // Create new patient
                var patient = new Patient(fullName, email, phone, address, password);
                
                // Save to file
                var patients = FileManager.LoadPatients();
                patients.Add(patient);
                FileManager.SavePatients(patients);
                
                Console.SetCursorPosition(5, 28);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{fullName} added to the system!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(5, 28);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error adding patient: {ex.Message}");
                Console.ResetColor();
            }
            
            Console.SetCursorPosition(5, 30);
            Console.WriteLine("Press any key to return to Administrator Menu");
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
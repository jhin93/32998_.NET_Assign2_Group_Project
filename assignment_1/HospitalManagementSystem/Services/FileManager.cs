using HospitalManagementSystem.Models;

namespace HospitalManagementSystem
{
    /// <summary>
    /// Manages file operations for persisting data
    /// </summary>
    public static class FileManager
    {
        private static readonly string DataDirectory = "Data";
        private static readonly string PatientsFile = Path.Combine(DataDirectory, "patients.txt");
        private static readonly string DoctorsFile = Path.Combine(DataDirectory, "doctors.txt");
        private static readonly string AdminsFile = Path.Combine(DataDirectory, "admins.txt");
        private static readonly string AppointmentsFile = Path.Combine(DataDirectory, "appointments.txt");

        /// <summary>
        /// Static constructor to ensure data directory exists
        /// </summary>
        static FileManager()
        {
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }
        }

        /// <summary>
        /// Loads all patients from the data file
        /// </summary>
        /// <returns>A list of patients</returns>
        public static List<Patient> LoadPatients()
        {
            var patients = new List<Patient>();
            
            try
            {
                if (File.Exists(PatientsFile))
                {
                    var lines = File.ReadAllLines(PatientsFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length >= 6)
                            {
                                var patient = new Patient
                                {
                                    Id = int.Parse(parts[0]),
                                    Name = parts[1],
                                    Email = parts[2],
                                    Phone = parts[3],
                                    Address = parts[4],
                                    Password = parts[5]
                                };
                                
                                if (parts.Length >= 7 && !string.IsNullOrWhiteSpace(parts[6]))
                                {
                                    patient.RegisteredDoctorId = int.Parse(parts[6]);
                                }
                                
                                patients.Add(patient);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading patients: {ex.Message}");
            }
            
            return patients;
        }

        /// <summary>
        /// Saves patients to the data file
        /// </summary>
        /// <param name="patients">The list of patients to save</param>
        public static void SavePatients(List<Patient> patients)
        {
            try
            {
                var lines = new List<string>();
                foreach (var patient in patients)
                {
                    string doctorId = patient.RegisteredDoctorId.HasValue ? patient.RegisteredDoctorId.Value.ToString() : "";
                    lines.Add($"{patient.Id}|{patient.Name}|{patient.Email}|{patient.Phone}|{patient.Address}|{patient.Password}|{doctorId}");
                }
                File.WriteAllLines(PatientsFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving patients: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads all doctors from the data file
        /// </summary>
        /// <returns>A list of doctors</returns>
        public static List<Doctor> LoadDoctors()
        {
            var doctors = new List<Doctor>();
            
            try
            {
                if (File.Exists(DoctorsFile))
                {
                    var lines = File.ReadAllLines(DoctorsFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length >= 6)
                            {
                                var doctor = new Doctor
                                {
                                    Id = int.Parse(parts[0]),
                                    Name = parts[1],
                                    Email = parts[2],
                                    Phone = parts[3],
                                    Address = parts[4],
                                    Password = parts[5]
                                };
                                doctors.Add(doctor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading doctors: {ex.Message}");
            }
            
            return doctors;
        }

        /// <summary>
        /// Saves doctors to the data file
        /// </summary>
        /// <param name="doctors">The list of doctors to save</param>
        public static void SaveDoctors(List<Doctor> doctors)
        {
            try
            {
                var lines = new List<string>();
                foreach (var doctor in doctors)
                {
                    lines.Add($"{doctor.Id}|{doctor.Name}|{doctor.Email}|{doctor.Phone}|{doctor.Address}|{doctor.Password}");
                }
                File.WriteAllLines(DoctorsFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving doctors: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads all administrators from the data file
        /// </summary>
        /// <returns>A list of administrators</returns>
        public static List<Administrator> LoadAdministrators()
        {
            var admins = new List<Administrator>();
            
            try
            {
                if (File.Exists(AdminsFile))
                {
                    var lines = File.ReadAllLines(AdminsFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length >= 6)
                            {
                                var admin = new Administrator
                                {
                                    Id = int.Parse(parts[0]),
                                    Name = parts[1],
                                    Email = parts[2],
                                    Phone = parts[3],
                                    Address = parts[4],
                                    Password = parts[5]
                                };
                                admins.Add(admin);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading administrators: {ex.Message}");
            }
            
            return admins;
        }

        /// <summary>
        /// Saves administrators to the data file
        /// </summary>
        /// <param name="admins">The list of administrators to save</param>
        public static void SaveAdministrators(List<Administrator> admins)
        {
            try
            {
                var lines = new List<string>();
                foreach (var admin in admins)
                {
                    lines.Add($"{admin.Id}|{admin.Name}|{admin.Email}|{admin.Phone}|{admin.Address}|{admin.Password}");
                }
                File.WriteAllLines(AdminsFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving administrators: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads all appointments from the data file
        /// </summary>
        /// <returns>A list of appointments</returns>
        public static List<Appointment> LoadAppointments()
        {
            var appointments = new List<Appointment>();
            
            try
            {
                if (File.Exists(AppointmentsFile))
                {
                    var lines = File.ReadAllLines(AppointmentsFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split('|');
                            if (parts.Length >= 4)
                            {
                                var appointment = new Appointment
                                {
                                    Id = int.Parse(parts[0]),
                                    DoctorId = int.Parse(parts[1]),
                                    PatientId = int.Parse(parts[2]),
                                    Description = parts[3]
                                };
                                appointments.Add(appointment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
            }
            
            return appointments;
        }

        /// <summary>
        /// Saves appointments to the data file
        /// </summary>
        /// <param name="appointments">The list of appointments to save</param>
        public static void SaveAppointments(List<Appointment> appointments)
        {
            try
            {
                var lines = new List<string>();
                foreach (var appointment in appointments)
                {
                    lines.Add($"{appointment.Id}|{appointment.DoctorId}|{appointment.PatientId}|{appointment.Description}");
                }
                File.WriteAllLines(AppointmentsFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving appointments: {ex.Message}");
            }
        }

        /// <summary>
        /// Finds a user by ID and password for authentication
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <param name="password">The user password</param>
        /// <returns>The user if found and authenticated, null otherwise</returns>
        public static User? FindUser(int id, string password)
        {
            // Check patients
            var patients = LoadPatients();
            var patient = patients.FirstOrDefault(p => p.Id == id && p.Password == password);
            if (patient != null) return patient;

            // Check doctors
            var doctors = LoadDoctors();
            var doctor = doctors.FirstOrDefault(d => d.Id == id && d.Password == password);
            if (doctor != null) return doctor;

            // Check administrators
            var admins = LoadAdministrators();
            var admin = admins.FirstOrDefault(a => a.Id == id && a.Password == password);
            if (admin != null) return admin;

            return null;
        }
    }
}
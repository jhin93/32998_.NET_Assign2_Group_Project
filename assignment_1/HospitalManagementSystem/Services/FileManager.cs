using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem
{
    // Static class for managing file operations
    public static class FileManager
    {
        private static readonly string DataDirectory = "Data";
        private static readonly string PatientsFile = Path.Combine(DataDirectory, "patients.txt");
        private static readonly string DoctorsFile = Path.Combine(DataDirectory, "doctors.txt");
        private static readonly string AdminsFile = Path.Combine(DataDirectory, "admins.txt");
        private static readonly string AppointmentsFile = Path.Combine(DataDirectory, "appointments.txt");

        // Static constructor to ensure data directory exists
        static FileManager()
        {
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }
        }

        // Load Patients from file
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

        // Save Patients to file
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

        // Load Doctors from file
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

        // Save Doctors to file
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

        // Load Administrators from file
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

        // Save Administrators to file
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

        // Load Appointments from file
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

        // Save Appointments to file
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

        // Find user by ID and password (for login)
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
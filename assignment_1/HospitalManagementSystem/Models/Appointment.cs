using System;

namespace HospitalManagementSystem.Models
{
    // Appointment class to manage appointments between doctors and patients
    public class Appointment
    {
        // Properties
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string Description { get; set; } = string.Empty;

        // Default constructor
        public Appointment()
        {
            // Do not generate ID here - it will be set when loading from file
        }

        // Parameterized constructor (method overloading) - for new appointments only
        public Appointment(int doctorId, int patientId, string description)
        {
            Id = Utils.GenerateId(); // Generate ID only for new appointments
            DoctorId = doctorId;
            PatientId = patientId;
            Description = description;
        }

        // ToString method for displaying appointment information
        public override string ToString()
        {
            return $"{Id} | Doctor: {DoctorId} | Patient: {PatientId} | {Description}";
        }
    }
}
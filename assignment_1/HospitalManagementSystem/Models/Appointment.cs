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
            Id = Utils.GenerateId();
        }

        // Parameterized constructor (method overloading)
        public Appointment(int doctorId, int patientId, string description)
        {
            Id = Utils.GenerateId();
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
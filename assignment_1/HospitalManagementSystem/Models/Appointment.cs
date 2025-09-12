using System;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents an appointment between a doctor and patient
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// Gets or sets the unique identifier for the appointment
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the doctor for this appointment
        /// </summary>
        public int DoctorId { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the patient for this appointment
        /// </summary>
        public int PatientId { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the appointment
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the Appointment class
        /// </summary>
        public Appointment()
        {
            // Do not generate ID here - it will be set when loading from file
        }

        /// <summary>
        /// Initializes a new instance of the Appointment class with specified parameters
        /// </summary>
        /// <param name="doctorId">The ID of the doctor</param>
        /// <param name="patientId">The ID of the patient</param>
        /// <param name="description">The description of the appointment</param>
        public Appointment(int doctorId, int patientId, string description)
        {
            Id = Utils.GenerateId(); // Generate ID only for new appointments
            DoctorId = doctorId;
            PatientId = patientId;
            Description = description;
        }

        /// <summary>
        /// Returns a string representation of the appointment
        /// </summary>
        /// <returns>A formatted string containing appointment information</returns>
        public override string ToString()
        {
            return $"{Id} | Doctor: {DoctorId} | Patient: {PatientId} | {Description}";
        }
    }
}
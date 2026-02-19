using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AfyaConnectLite.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        // Legacy integer role column (kept for DB compatibility)
        public int Role { get; set; } = 0;

        // Healthcare specific fields
        [StringLength(20)]
        public string? NationalId { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(500)]
        public string? MedicalHistory { get; set; }

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }

        // Navigation properties
        public virtual ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();
        public virtual DoctorProfile? DoctorProfile { get; set; }
        public virtual ICollection<ConsultationNote> ConsultationNotes { get; set; } = new List<ConsultationNote>();
    }
}

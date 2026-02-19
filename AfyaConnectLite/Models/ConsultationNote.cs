using System.ComponentModel.DataAnnotations;

namespace AfyaConnectLite.Models
{
    public class ConsultationNote
    {
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; } = string.Empty;

        [Required]
        public string DoctorId { get; set; } = string.Empty;

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Appointment Appointment { get; set; } = null!;
        public virtual ApplicationUser Doctor { get; set; } = null!;
        public virtual ApplicationUser Patient { get; set; } = null!;
    }
}

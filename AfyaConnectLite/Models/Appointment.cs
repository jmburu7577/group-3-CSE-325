using System.ComponentModel.DataAnnotations;

namespace AfyaConnectLite.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; } = string.Empty;

        [Required]
        public string DoctorId { get; set; } = string.Empty;

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Symptoms { get; set; }

        [StringLength(1000)]
        public string? BasicConsultationNotes { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? CancelledBy { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        public decimal? ConsultationFee { get; set; }

        public bool IsPaid { get; set; } = false;

        // Navigation properties
        public virtual ApplicationUser Patient { get; set; } = null!;
        public virtual ApplicationUser Doctor { get; set; } = null!;
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        InProgress,
        Completed,
        Cancelled,
        NoShow
    }
}

using System.ComponentModel.DataAnnotations;

namespace AfyaConnectLite.Models
{
    public class DoctorProfile
    {
        public int Id { get; set; }

        [Required]
        public string DoctorId { get; set; } = string.Empty;

        [Required]
        public int SpecialtyId { get; set; }

        [StringLength(50)]
        public string? LicenseNumber { get; set; }

        [StringLength(1000)]
        public string? Qualifications { get; set; }

        [StringLength(1000)]
        public string? Experience { get; set; }

        public decimal? ConsultationFee { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime? ApprovedAt { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ApplicationUser Doctor { get; set; } = null!;
        public virtual MedicalSpecialty Specialty { get; set; } = null!;
    }
}

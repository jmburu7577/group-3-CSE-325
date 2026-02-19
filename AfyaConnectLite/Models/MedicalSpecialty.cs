using System.ComponentModel.DataAnnotations;

namespace AfyaConnectLite.Models
{
    public class MedicalSpecialty
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<DoctorProfile> DoctorProfiles { get; set; } = new List<DoctorProfile>();
    }
}

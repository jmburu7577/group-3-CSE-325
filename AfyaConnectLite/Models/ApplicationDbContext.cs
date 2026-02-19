using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AfyaConnectLite.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ConsultationNote> ConsultationNotes { get; set; }
        public DbSet<DoctorProfile> DoctorProfiles { get; set; }
        public DbSet<MedicalSpecialty> MedicalSpecialties { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Appointment entity
            builder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Symptoms)
                    .HasMaxLength(1000);

                entity.Property(e => e.BasicConsultationNotes)
                    .HasMaxLength(1000);

                entity.Property(e => e.CancellationReason)
                    .HasMaxLength(500);

                entity.Property(e => e.ConsultationFee)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.PatientAppointments)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Doctor)
                    .WithMany(d => d.DoctorAppointments)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AppointmentDate);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.Status);
            });

            // Configure ConsultationNote entity
            builder.Entity<ConsultationNote>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Notes)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(e => e.Patient)
                    .WithMany()
                    .HasForeignKey(e => e.PatientId)
                    .HasConstraintName("FK_ConsultationNotes_AspNetUsers_ApplicationUserId")
                    .OnDelete(DeleteBehavior.Restrict);

                // Map domain property PatientId to the actual DB column name used in migrations
                entity.Property(e => e.PatientId).HasColumnName("ApplicationUserId");

                entity.HasOne(e => e.Doctor)
                    .WithMany()
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Explicit FK to Appointment (was previously a shadow FK in migrations)
                entity.HasOne(e => e.Appointment)
                    .WithMany()
                    .HasForeignKey(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DoctorId);
            });

            // Configure DoctorProfile entity
            builder.Entity<DoctorProfile>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LicenseNumber)
                    .HasMaxLength(50);

                entity.Property(e => e.Qualifications)
                    .HasMaxLength(1000);

                entity.Property(e => e.Experience)
                    .HasMaxLength(1000);

                entity.Property(e => e.ConsultationFee)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Doctor)
                    .WithOne(d => d.DoctorProfile)
                    .HasForeignKey<DoctorProfile>(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Specialty)
                    .WithMany(s => s.DoctorProfiles)
                    .HasForeignKey(e => e.SpecialtyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.DoctorId).IsUnique();
                entity.HasIndex(e => e.SpecialtyId);
                entity.HasIndex(e => e.IsApproved);
            });

            // Configure MedicalSpecialty entity
            builder.Entity<MedicalSpecialty>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.IsActive);
            });

            // Configure ApplicationUser entity
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Address)
                    .HasMaxLength(200);

                entity.Property(e => e.NationalId)
                    .HasMaxLength(20);

                entity.Property(e => e.Gender)
                    .HasMaxLength(10);

                entity.Property(e => e.MedicalHistory)
                    .HasMaxLength(500);

                entity.Property(e => e.EmergencyContactName)
                    .HasMaxLength(100);

                entity.Property(e => e.EmergencyContactPhone)
                    .HasMaxLength(20);

                entity.HasOne(e => e.DoctorProfile)
                    .WithOne(dp => dp.Doctor)
                    .HasForeignKey<DoctorProfile>(dp => dp.DoctorId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed default medical specialties
            builder.Entity<MedicalSpecialty>().HasData(
                new MedicalSpecialty { Id = 1, Name = "General Practice", Description = "General medical practice and primary care", IsActive = true },
                new MedicalSpecialty { Id = 2, Name = "Pediatrics", Description = "Medical care for infants, children, and adolescents", IsActive = true },
                new MedicalSpecialty { Id = 3, Name = "Cardiology", Description = "Heart and cardiovascular system disorders", IsActive = true },
                new MedicalSpecialty { Id = 4, Name = "Dermatology", Description = "Skin, hair, and nail disorders", IsActive = true },
                new MedicalSpecialty { Id = 5, Name = "Mental Health", Description = "Psychological and psychiatric care", IsActive = true }
            );
        }
    }
}

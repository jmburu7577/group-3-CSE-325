using AfyaConnectLite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AfyaConnectLite.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider services, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            // Create roles if they don't exist
            await CreateRoles(roleManager);

            // Create admin user if it doesn't exist
            await CreateAdminUser(userManager, roleManager);

            // Create sample doctor if it doesn't exist
            await CreateSampleDoctor(userManager, roleManager, context);

            // Create sample patient if it doesn't exist
            await CreateSamplePatient(userManager, roleManager);
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Patient", "Doctor", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task CreateAdminUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@afyaconnect.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@afyaconnect.com",
                    Email = "admin@afyaconnect.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task CreateSampleDoctor(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            var doctorUser = await userManager.FindByEmailAsync("doctor@afyaconnect.com");
            if (doctorUser == null)
            {
                doctorUser = new ApplicationUser
                {
                    UserName = "doctor@afyaconnect.com",
                    Email = "doctor@afyaconnect.com",
                    FirstName = "John",
                    LastName = "Smith",
                    DateOfBirth = DateTime.Now.AddYears(-35),
                    EmailConfirmed = true,
                    IsActive = true,
                    NationalId = "DOC123456",
                    Gender = "Male"
                };

                var result = await userManager.CreateAsync(doctorUser, "Doctor123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(doctorUser, "Doctor");

                    // Create doctor profile
                    var generalPracticeSpecialty = await context.MedicalSpecialties
                        .FirstOrDefaultAsync(s => s.Name == "General Practice");

                    if (generalPracticeSpecialty != null)
                    {
                        var doctorProfile = new DoctorProfile
                        {
                            DoctorId = doctorUser.Id,
                            SpecialtyId = generalPracticeSpecialty.Id,
                            LicenseNumber = "MD789012",
                            Qualifications = "MD from University of Medicine, Board Certified in General Practice",
                            Experience = "10+ years in general practice with focus on preventive care",
                            ConsultationFee = 50.00m,
                            IsApproved = true,
                            ApprovedAt = DateTime.UtcNow,
                            ApprovedBy = "admin@afyaconnect.com"
                        };

                        context.DoctorProfiles.Add(doctorProfile);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        private static async Task CreateSamplePatient(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var patientUser = await userManager.FindByEmailAsync("patient@afyaconnect.com");
            if (patientUser == null)
            {
                patientUser = new ApplicationUser
                {
                    UserName = "patient@afyaconnect.com",
                    Email = "patient@afyaconnect.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    DateOfBirth = DateTime.Now.AddYears(-25),
                    EmailConfirmed = true,
                    IsActive = true,
                    NationalId = "PAT789012",
                    Gender = "Female",
                    Address = "123 Main St, City, State",
                    EmergencyContactName = "John Doe",
                    EmergencyContactPhone = "555-0123",
                    MedicalHistory = "No chronic conditions"
                };

                var result = await userManager.CreateAsync(patientUser, "Patient123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(patientUser, "Patient");
                }
            }
        }
    }
}

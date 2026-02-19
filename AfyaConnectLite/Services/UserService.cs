using AfyaConnectLite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AfyaConnectLite.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.DoctorProfile)
                    .ThenInclude(dp => dp!.Specialty)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                throw;
            }
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.DoctorProfile)
                    .ThenInclude(dp => dp!.Specialty)
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.DoctorProfile)
                    .ThenInclude(dp => dp!.Specialty)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string role)
        {
            try
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                return usersInRole
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users by role {Role}", role);
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated user {UserId}", user.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", user.Id);
                return false;
            }
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deactivated user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Activated user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Add default Patient role
                    await _userManager.AddToRoleAsync(user, "Patient");
                    _logger.LogInformation("Created user {UserId}", user.Id);
                    return true;
                }

                _logger.LogWarning("Failed to create user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<IEnumerable<DoctorProfile>> GetDoctorProfilesAsync()
        {
            try
            {
                return await _context.DoctorProfiles
                    .Include(dp => dp.Doctor)
                    .Include(dp => dp.Specialty)
                    .OrderBy(dp => dp.Doctor.LastName)
                    .ThenBy(dp => dp.Doctor.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor profiles");
                throw;
            }
        }

        public async Task<DoctorProfile?> GetDoctorProfileAsync(string doctorId)
        {
            try
            {
                return await _context.DoctorProfiles
                    .Include(dp => dp.Doctor)
                    .Include(dp => dp.Specialty)
                    .FirstOrDefaultAsync(dp => dp.DoctorId == doctorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor profile for {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<bool> ApproveDoctorProfileAsync(string doctorId, string approvedBy)
        {
            try
            {
                var profile = await _context.DoctorProfiles.FirstOrDefaultAsync(dp => dp.DoctorId == doctorId);
                if (profile == null)
                {
                    return false;
                }

                profile.IsApproved = true;
                profile.ApprovedAt = DateTime.UtcNow;
                profile.ApprovedBy = approvedBy;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Approved doctor profile for {DoctorId} by {ApprovedBy}", doctorId, approvedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving doctor profile for {DoctorId}", doctorId);
                return false;
            }
        }

        public async Task<bool> UpdateDoctorProfileAsync(DoctorProfile profile)
        {
            try
            {
                _context.DoctorProfiles.Update(profile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated doctor profile {ProfileId}", profile.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor profile {ProfileId}", profile.Id);
                return false;
            }
        }

        public async Task<IEnumerable<MedicalSpecialty>> GetMedicalSpecialtiesAsync()
        {
            try
            {
                return await _context.MedicalSpecialties
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical specialties");
                throw;
            }
        }
    }
}

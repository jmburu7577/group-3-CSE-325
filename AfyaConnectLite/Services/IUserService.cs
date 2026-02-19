using AfyaConnectLite.Models;

namespace AfyaConnectLite.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string role);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ActivateUserAsync(string userId);
        Task<bool> CreateUserAsync(ApplicationUser user, string password);
        Task<IEnumerable<DoctorProfile>> GetDoctorProfilesAsync();
        Task<DoctorProfile?> GetDoctorProfileAsync(string doctorId);
        Task<bool> ApproveDoctorProfileAsync(string doctorId, string approvedBy);
        Task<bool> UpdateDoctorProfileAsync(DoctorProfile profile);
        Task<IEnumerable<MedicalSpecialty>> GetMedicalSpecialtiesAsync();
    }
}

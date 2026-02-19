using AfyaConnectLite.Models;

namespace AfyaConnectLite.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAppointmentsForPatientAsync(string patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsForDoctorAsync(string doctorId);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> CancelAppointmentAsync(int appointmentId, string cancelledBy, string reason);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status);
        Task<ConsultationNote> AddConsultationNoteAsync(ConsultationNote note);
        Task<IEnumerable<ConsultationNote>> GetConsultationNotesForAppointmentAsync(string patientId);
        Task<IEnumerable<ConsultationNote>> GetConsultationNotesForDoctorAsync(string doctorId);
        Task<ConsultationNote> UpdateConsultationNoteAsync(ConsultationNote note);
        Task DeleteConsultationNoteAsync(int noteId);
    }
}

using AfyaConnectLite.Models;
using Microsoft.EntityFrameworkCore;

namespace AfyaConnectLite.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(ApplicationDbContext context, ILogger<AppointmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForPatientAsync(string patientId)
        {
            try
            {
                return await _context.Appointments
                    .Include(a => a.Doctor)
                    .Where(a => a.PatientId == patientId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for patient {PatientId}", patientId);
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForDoctorAsync(string doctorId)
        {
            try
            {
                return await _context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorId == doctorId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for doctor {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            try
            {
                return await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment {AppointmentId}", id);
                throw;
            }
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            try
            {
                // Check for conflicting appointments
                var conflictingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.DoctorId == appointment.DoctorId &&
                                          a.AppointmentDate == appointment.AppointmentDate &&
                                          a.Status != AppointmentStatus.Cancelled &&
                                          a.Status != AppointmentStatus.NoShow);

                if (conflictingAppointment != null)
                {
                    throw new InvalidOperationException("Doctor already has an appointment at this time.");
                }

                appointment.CreatedAt = DateTime.UtcNow;
                appointment.Status = AppointmentStatus.Scheduled;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created appointment {AppointmentId} for patient {PatientId} with doctor {DoctorId}",
                    appointment.Id, appointment.PatientId, appointment.DoctorId);

                return appointment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment for patient {PatientId}", appointment.PatientId);
                throw;
            }
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            try
            {
                appointment.UpdatedAt = DateTime.UtcNow;
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated appointment {AppointmentId}", appointment.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {AppointmentId}", appointment.Id);
                return false;
            }
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, string cancelledBy, string reason)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                if (appointment == null)
                {
                    return false;
                }

                if (appointment.Status == AppointmentStatus.Completed)
                {
                    throw new InvalidOperationException("Cannot cancel a completed appointment.");
                }

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.CancelledBy = cancelledBy;
                appointment.CancellationReason = reason;
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cancelled appointment {AppointmentId} by {CancelledBy}", appointmentId, cancelledBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment {AppointmentId}", appointmentId);
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            try
            {
                return await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all appointments");
                throw;
            }
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                if (appointment == null)
                {
                    return false;
                }

                appointment.Status = status;
                appointment.UpdatedAt = DateTime.UtcNow;

                if (status == AppointmentStatus.Completed)
                {
                    appointment.CompletedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated appointment {AppointmentId} status to {Status}", appointmentId, status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {AppointmentId} status", appointmentId);
                return false;
            }
        }

        public async Task<ConsultationNote> AddConsultationNoteAsync(ConsultationNote note)
        {
            try
            {
                note.CreatedAt = DateTime.UtcNow;
                _context.ConsultationNotes.Add(note);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added consultation note {NoteId} for patient {PatientId}", note.Id, note.PatientId);
                return note;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving consultation notes for patient {PatientId}", note.PatientId);
                throw;
            }
        }

        public async Task<IEnumerable<ConsultationNote>> GetConsultationNotesForAppointmentAsync(string patientId)
        {
            try
            {
                return await _context.ConsultationNotes
                    .Include(cn => cn.Doctor)
                    .Where(cn => cn.PatientId == patientId)
                    .OrderBy(cn => cn.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving consultation notes for patient {PatientId}", patientId);
                throw;
            }
        }

        public async Task<IEnumerable<ConsultationNote>> GetConsultationNotesForDoctorAsync(string doctorId)
        {
            try
            {
                return await _context.ConsultationNotes
                    .Include(cn => cn.Patient)
                    .Include(cn => cn.Doctor)
                    .Where(cn => cn.DoctorId == doctorId)
                    .OrderByDescending(cn => cn.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving consultation notes for doctor {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<ConsultationNote> UpdateConsultationNoteAsync(ConsultationNote note)
        {
            try
            {
                _context.ConsultationNotes.Update(note);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Updated consultation note {NoteId}", note.Id);
                return note;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating consultation note {NoteId}", note.Id);
                throw;
            }
        }

        public async Task DeleteConsultationNoteAsync(int noteId)
        {
            try
            {
                var note = await _context.ConsultationNotes.FindAsync(noteId);
                if (note != null)
                {
                    _context.ConsultationNotes.Remove(note);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Deleted consultation note {NoteId}", noteId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting consultation note {NoteId}", noteId);
                throw;
            }
        }
    }
}

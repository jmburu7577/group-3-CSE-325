# AfyaConnect Lite - User Stories

## Patient Stories

### Story 1: Account Registration
**As a** new patient  
**I want to** create an account with my personal information  
**So that** I can access the telemedicine platform

**Acceptance Criteria:**
- User can register with email, password, and personal details
- Email verification is required
- Password must meet complexity requirements
- User is assigned "Patient" role by default

### Story 2: Login Access
**As a** registered patient  
**I want to** log in securely to my account  
**So that** I can access my medical appointments

**Acceptance Criteria:**
- User can log in with email and password
- Invalid credentials show appropriate error messages
- Remember me functionality is available
- Session timeout after inactivity

### Story 3: Book Appointment
**As a** patient  
**I want to** book a medical appointment online  
**So that** I don't have to wait in long hospital queues

**Acceptance Criteria:**
- Patient can select doctor from available list
- Patient can choose preferred date and time
- Appointment reason is required
- Confirmation is sent after successful booking
- Duplicate appointments are prevented

### Story 4: View Appointment History
**As a** patient  
**I want to** view my appointment history  
**So that** I can track my medical consultations

**Acceptance Criteria:**
- Patient sees all past and upcoming appointments
- Appointments are sorted by date
- Status is clearly visible (Scheduled, Completed, Cancelled)
- Consultation notes are visible for completed appointments

### Story 5: Cancel Appointment
**As a** patient  
**I want to** cancel upcoming appointments  
**So that** I can manage my schedule effectively

**Acceptance Criteria:**
- Only upcoming appointments can be cancelled
- Cancellation requires confirmation
- Doctor is notified of cancellation
- Appointment status updates to "Cancelled"

## Doctor Stories

### Story 6: Doctor Registration
**As a** healthcare provider  
**I want to** register as a doctor  
**So that** I can provide telemedicine consultations

**Acceptance Criteria:**
- Doctor can register with professional credentials
- Admin approval is required for doctor accounts
- Professional information is stored securely
- Doctor is assigned "Doctor" role after approval

### Story 7: View Appointments
**As a** doctor  
**I want to** view my scheduled appointments  
**So that** I can prepare for patient consultations

**Acceptance Criteria:**
- Doctor sees appointments assigned to them
- Appointments are grouped by date
- Patient information is visible
- Appointment status is clearly displayed

### Story 8: Add Consultation Notes
**As a** doctor  
**I want to** add consultation notes during appointments  
**So that** I can document patient interactions

**Acceptance Criteria:**
- Notes can be added during or after appointments
- Notes are timestamped and attributed to doctor
- Notes are securely stored and accessible to patient
- Rich text formatting is available for notes

### Story 9: Update Appointment Status
**As a** doctor  
**I want to** update appointment status  
**So that** I can track consultation progress

**Acceptance Criteria:**
- Status can be changed from Scheduled to InProgress
- Status can be changed from InProgress to Completed
- Status changes are logged with timestamps
- Patients are notified of status changes

## Administrator Stories

### Story 10: User Management
**As an** administrator  
**I want to** manage user accounts  
**So that** I can maintain system security

**Acceptance Criteria:**
- Admin can view all user accounts
- Admin can approve/reject doctor registrations
- Admin can disable/enable user accounts
- Admin can reset user passwords

### Story 11: System Monitoring
**As an** administrator  
**I want to** view system statistics  
**So that** I can monitor platform usage

**Acceptance Criteria:**
- Dashboard shows total users by role
- Appointment statistics are displayed
- System health indicators are visible
- Export functionality for reports

### Story 12: Appointment Oversight
**As an** administrator  
**I want to** view all appointments  
**So that** I can ensure smooth operations

**Acceptance Criteria:**
- All appointments are visible regardless of doctor/patient
- Appointments can be filtered by date, status, or participants
- Admin can modify appointment details if needed
- Audit trail is maintained for all changes

## Cross-Functional Stories

### Story 13: Responsive Design
**As a** user  
**I want to** access the platform on any device  
**So that** I can manage healthcare needs on the go

**Acceptance Criteria:**
- Platform works on desktop, tablet, and mobile
- Touch interactions are optimized for mobile
- Layout adapts to different screen sizes
- Performance is acceptable on all devices

### Story 14: Data Privacy
**As a** user  
**I want to** ensure my medical information is private  
**So that** I can trust the platform with sensitive data

**Acceptance Criteria:**
- Personal information is encrypted
- Medical data is only accessible to authorized users
- Audit logs track all data access
- GDPR compliance is maintained

### Story 15: Notifications
**As a** user  
**I want to** receive notifications about appointments  
**So that** I stay informed about my medical schedule

**Acceptance Criteria:**
- Email notifications for appointment confirmations
- Reminders sent before scheduled appointments
- Notifications for appointment changes
- Notification preferences are configurable

# AfyaConnect Lite - Telemedicine Platform

A comprehensive web-based telemedicine application built with .NET Blazor that enables patients to book medical appointments, consult with healthcare providers remotely, and manage basic health interactions online.

## 🌟 Features

### Core Functionality
- **User Registration & Authentication**: Secure user registration with role-based access control
- **Appointment Booking**: Easy appointment scheduling with qualified healthcare professionals
- **Doctor Management**: Comprehensive dashboard for doctors to manage appointments and add consultation notes
- **Admin Panel**: Complete system administration and oversight capabilities
- **Consultation Notes**: Secure medical note-taking and patient history tracking
- **Responsive Design**: Optimized for desktop, tablet, and mobile devices

### User Roles
- **Patients**: Book appointments, view history, manage personal information
- **Doctors**: View scheduled appointments, add consultation notes, manage patient interactions
- **Administrators**: User management, system oversight, doctor approvals

## 🏗️ Architecture

### Technology Stack
- **Frontend**: Blazor Server with Interactive Server Components
- **Backend**: ASP.NET Core 8.0
- **Database**: SQLite (development) / SQL Server (production)
- **Authentication**: ASP.NET Core Identity
- **ORM**: Entity Framework Core 8.0
- **UI Framework**: Bootstrap 5

### System Architecture
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Blazor UI     │    │  Application    │    │   Data Layer    │
│   Components    │◄──►│    Services     │◄──►│   Entity Framework│
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Authentication│    │  Authorization  │    │   Database      │
│   & Authorization│   │    Policies     │    │   (SQLite/SQL)  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 📁 Project Structure

```
AfyaConnectLite/
├── Components/
│   ├── Pages/
│   │   ├── Patient/          # Patient-facing components
│   │   ├── Doctor/           # Doctor-facing components
│   │   ├── Admin/            # Admin-facing components
│   │   └── Home.razor        # Landing page
│   └── Shared/
│       └── MainLayout.razor  # Main application layout
├── Data/
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs          # Database seeding
├── Models/
│   ├── ApplicationUser.cs     # Extended user model
│   ├── Appointment.cs        # Appointment entity
│   ├── ConsultationNote.cs   # Medical notes
│   ├── DoctorProfile.cs      # Doctor profile
│   └── MedicalSpecialty.cs   # Medical specialties
├── Services/
│   ├── IAppointmentService.cs
│   ├── AppointmentService.cs
│   ├── IUserService.cs
│   └── UserService.cs
├── Pages/
│   └── Account/              # Authentication pages
├── docs/
│   ├── ARCHITECTURE.md       # System architecture documentation
│   └── USER_STORIES.md       # User stories and requirements
└── README.md
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code
- SQLite (for development)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/afyaconnect-lite.git
   cd afyaconnect-lite
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Create and apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Open your browser and navigate to `https://localhost:5001`
   - Register a new account or use the default admin credentials

### Default Credentials
- **Admin**: admin@afyaconnect.com / Admin123!
- **Doctor**: doctor@afyaconnect.com / Doctor123!
- **Patient**: patient@afyaconnect.com / Patient123!

## 📊 Database Schema

### Core Entities

#### ApplicationUser
- Extends ASP.NET Identity user with healthcare-specific fields
- Includes personal information, medical history, emergency contacts

#### Appointment
- Links patients and doctors with scheduled consultations
- Tracks appointment status, consultation notes, and payment information

#### ConsultationNote
- Medical notes entered by doctors during consultations
- Timestamped and attributed to specific doctors

#### DoctorProfile
- Professional profile for doctors including specialties and qualifications
- Requires admin approval before doctors can see patients

#### MedicalSpecialty
- Defines medical specialties available in the system
- Used to categorize doctors and appointments

## 🔐 Security Features

- **Authentication**: ASP.NET Core Identity with secure password policies
- **Authorization**: Role-based access control with granular permissions
- **Data Protection**: Sensitive medical information encryption
- **Secure Routing**: Protected pages and API endpoints
- **Input Validation**: Comprehensive data validation and sanitization

## 🎯 User Stories

### Patient Stories
- As a patient, I want to book appointments online to avoid waiting in queues
- As a patient, I want to view my appointment history and consultation notes
- As a patient, I want to manage my personal health information securely

### Doctor Stories
- As a doctor, I want to view my scheduled appointments and patient information
- As a doctor, I want to add consultation notes during patient visits
- As a doctor, I want to manage my professional profile and availability

### Admin Stories
- As an admin, I want to manage user accounts and approve doctor registrations
- As an admin, I want to monitor system usage and appointment statistics
- As an admin, I want to oversee all appointments and system health

## 🧪 Testing

### Running Tests
```bash
dotnet test
```

### Test Coverage
- Unit tests for business logic
- Integration tests for database operations
- UI tests for critical user workflows

## 📦 Deployment

### Development Environment
```bash
dotnet run --environment Development
```

### Production Environment
```bash
dotnet run --environment Production
```

### Docker Deployment
```bash
docker build -t afyaconnect-lite .
docker run -p 8080:80 afyaconnect-lite
```

### Azure Deployment
The application is designed to be easily deployable to Azure App Service with SQL Database integration.

## 🔧 Configuration

### Database Configuration
Update `appsettings.json` with your database connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=afyaconnect.db"
  }
}
```

### Email Configuration
Configure email settings for appointment notifications:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Microsoft .NET team for the excellent Blazor framework
- Bootstrap team for the responsive UI components
- The healthcare professionals who provided requirements and feedback

## 📞 Support

For support and questions:
- Create an issue in the GitHub repository
- Email: support@afyaconnect.com
- Documentation: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

## 🗺️ Roadmap

### Phase 1 (Current)
- ✅ Basic appointment booking
- ✅ User authentication
- ✅ Doctor dashboard
- ✅ Admin panel

### Phase 2 (Planned)
- 🔄 Video conferencing integration
- 🔄 Online payment processing
- 🔄 Mobile application
- 🔄 Advanced reporting

### Phase 3 (Future)
- 📋 AI-powered symptom checker
- 📋 Integration with hospital EHR systems
- 📋 Prescription management
- 📋 Multi-language support

---

**AfyaConnect Lite** - Making healthcare accessible, one appointment at a time. 🏥💙

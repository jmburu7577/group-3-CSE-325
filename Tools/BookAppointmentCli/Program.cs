using Microsoft.Data.Sqlite;
using System.Text;

string email = null;
if (args.Length > 0) email = args[0];
else
{
    var path = Path.Combine("Tools","last_registered_email.txt");
    if (File.Exists(path)) email = File.ReadAllText(path).Trim();
}
if (string.IsNullOrEmpty(email))
{
    Console.WriteLine("Usage: dotnet run --project Tools/BookAppointmentCli -- <email> or ensure Tools/last_registered_email.txt exists");
    return 1;
}

// locate DB
string dbPath = null;
var dir = Directory.GetCurrentDirectory();
while (!string.IsNullOrEmpty(dir))
{
    var candidate = Path.Combine(dir, "AfyaConnectLite", "afyaconnect.db");
    if (File.Exists(candidate)) { dbPath = Path.GetFullPath(candidate); break; }
    var parent = Directory.GetParent(dir);
    dir = parent?.FullName;
}
if (dbPath == null)
{
    var fallback = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "group-3-CSE-325", "AfyaConnectLite", "afyaconnect.db"));
    if (File.Exists(fallback)) dbPath = fallback;
}
if (dbPath == null)
{
    Console.WriteLine("Could not locate afyaconnect.db");
    return 2;
}
Console.WriteLine($"Using DB: {dbPath}");

using var conn = new SqliteConnection($"Data Source={dbPath}");
conn.Open();

// find user by email
string patientId = null;
using (var cmd = conn.CreateCommand())
{
    cmd.CommandText = "SELECT Id FROM AspNetUsers WHERE Email = @email LIMIT 1";
    cmd.Parameters.AddWithValue("@email", email);
    var obj = cmd.ExecuteScalar();
    if (obj != null) patientId = obj.ToString();
}
if (patientId == null)
{
    Console.WriteLine($"User with email {email} not found in AspNetUsers");
    return 3;
}
Console.WriteLine($"Found patient Id: {patientId}");

// find a doctor with DoctorProfile
string doctorId = null;
using (var cmd = conn.CreateCommand())
{
    cmd.CommandText = "SELECT DoctorId FROM DoctorProfiles WHERE IsApproved = 1 LIMIT 1";
    var obj = cmd.ExecuteScalar();
    if (obj != null) doctorId = obj.ToString();
}
if (doctorId == null)
{
    // try to create a doctor user and profile
    doctorId = Guid.NewGuid().ToString();
    using (var tx = conn.BeginTransaction())
    {
        using var ins = conn.CreateCommand();
        ins.Transaction = tx;
        ins.CommandText = "INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, FirstName, LastName, DateOfBirth, CreatedAt) VALUES (@Id,@UserName,@Norm,@Email,@NormE,0,@First,@Last,'1980-01-01',@Created)";
        ins.Parameters.AddWithValue("@Id", doctorId);
        ins.Parameters.AddWithValue("@UserName", "auto.doctor@local.test");
        ins.Parameters.AddWithValue("@Norm", "AUTO.DOCTOR@LOCAL.TEST");
        ins.Parameters.AddWithValue("@Email", "auto.doctor@local.test");
        ins.Parameters.AddWithValue("@NormE", "AUTO.DOCTOR@LOCAL.TEST");
        ins.Parameters.AddWithValue("@First", "Auto");
        ins.Parameters.AddWithValue("@Last", "Doctor");
        ins.Parameters.AddWithValue("@Created", DateTime.UtcNow.ToString("o"));
        ins.ExecuteNonQuery();

        using var ins2 = conn.CreateCommand();
        ins2.Transaction = tx;
        ins2.CommandText = "INSERT INTO MedicalSpecialties (Name, Description, IsActive, CreatedAt) VALUES (@n,@d,1,@c); SELECT last_insert_rowid();";
        ins2.Parameters.AddWithValue("@n", "AutoSpec");
        ins2.Parameters.AddWithValue("@d", "Auto created");
        ins2.Parameters.AddWithValue("@c", DateTime.UtcNow.ToString("o"));
        var sid = Convert.ToInt32(ins2.ExecuteScalar());

        using var ins3 = conn.CreateCommand();
        ins3.Transaction = tx;
        ins3.CommandText = "INSERT INTO DoctorProfiles (DoctorId, SpecialtyId, LicenseNumber, Qualifications, Experience, ConsultationFee, IsApproved, CreatedAt) VALUES (@did,@sid,@lic,@qual,@exp,@fee,1,@created)";
        ins3.Parameters.AddWithValue("@did", doctorId);
        ins3.Parameters.AddWithValue("@sid", sid);
        ins3.Parameters.AddWithValue("@lic", "AUTO-LIC");
        ins3.Parameters.AddWithValue("@qual", "MD");
        ins3.Parameters.AddWithValue("@exp", "5 years");
        ins3.Parameters.AddWithValue("@fee", 50.0m);
        ins3.Parameters.AddWithValue("@created", DateTime.UtcNow.ToString("o"));
        ins3.ExecuteNonQuery();

        tx.Commit();
    }
    Console.WriteLine($"Inserted auto doctor {doctorId}");
}
else
{
    Console.WriteLine($"Using doctorId: {doctorId}");
}

// insert appointment
int appointmentId = 0;
using (var cmd = conn.CreateCommand())
{
    cmd.CommandText = "INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, Reason, Symptoms, ConsultationFee, IsPaid, CreatedAt, Status) VALUES (@pid,@did,@adate,@reason,@symp,@fee,0,@created,0); SELECT last_insert_rowid();";
    cmd.Parameters.AddWithValue("@pid", patientId);
    cmd.Parameters.AddWithValue("@did", doctorId);
    cmd.Parameters.AddWithValue("@adate", DateTime.UtcNow.AddDays(3).ToString("o"));
    cmd.Parameters.AddWithValue("@reason", "Programmatic booking test");
    cmd.Parameters.AddWithValue("@symp", "None");
    cmd.Parameters.AddWithValue("@fee", 50.0m);
    cmd.Parameters.AddWithValue("@created", DateTime.UtcNow.ToString("o"));
    appointmentId = Convert.ToInt32(cmd.ExecuteScalar());
}
Console.WriteLine($"Inserted appointment Id={appointmentId}");

// insert a consultation note for the appointment
try
{
    using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "INSERT INTO ConsultationNotes (AppointmentId, DoctorId, Notes, CreatedAt, ApplicationUserId) VALUES (@aid,@did,@notes,@created,@pid); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@aid", appointmentId);
        cmd.Parameters.AddWithValue("@did", doctorId);
        cmd.Parameters.AddWithValue("@notes", "Programmatic consultation note");
        cmd.Parameters.AddWithValue("@created", DateTime.UtcNow.ToString("o"));
        cmd.Parameters.AddWithValue("@pid", patientId);
        var noteId = Convert.ToInt32(cmd.ExecuteScalar());
        Console.WriteLine($"Inserted ConsultationNote Id={noteId}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed inserting consultation note: {ex.Message}");
}

return 0;
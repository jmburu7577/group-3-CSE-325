using Microsoft.Data.Sqlite;

// Locate afyaconnect.db by walking up folders looking for AfyaConnectLite/afyaconnect.db
string dbPath = null;
var dir = Directory.GetCurrentDirectory();
while (!string.IsNullOrEmpty(dir))
{
    var candidate = Path.Combine(dir, "AfyaConnectLite", "afyaconnect.db");
    if (File.Exists(candidate))
    {
        dbPath = Path.GetFullPath(candidate);
        break;
    }
    var parent = Directory.GetParent(dir);
    dir = parent?.FullName;
}
// fallback to known workspace path
if (dbPath == null)
{
    var fallback = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "group-3-CSE-325", "AfyaConnectLite", "afyaconnect.db"));
    if (File.Exists(fallback)) dbPath = fallback;
}
if (dbPath == null) dbPath = Path.Combine(Directory.GetCurrentDirectory(), "afyaconnect.db");

void Log(string s)
{
    Console.WriteLine(s);
    Console.Out.Flush();
}

Log($"Inspecting SQLite DB at: {dbPath}");

if (!File.Exists(dbPath))
{
    Log("ERROR: database file not found.");
    return 1;
}

using var conn = new SqliteConnection($"Data Source={dbPath}");
conn.Open();

string[] checks = new[] { "AspNetUsers", "Appointments", "ConsultationNotes", "DoctorProfiles", "MedicalSpecialties", "__EFMigrationsHistory" };

foreach (var table in checks)
{
    using var cmd = conn.CreateCommand();
    cmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@name";
    cmd.Parameters.AddWithValue("@name", table);
    var exists = Convert.ToInt32(cmd.ExecuteScalar() ?? 0) > 0;
    Log($"Table {table}: exists={exists}");
}

// Row counts sample
var queries = new Dictionary<string,string>
{
    { "UsersCount", "SELECT COUNT(*) FROM AspNetUsers" },
    { "AppointmentsCount", "SELECT COUNT(*) FROM Appointments" },
    { "ConsultationNotesCount", "SELECT COUNT(*) FROM ConsultationNotes" },
    { "DoctorProfilesCount", "SELECT COUNT(*) FROM DoctorProfiles" },
    { "SpecialtiesSample", "SELECT Id, Name FROM MedicalSpecialties LIMIT 5" }
};

foreach (var q in queries)
{
    try
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = q.Value;
        if (q.Key.EndsWith("Sample"))
        {
            using var reader = cmd.ExecuteReader();
            Log($"{q.Key}:");
            while (reader.Read())
            {
                Log($"  Id={reader.GetInt32(0)} Name={reader.GetString(1)}");
            }
        }
        else
        {
            var count = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
            Log($"{q.Key}: {count}");
        }
    }
    catch (Exception ex)
    {
        Log($"Query {q.Key} failed: {ex.Message}");
    }
}

// Inspect schema for ConsultationNotes to ensure AppointmentId column exists
try
{
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "PRAGMA table_info('ConsultationNotes')";
    using var reader = cmd.ExecuteReader();
    Log("ConsultationNotes schema:");
    while (reader.Read())
    {
        var cid = reader.GetInt32(0);
        var name = reader.GetString(1);
        var type = reader.GetString(2);
        var notnull = reader.GetInt32(3);
        var dflt = reader.IsDBNull(4) ? "NULL" : reader.GetValue(4).ToString();
        var pk = reader.GetInt32(5);
        Log($"  {cid}: {name} ({type}) notnull={notnull} pk={pk} default={dflt}");
    }
}
catch (Exception ex)
{
    Log("Failed reading ConsultationNotes schema: " + ex.Message);
}

Log("DB inspection completed.");

// --- Creation steps to ensure models work ---
Log("\nStarting creation steps: ensure doctor profile, appointment, consultation note exist.");
using (var tran = conn.BeginTransaction())
{
    string getUserSql = "SELECT Id, Email FROM AspNetUsers LIMIT 2";
    var users = new List<(string Id, string Email)>();
    using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = getUserSql;
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            users.Add((r.GetString(0), r.IsDBNull(1) ? "" : r.GetString(1)));
        }
    }

    string patientId = null;
    string doctorId = null;

    if (users.Count >= 2)
    {
        patientId = users[0].Id;
        doctorId = users[1].Id;
        Log($"Using existing users as patient {patientId} and doctor {doctorId}");
    }
    else if (users.Count == 1)
    {
        patientId = users[0].Id;
        doctorId = Guid.NewGuid().ToString();
        // insert doctor
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, FirstName, LastName, DateOfBirth, CreatedAt, AccessFailedCount, LockoutEnabled, PhoneNumberConfirmed, TwoFactorEnabled) VALUES (@Id,@UserName,@NormalizedUserName,@Email,@NormalizedEmail,0,@FirstName,@LastName,'1980-01-01',@CreatedAt,0,0,0,0)";
            cmd.Parameters.AddWithValue("@Id", doctorId);
            cmd.Parameters.AddWithValue("@UserName", "auto.doctor@local.test");
            cmd.Parameters.AddWithValue("@NormalizedUserName", "AUTO.DOCTOR@LOCAL.TEST");
            cmd.Parameters.AddWithValue("@Email", "auto.doctor@local.test");
            cmd.Parameters.AddWithValue("@NormalizedEmail", "AUTO.DOCTOR@LOCAL.TEST");
            cmd.Parameters.AddWithValue("@FirstName", "Auto");
            cmd.Parameters.AddWithValue("@LastName", "Doctor");
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }
        Log($"Inserted doctor user {doctorId}");
    }
    else
    {
        // insert patient and doctor
        patientId = Guid.NewGuid().ToString();
        doctorId = Guid.NewGuid().ToString();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, FirstName, LastName, DateOfBirth, CreatedAt, AccessFailedCount, LockoutEnabled, PhoneNumberConfirmed, TwoFactorEnabled) VALUES (@Id,@UserName,@NormalizedUserName,@Email,@NormalizedEmail,0,@FirstName,@LastName,'1990-01-01',@CreatedAt,0,0,0,0)";
            cmd.Parameters.AddWithValue("@Id", patientId);
            cmd.Parameters.AddWithValue("@UserName", "auto.patient@local.test");
            cmd.Parameters.AddWithValue("@NormalizedUserName", "AUTO.PATIENT@LOCAL.TEST");
            cmd.Parameters.AddWithValue("@Email", "auto.patient@local.test");
            cmd.Parameters.AddWithValue("@NormalizedEmail", "AUTO.PATIENT@LOCAL.TEST");
            cmd.Parameters.AddWithValue("@FirstName", "Auto");
            cmd.Parameters.AddWithValue("@LastName", "Patient");
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, FirstName, LastName, DateOfBirth, CreatedAt, AccessFailedCount, LockoutEnabled, PhoneNumberConfirmed, TwoFactorEnabled) VALUES (@Id,@UserName,@NormalizedUserName,@Email,@NormalizedEmail,0,@FirstName,@LastName,'1980-01-01',@CreatedAt,0,0,0,0)";
            cmd.Parameters.AddWithValue("@Id", doctorId);
            cmd.Parameters.AddWithValue("@UserName", "auto.doctor@local.test");
            cmd.Parameters.AddWithValue("@NormalizedUserName", "AUTO.DOCTOR@LOCAL.TEST");
            cmd.Parameters.AddWithValue("@Email", "auto.doctor@local.test");
            cmd.Parameters.AddWithValue("@NormalizedEmail", "AUTO.DOCTOR@LOCAL.TEST");
            cmd.Parameters.AddWithValue("@FirstName", "Auto");
            cmd.Parameters.AddWithValue("@LastName", "Doctor");
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }
        Log($"Inserted patient {patientId} and doctor {doctorId}");
    }

    // get a specialty id
    int specialtyId = 0;
    using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "SELECT Id FROM MedicalSpecialties LIMIT 1";
        var obj = cmd.ExecuteScalar();
        specialtyId = obj == null ? 0 : Convert.ToInt32(obj);
    }
    if (specialtyId == 0)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO MedicalSpecialties (Name, Description, IsActive, CreatedAt) VALUES (@name,@desc,1,@created); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@name", "AutoSpecialty");
        cmd.Parameters.AddWithValue("@desc", "Created by tester");
        cmd.Parameters.AddWithValue("@created", DateTime.UtcNow.ToString("o"));
        specialtyId = Convert.ToInt32(cmd.ExecuteScalar());
        Log($"Inserted specialty Id={specialtyId}");
    }
    else
    {
        Log($"Using specialty Id={specialtyId}");
    }

    // Ensure doctor profile exists
    using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "SELECT COUNT(*) FROM DoctorProfiles WHERE DoctorId = @did";
        cmd.Parameters.AddWithValue("@did", doctorId);
        var exists = Convert.ToInt32(cmd.ExecuteScalar());
        if (exists == 0)
        {
            using var ins = conn.CreateCommand();
            ins.CommandText = "INSERT INTO DoctorProfiles (DoctorId, SpecialtyId, LicenseNumber, Qualifications, Experience, ConsultationFee, IsApproved, CreatedAt) VALUES (@did,@sid,@lic,@qual,@exp,@fee,1,@created);";
            ins.Parameters.AddWithValue("@did", doctorId);
            ins.Parameters.AddWithValue("@sid", specialtyId);
            ins.Parameters.AddWithValue("@lic", "AUTO-LIC-1");
            ins.Parameters.AddWithValue("@qual", "MD");
            ins.Parameters.AddWithValue("@exp", "5 years");
            ins.Parameters.AddWithValue("@fee", 60.00m);
            ins.Parameters.AddWithValue("@created", DateTime.UtcNow.ToString("o"));
            ins.ExecuteNonQuery();
            Log($"Inserted DoctorProfile for doctor {doctorId}");
        }
        else
        {
            Log($"DoctorProfile already exists for {doctorId}");
        }
    }

    // Create appointment
    int appointmentId = 0;
    using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, Reason, Symptoms, ConsultationFee, IsPaid, CreatedAt, Status) VALUES (@pid,@did,@adate,@reason,@symp,@fee,0,@created,0); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@pid", patientId);
        cmd.Parameters.AddWithValue("@did", doctorId);
        cmd.Parameters.AddWithValue("@adate", DateTime.UtcNow.AddDays(2).ToString("o"));
        cmd.Parameters.AddWithValue("@reason", "Auto created appointment");
        cmd.Parameters.AddWithValue("@symp", "None");
        cmd.Parameters.AddWithValue("@fee", 60.00m);
        cmd.Parameters.AddWithValue("@created", DateTime.UtcNow.ToString("o"));
        appointmentId = Convert.ToInt32(cmd.ExecuteScalar());
    }
    Log($"Inserted Appointment Id={appointmentId}");

    // Create consultation note
    using (var cmd = conn.CreateCommand())
    {
        // DB schema uses ApplicationUserId for the Patient FK (migrations). Use that column if present.
        cmd.CommandText = "INSERT INTO ConsultationNotes (AppointmentId, DoctorId, Notes, CreatedAt, ApplicationUserId) VALUES (@aid,@did,@notes,@created,@pid); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@aid", appointmentId);
        cmd.Parameters.AddWithValue("@did", doctorId);
        cmd.Parameters.AddWithValue("@notes", "Auto note: all good");
        cmd.Parameters.AddWithValue("@created", DateTime.UtcNow.ToString("o"));
        cmd.Parameters.AddWithValue("@pid", patientId);
        var noteId = Convert.ToInt32(cmd.ExecuteScalar());
        Log($"Inserted ConsultationNote Id={noteId}");
    }

    tran.Commit();
}

Log("Creation steps completed.");
return 0;

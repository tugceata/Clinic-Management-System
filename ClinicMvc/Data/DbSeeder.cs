using ClinicMvc.Models;

namespace ClinicMvc.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Departments.Any()) return;

        // ============ BRANŞLAR ============
        var deps = new[]
        {
            new Department { Name = "Kardiyoloji" },
            new Department { Name = "Dermatoloji" },
            new Department { Name = "Nöroloji" },
            new Department { Name = "Pediatri" },
            new Department { Name = "Ortopedi" },
            new Department { Name = "Dahiliye" }
        };
        db.Departments.AddRange(deps);
        db.SaveChanges();

        // ============ DOKTORLAR ============
        var doctors = new[]
        {
            new Doctor { FullName = "Dr. Elif Yılmaz",  DoctorNumber = "DR001", Password = "1234", DepartmentId = deps[0].Id, RoomNumber = "101", MaxAppointments = 10, Email = "elif.yilmaz@klinik.com",  Bio = "Kardiyoloji uzmanı, 12 yıl deneyim. Hipertansiyon, ritim bozuklukları ve koroner hastalıklar.", WorkStartHour = 9,  WorkEndHour = 17 },
            new Doctor { FullName = "Dr. Mert Kaya",    DoctorNumber = "DR002", Password = "1234", DepartmentId = deps[1].Id, RoomNumber = "102", MaxAppointments = 12, Email = "mert.kaya@klinik.com",    Bio = "Cilt hastalıkları ve estetik dermatoloji.", WorkStartHour = 10, WorkEndHour = 18 },
            new Doctor { FullName = "Dr. Selin Demir",  DoctorNumber = "DR003", Password = "1234", DepartmentId = deps[2].Id, RoomNumber = "201", MaxAppointments = 8,  Email = "selin.demir@klinik.com",  Bio = "Nöroloji uzmanı. Migren, epilepsi ve nöropatik ağrı.", WorkStartHour = 9,  WorkEndHour = 16 },
            new Doctor { FullName = "Dr. Can Aydın",    DoctorNumber = "DR004", Password = "1234", DepartmentId = deps[3].Id, RoomNumber = "202", MaxAppointments = 14, Email = "can.aydin@klinik.com",    Bio = "Çocuk sağlığı ve hastalıkları uzmanı.", WorkStartHour = 9,  WorkEndHour = 17 },
            new Doctor { FullName = "Dr. Ayşe Şahin",   DoctorNumber = "DR005", Password = "1234", DepartmentId = deps[4].Id, RoomNumber = "301", MaxAppointments = 10, Email = "ayse.sahin@klinik.com",   Bio = "Ortopedi ve travmatoloji uzmanı.", WorkStartHour = 8,  WorkEndHour = 15 },
            new Doctor { FullName = "Dr. Burak Öztürk", DoctorNumber = "DR006", Password = "1234", DepartmentId = deps[5].Id, RoomNumber = "302", MaxAppointments = 12, Email = "burak.ozturk@klinik.com", Bio = "İç hastalıkları uzmanı.", WorkStartHour = 9,  WorkEndHour = 18 }
        };
        db.Doctors.AddRange(doctors);
        db.SaveChanges();

        // ============ HASTALAR ============
        var patients = new[]
        {
            new Patient { FullName = "Ayşe Çelik",    Email = "ayse@mail.com",   Password = "1234", Phone = "05551112233", DateOfBirth = new DateTime(1995, 4, 12),  Gender = Gender.Female },
            new Patient { FullName = "Burak Şahin",   Email = "burak@mail.com",  Password = "1234", Phone = "05554445566", DateOfBirth = new DateTime(1988, 9, 3),   Gender = Gender.Male },
            new Patient { FullName = "Zeynep Arslan", Email = "zeynep@mail.com", Password = "1234", Phone = "05553334477", DateOfBirth = new DateTime(1992, 7, 21),  Gender = Gender.Female },
            new Patient { FullName = "Mehmet Yıldız", Email = "mehmet@mail.com", Password = "1234", Phone = "05556667788", DateOfBirth = new DateTime(1985, 2, 8),   Gender = Gender.Male },
            new Patient { FullName = "Fatma Kara",    Email = "fatma@mail.com",  Password = "1234", Phone = "05557778899", DateOfBirth = new DateTime(1979, 11, 15), Gender = Gender.Female },
            new Patient { FullName = "Ali Doğan",     Email = "ali@mail.com",    Password = "1234", Phone = "05558889900", DateOfBirth = new DateTime(2001, 6, 30),  Gender = Gender.Male },
            new Patient { FullName = "Elif Bulut",    Email = "elif@mail.com",   Password = "1234", Phone = "05559990011", DateOfBirth = new DateTime(1998, 3, 17),  Gender = Gender.Female },
            new Patient { FullName = "Cem Aksoy",     Email = "cem@mail.com",    Password = "1234", Phone = "05551223344", DateOfBirth = new DateTime(1990, 12, 5),  Gender = Gender.Male }
        };
        db.Patients.AddRange(patients);
        db.SaveChanges();

        // ============ RANDEVULAR ============
        var today = DateTime.Today;

        var appointments = new List<Appointment>
        {
            // --- Dr. Elif (DR001) — demo doktoru ---
            new() { DoctorId = doctors[0].Id, PatientId = patients[0].Id, AppointmentDate = today.AddDays(-15).AddHours(10), Status = AppointmentStatus.Completed, Notes = "Kontrol muayenesi yapıldı, EKG normal.", Diagnosis = "Hipertansiyon takibi" },
            new() { DoctorId = doctors[0].Id, PatientId = patients[2].Id, AppointmentDate = today.AddDays(-10).AddHours(11), Status = AppointmentStatus.Completed, Notes = "Şikayetler dinlendi.", Diagnosis = "Stres kaynaklı çarpıntı" },
            new() { DoctorId = doctors[0].Id, PatientId = patients[3].Id, AppointmentDate = today.AddDays(-7).AddHours(14),  Status = AppointmentStatus.Completed, Notes = "Lipit profili değerlendirildi.", Diagnosis = "Yüksek kolesterol" },
            new() { DoctorId = doctors[0].Id, PatientId = patients[4].Id, AppointmentDate = today.AddDays(-4).AddHours(9),   Status = AppointmentStatus.Completed, Notes = "İlk muayene, holter önerildi.", Diagnosis = "Aritmi şüphesi" },
            new() { DoctorId = doctors[0].Id, PatientId = patients[5].Id, AppointmentDate = today.AddDays(-2).AddHours(15),  Status = AppointmentStatus.Completed, Notes = "Holter sonuçları değerlendirildi.", Diagnosis = "Atriyal fibrilasyon" },
            // Bugün — Approved
            new() { DoctorId = doctors[0].Id, PatientId = patients[6].Id, AppointmentDate = today.AddHours(11), Status = AppointmentStatus.Approved },
            new() { DoctorId = doctors[0].Id, PatientId = patients[1].Id, AppointmentDate = today.AddHours(14), Status = AppointmentStatus.Approved },
            // Onay bekleyen
            new() { DoctorId = doctors[0].Id, PatientId = patients[7].Id, AppointmentDate = today.AddDays(2).AddHours(10), Status = AppointmentStatus.Pending },
            new() { DoctorId = doctors[0].Id, PatientId = patients[2].Id, AppointmentDate = today.AddDays(3).AddHours(13), Status = AppointmentStatus.Pending },
            new() { DoctorId = doctors[0].Id, PatientId = patients[6].Id, AppointmentDate = today.AddDays(5).AddHours(15), Status = AppointmentStatus.Pending },

            // --- Dr. Mert (DR002) ---
            new() { DoctorId = doctors[1].Id, PatientId = patients[0].Id, AppointmentDate = today.AddDays(-20).AddHours(11), Status = AppointmentStatus.Completed, Notes = "Akne tedavisi başlandı.", Diagnosis = "Akne vulgaris" },
            new() { DoctorId = doctors[1].Id, PatientId = patients[2].Id, AppointmentDate = today.AddDays(-12).AddHours(13), Status = AppointmentStatus.Completed, Notes = "Cilt analizi yapıldı.", Diagnosis = "Kuru cilt" },
            new() { DoctorId = doctors[1].Id, PatientId = patients[4].Id, AppointmentDate = today.AddDays(1).AddHours(14),   Status = AppointmentStatus.Approved },
            new() { DoctorId = doctors[1].Id, PatientId = patients[6].Id, AppointmentDate = today.AddDays(4).AddHours(11),   Status = AppointmentStatus.Pending },

            // --- Dr. Selin (DR003) ---
            new() { DoctorId = doctors[2].Id, PatientId = patients[3].Id, AppointmentDate = today.AddDays(-8).AddHours(10), Status = AppointmentStatus.Completed, Notes = "Migren atak sıklığı azaldı.", Diagnosis = "Migren" },
            new() { DoctorId = doctors[2].Id, PatientId = patients[1].Id, AppointmentDate = today.AddDays(6).AddHours(11),  Status = AppointmentStatus.Pending },

            // --- Dr. Can (DR004) ---
            new() { DoctorId = doctors[3].Id, PatientId = patients[5].Id, AppointmentDate = today.AddDays(-5).AddHours(9),  Status = AppointmentStatus.Completed, Notes = "Aşı yapıldı.", Diagnosis = "Sağlıklı gelişim" },
            new() { DoctorId = doctors[3].Id, PatientId = patients[7].Id, AppointmentDate = today.AddDays(-3).AddHours(15), Status = AppointmentStatus.Cancelled, Notes = "Hasta iptal etti." },

            // --- Dr. Ayşe (DR005) ---
            new() { DoctorId = doctors[4].Id, PatientId = patients[3].Id, AppointmentDate = today.AddDays(-2).AddHours(10), Status = AppointmentStatus.Completed, Notes = "Diz MR sonucu değerlendirildi.", Diagnosis = "Menisküs yırtığı" }
        };
        db.Appointments.AddRange(appointments);
        db.SaveChanges();

        // ============ REÇETELER ============
        db.Prescriptions.AddRange(
            new Prescription { AppointmentId = appointments[0].Id, Medication = "Concor 5mg",    Dosage = "Günde 1",  Instructions = "Sabah aç karnına alın." },
            new Prescription { AppointmentId = appointments[0].Id, Medication = "Aspirin 100mg", Dosage = "Günde 1",  Instructions = "Yemekten sonra alın." },
            new Prescription { AppointmentId = appointments[2].Id, Medication = "Lipitor 20mg",  Dosage = "Günde 1",  Instructions = "Akşam yemeğinden sonra." },
            new Prescription { AppointmentId = appointments[3].Id, Medication = "Beloc 50mg",    Dosage = "Günde 2",  Instructions = "Sabah ve akşam." },
            new Prescription { AppointmentId = appointments[10].Id, Medication = "Aknetrent 10mg", Dosage = "Günde 1", Instructions = "Yemekle birlikte." },
            new Prescription { AppointmentId = appointments[14].Id, Medication = "Imigran 50mg", Dosage = "Krizde 1 tablet", Instructions = "Günde maks. 2 tablet." }
        );

        // ============ YORUMLAR / PUANLAR ============
        db.Reviews.AddRange(
            // Dr. Elif (yüksek puanlı, çok yorum)
            new Review { DoctorId = doctors[0].Id, PatientId = patients[0].Id, Score = 5, Comment = "Çok ilgili ve detaylı muayene yaptı.",         CreatedAt = today.AddDays(-14) },
            new Review { DoctorId = doctors[0].Id, PatientId = patients[2].Id, Score = 5, Comment = "Sorularımı sabırla dinledi, memnun kaldım.",   CreatedAt = today.AddDays(-9) },
            new Review { DoctorId = doctors[0].Id, PatientId = patients[3].Id, Score = 4, Comment = "Profesyonel yaklaşım.",                         CreatedAt = today.AddDays(-6) },
            new Review { DoctorId = doctors[0].Id, PatientId = patients[4].Id, Score = 5, Comment = "Güven verici bir doktor.",                      CreatedAt = today.AddDays(-3) },

            new Review { DoctorId = doctors[1].Id, PatientId = patients[0].Id, Score = 4, Comment = "Tedavi planı işe yaradı.",                      CreatedAt = today.AddDays(-18) },
            new Review { DoctorId = doctors[1].Id, PatientId = patients[2].Id, Score = 5, Comment = "Cildim çok düzeldi, teşekkürler.",              CreatedAt = today.AddDays(-11) },

            new Review { DoctorId = doctors[2].Id, PatientId = patients[3].Id, Score = 4, Comment = "Migrenim kontrol altına alındı.",               CreatedAt = today.AddDays(-7) },
            new Review { DoctorId = doctors[3].Id, PatientId = patients[5].Id, Score = 5, Comment = "Çocuklara çok iyi davranıyor.",                 CreatedAt = today.AddDays(-4) },
            new Review { DoctorId = doctors[4].Id, PatientId = patients[3].Id, Score = 5, Comment = "Net teşhis, ameliyatsız iyileştik.",            CreatedAt = today.AddDays(-1) }
        );

        // ============ BİLDİRİMLER (DR001 için onay bekleyenler) ============
        db.Notifications.AddRange(
            new Notification { TargetRole = "Doctor",  TargetUserId = doctors[0].Id, Message = $"Yeni randevu talebi: {patients[7].FullName} - {today.AddDays(2).AddHours(10):dd.MM.yyyy HH:mm}", CreatedAt = today.AddDays(-1).AddHours(14) },
            new Notification { TargetRole = "Doctor",  TargetUserId = doctors[0].Id, Message = $"Yeni randevu talebi: {patients[2].FullName} - {today.AddDays(3).AddHours(13):dd.MM.yyyy HH:mm}", CreatedAt = today.AddHours(-6) },
            new Notification { TargetRole = "Doctor",  TargetUserId = doctors[0].Id, Message = $"Yeni randevu talebi: {patients[6].FullName} - {today.AddDays(5).AddHours(15):dd.MM.yyyy HH:mm}", CreatedAt = today.AddHours(-2) },
            new Notification { TargetRole = "Patient", TargetUserId = patients[6].Id, Message = $"Randevun onaylandı: {today.AddHours(11):dd.MM.yyyy HH:mm}", CreatedAt = today.AddDays(-1) },
            new Notification { TargetRole = "Patient", TargetUserId = patients[1].Id, Message = $"Randevun onaylandı: {today.AddHours(14):dd.MM.yyyy HH:mm}", CreatedAt = today.AddDays(-1) }
        );

        db.SaveChanges();
    }
}

using ClinicMvc.Models;

namespace ClinicMvc.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Departments.Any()) return;

        var kardiyoloji = new Department { Name = "Kardiyoloji" };
        var dermatoloji = new Department { Name = "Dermatoloji" };
        var noroloji    = new Department { Name = "Nöroloji" };
        var pediatri    = new Department { Name = "Pediatri" };
        db.Departments.AddRange(kardiyoloji, dermatoloji, noroloji, pediatri);
        db.SaveChanges();

        var doctors = new List<Doctor>
        {
            new() { FullName = "Dr. Elif Yılmaz", DoctorNumber = "DR001", Password = "1234", DepartmentId = kardiyoloji.Id, RoomNumber = "101", MaxAppointments = 8,  Email = "elif.yilmaz@klinik.com",  Bio = "Kardiyoloji uzmanı." },
            new() { FullName = "Dr. Mert Kaya",   DoctorNumber = "DR002", Password = "1234", DepartmentId = dermatoloji.Id, RoomNumber = "102", MaxAppointments = 12, Email = "mert.kaya@klinik.com",    Bio = "Cilt hastalıkları uzmanı." },
            new() { FullName = "Dr. Selin Demir", DoctorNumber = "DR003", Password = "1234", DepartmentId = noroloji.Id,    RoomNumber = "201", MaxAppointments = 6,  Email = "selin.demir@klinik.com",  Bio = "Nöroloji uzmanı." },
            new() { FullName = "Dr. Can Aydın",   DoctorNumber = "DR004", Password = "1234", DepartmentId = pediatri.Id,    RoomNumber = "202", MaxAppointments = 10, Email = "can.aydin@klinik.com",    Bio = "Çocuk sağlığı uzmanı." },
        };
        db.Doctors.AddRange(doctors);
        db.SaveChanges();

        var patients = new List<Patient>
        {
            new() { FullName = "Ayşe Çelik",  Email = "ayse@mail.com",  Password = "1234", Phone = "05551112233", DateOfBirth = new DateTime(1995, 4, 12), Gender = Gender.Female },
            new() { FullName = "Burak Şahin", Email = "burak@mail.com", Password = "1234", Phone = "05554445566", DateOfBirth = new DateTime(1988, 9, 3),  Gender = Gender.Male },
        };
        db.Patients.AddRange(patients);
        db.SaveChanges();

        var appt = new Appointment
        {
            DoctorId = doctors[0].Id,
            PatientId = patients[0].Id,
            AppointmentDate = DateTime.Now.AddDays(-3),
            Status = AppointmentStatus.Completed,
            Notes = "Kontrol muayenesi."
        };
        db.Appointments.Add(appt);
        db.SaveChanges();

        db.Prescriptions.Add(new Prescription
        {
            AppointmentId = appt.Id,
            Medication = "Aspirin 100mg",
            Dosage = "Günde 1",
            Instructions = "Yemekten sonra alınız."
        });

        db.Reviews.Add(new Review
        {
            DoctorId = doctors[0].Id,
            PatientId = patients[0].Id,
            Score = 5,
            Comment = "Çok ilgiliydi."
        });

        db.SaveChanges();
    }
}

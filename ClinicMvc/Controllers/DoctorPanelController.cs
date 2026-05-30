using ClinicMvc.Data;
using ClinicMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ClinicMvc.Controllers;

public class DoctorPanelController : Controller
{
    private readonly AppDbContext _db;
    public DoctorPanelController(AppDbContext db) => _db = db;

    private int DoctorId => HttpContext.Session.GetInt32("UserId") ?? 0;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetString("Role") != "Doctor")
            context.Result = RedirectToAction("DoctorLogin", "Account");
        base.OnActionExecuting(context);
    }

    // Doktorun kendi randevuları
public IActionResult Index(string? search, int page = 1)
    {
        int pageSize = 10;
        var q = _db.Appointments
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == DoctorId
                     && a.Status != AppointmentStatus.Pending); // bekleyenler ayrı ekranda
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(a => a.Patient!.FullName.Contains(search));

        int total = q.Count();
        var items = q.OrderByDescending(a => a.AppointmentDate)
                     .Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.Search = search;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        return View(items);
    }

    public IActionResult Pending()
    {
        var list = _db.Appointments
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == DoctorId && a.Status == AppointmentStatus.Pending)
            .OrderBy(a => a.AppointmentDate)
            .ToList();
        return View(list);
    }

    [HttpPost]
    public IActionResult Approve(int id)
    {
        var appt = _db.Appointments.FirstOrDefault(a => a.Id == id && a.DoctorId == DoctorId);
        if (appt != null) { appt.Status = AppointmentStatus.Approved; 
        _db.Notifications.Add(new Notification
            {
                TargetRole = "Patient",
                TargetUserId = appt.PatientId,
                Message = $"Randevun onaylandı: {appt.AppointmentDate:dd.MM.yyyy HH:mm}"
            });
        _db.SaveChanges(); }
        return RedirectToAction("Pending");
    }

    [HttpPost]
    public IActionResult CancelAppt(int id)
    {
        var appt = _db.Appointments.FirstOrDefault(a => a.Id == id && a.DoctorId == DoctorId);
        if (appt != null) { appt.Status = AppointmentStatus.Cancelled; 
        _db.Notifications.Add(new Notification
            {
                TargetRole = "Patient",
                TargetUserId = appt.PatientId,
                Message = $"Randevun reddedildi: {appt.AppointmentDate:dd.MM.yyyy HH:mm}"
            });
        _db.SaveChanges(); }
        return RedirectToAction("Pending");
    }

    [HttpPost]
    public IActionResult SetStatus(int id, AppointmentStatus status)
    {
        var appt = _db.Appointments.FirstOrDefault(a => a.Id == id && a.DoctorId == DoctorId);
        if (appt != null) { appt.Status = status; _db.SaveChanges(); }
        return RedirectToAction("Index");
    }

    // Reçete sayfası
    [HttpGet]
    public IActionResult Prescribe(int id)
    {
        var appt = _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Prescriptions)
            .FirstOrDefault(a => a.Id == id && a.DoctorId == DoctorId);
        if (appt == null) return RedirectToAction("Index");
        return View(appt);
    }

    [HttpPost]
    public IActionResult AddPrescription(Prescription model)
    {
        var appt = _db.Appointments.FirstOrDefault(a => a.Id == model.AppointmentId && a.DoctorId == DoctorId);
        if (appt == null) return RedirectToAction("Index");

        if (!ModelState.IsValid)
        {
            var full = _db.Appointments.Include(a => a.Patient).Include(a => a.Prescriptions)
                          .First(a => a.Id == model.AppointmentId);
            return View("Prescribe", full);
        }
        _db.Prescriptions.Add(model);
        _db.SaveChanges();
        return RedirectToAction("Details", new { id = model.AppointmentId });
    }

    // Ayarlar (sadece doktorun değiştirebileceği alanlar)
// ---- AYARLAR: oda, max randevu, mesai ----
    [HttpGet]
    public IActionResult Settings()
    {
        var doc = _db.Doctors.Find(DoctorId);
        if (doc == null) return RedirectToAction("Index");
        return View(doc);
    }

    [HttpPost]
    public IActionResult Settings(Doctor model)
    {
        var doc = _db.Doctors.Find(DoctorId);
        if (doc == null) return RedirectToAction("Index");
        doc.RoomNumber = model.RoomNumber;
        doc.MaxAppointments = model.MaxAppointments < 1 ? 1 : model.MaxAppointments;
        doc.WorkStartHour = model.WorkStartHour;
        doc.WorkEndHour = model.WorkEndHour;
        _db.SaveChanges();
        TempData["Msg"] = "Ayarlar kaydedildi.";
        return RedirectToAction("Settings");
    }

    // ---- PROFİL: isim, branş (salt-okunur), hakkında, şifre ----
    [HttpGet]
    public IActionResult Profile()
    {
        var doc = _db.Doctors.Include(d => d.Department).FirstOrDefault(d => d.Id == DoctorId);
        if (doc == null) return RedirectToAction("Index");
        return View(doc);
    }

    [HttpPost]
    public IActionResult UpdateBio(string bio)
    {
        var doc = _db.Doctors.Find(DoctorId);
        if (doc == null) return RedirectToAction("Index");
        doc.Bio = bio ?? "";
        _db.SaveChanges();
        TempData["Msg"] = "Hakkında güncellendi.";
        return RedirectToAction("Profile");
    }

    [HttpPost]
    public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        var doc = _db.Doctors.Find(DoctorId);
        if (doc == null) return RedirectToAction("Index");

        if (doc.Password != oldPassword)
            TempData["PwError"] = "Eski şifre yanlış.";
        else if (string.IsNullOrWhiteSpace(newPassword))
            TempData["PwError"] = "Yeni şifre boş olamaz.";
        else if (newPassword != confirmPassword)
            TempData["PwError"] = "Yeni şifreler eşleşmiyor.";
        else
        {
            doc.Password = newPassword;
            _db.SaveChanges();
            TempData["Msg"] = "Şifre değiştirildi.";
        }
        return RedirectToAction("Profile");
    }
    // Randevu detay
[HttpGet]
    public IActionResult Details(int id)
    {
        var appt = _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Prescriptions)
            .FirstOrDefault(a => a.Id == id && a.DoctorId == DoctorId);
        if (appt == null) return RedirectToAction("Index");

        var history = _db.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d!.Department)
            .Where(a => a.PatientId == appt.PatientId && a.Id != appt.Id
                && a.Status == AppointmentStatus.Completed)
            .OrderByDescending(a => a.AppointmentDate)
            .Take(10)
            .ToList();

        return View(new ClinicMvc.ViewModels.AppointmentDetailViewModel
        {
            Appointment = appt,
            History = history
        });
    }

    // Doktor: durum + not + teşhis doldurur
    [HttpPost]
    public IActionResult UpdateDetails(int id, AppointmentStatus status, string notes, string diagnosis)
    {
        var appt = _db.Appointments.FirstOrDefault(a => a.Id == id && a.DoctorId == DoctorId);
        if (appt == null) return RedirectToAction("Index");
        appt.Status = status;
        appt.Notes = notes ?? "";
        appt.Diagnosis = diagnosis ?? "";
        _db.SaveChanges();
        TempData["Msg"] = "Randevu güncellendi.";
        return RedirectToAction("Details", new { id });
    }
}
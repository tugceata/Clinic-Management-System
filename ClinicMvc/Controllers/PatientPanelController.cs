using ClinicMvc.Data;
using ClinicMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicMvc.Controllers;

public class PatientPanelController : Controller
{
    private readonly AppDbContext _db;
    public PatientPanelController(AppDbContext db) => _db = db;

    private int PatientId => HttpContext.Session.GetInt32("UserId") ?? 0;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetString("Role") != "Patient")
            context.Result = RedirectToAction("Login", "Account");
        base.OnActionExecuting(context);
    }

    public IActionResult Doctors(int? departmentId, string? search)
    {
        var q = _db.Doctors.Include(d => d.Department).Where(d => d.IsActive);
        if (departmentId.HasValue) q = q.Where(d => d.DepartmentId == departmentId.Value);
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(d => d.FullName.Contains(search));

        var doctors = q.OrderBy(d => d.FullName).ToList();

        var ratings = _db.Reviews
            .GroupBy(r => r.DoctorId)
            .Select(g => new { DoctorId = g.Key, Avg = g.Average(x => (double)x.Score), Cnt = g.Count() })
            .ToList();

        ViewBag.AvgDict = ratings.ToDictionary(r => r.DoctorId, r => r.Avg);
        ViewBag.CntDict = ratings.ToDictionary(r => r.DoctorId, r => r.Cnt);
        ViewBag.Departments = new SelectList(_db.Departments.ToList(), "Id", "Name", departmentId);
        ViewBag.Search = search;
        ViewBag.DepartmentId = departmentId;
        return View(doctors);
    }

public IActionResult DoctorDetails(int id)
    {
        var doc = _db.Doctors.Include(d => d.Department).FirstOrDefault(d => d.Id == id && d.IsActive);
        if (doc == null) return RedirectToAction("Doctors");

        var reviews = _db.Reviews.Include(r => r.Patient)
            .Where(r => r.DoctorId == id).OrderByDescending(r => r.CreatedAt).ToList();

        var vm = new ClinicMvc.ViewModels.DoctorDetailViewModel
        {
            Doctor = doc,
            Reviews = reviews,
            ReviewCount = reviews.Count,
            AverageRating = reviews.Any() ? reviews.Average(r => r.Score) : 0,
            HasCompleted = _db.Appointments.Any(a => a.DoctorId == id && a.PatientId == PatientId && a.Status == AppointmentStatus.Completed),
            HasReview = _db.Reviews.Any(r => r.DoctorId == id && r.PatientId == PatientId)
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Book(int doctorId)
    {
        var doc = _db.Doctors.Include(d => d.Department).FirstOrDefault(d => d.Id == doctorId && d.IsActive);
        if (doc == null) return RedirectToAction("Doctors");
        return View(doc);
    }

    [HttpPost]
    public IActionResult Book(int doctorId, DateTime date, int hour)
    {
        var doc = _db.Doctors.FirstOrDefault(d => d.Id == doctorId && d.IsActive);
        if (doc == null) return RedirectToAction("Doctors");

        if (date.Date < DateTime.Today)
        { TempData["Error"] = "Geçmiş bir tarih seçilemez."; return RedirectToAction("Book", new { doctorId }); }

        if (hour < doc.WorkStartHour || hour >= doc.WorkEndHour)
        { TempData["Error"] = $"Doktor sadece {doc.WorkStartHour:00}:00 - {doc.WorkEndHour:00}:00 arasında çalışıyor."; return RedirectToAction("Book", new { doctorId }); }

        var slot = date.Date.AddHours(hour);
        if (slot < DateTime.Now)
        { TempData["Error"] = "Geçmiş bir saat seçilemez."; return RedirectToAction("Book", new { doctorId }); }

        bool clash = _db.Appointments.Any(a => a.DoctorId == doctorId
            && a.AppointmentDate == slot && a.Status != AppointmentStatus.Cancelled);
        if (clash)
        { TempData["Error"] = "Bu saat dolu, başka bir saat seç."; return RedirectToAction("Book", new { doctorId }); }

        int dayCount = _db.Appointments.Count(a => a.DoctorId == doctorId
            && a.AppointmentDate.Date == date.Date && a.Status != AppointmentStatus.Cancelled);
        if (dayCount >= doc.MaxAppointments)
        { TempData["Error"] = "Doktorun bu güne ait randevu limiti dolu."; return RedirectToAction("Book", new { doctorId }); }

        _db.Appointments.Add(new Appointment
        {
            DoctorId = doctorId, PatientId = PatientId,
            AppointmentDate = slot, Status = AppointmentStatus.Pending
        });
        _db.SaveChanges();
        TempData["Msg"] = "Randevu talebin oluşturuldu, doktor onayını bekliyor.";
        return RedirectToAction("MyAppointments");
    }

    public IActionResult MyAppointments()
    {
        var list = _db.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d!.Department)
            .Include(a => a.Prescriptions)
            .Where(a => a.PatientId == PatientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToList();

        ViewBag.ReviewedDoctorIds = _db.Reviews
            .Where(r => r.PatientId == PatientId)
            .Select(r => r.DoctorId)
            .ToHashSet();
        return View(list);
    }
    [HttpGet]
    public IActionResult Review(int doctorId)
    {
        var doc = _db.Doctors.Include(d => d.Department).FirstOrDefault(d => d.Id == doctorId);
        if (doc == null) return RedirectToAction("MyAppointments");

        bool hasCompleted = _db.Appointments.Any(a => a.DoctorId == doctorId
            && a.PatientId == PatientId && a.Status == AppointmentStatus.Completed);
        if (!hasCompleted)
        {
            TempData["Error"] = "Sadece tamamlanmış randevu yaptığın doktora puan verebilirsin.";
            return RedirectToAction("MyAppointments");
        }

        ViewBag.Existing = _db.Reviews.FirstOrDefault(r => r.DoctorId == doctorId && r.PatientId == PatientId);
        return View(doc);
    }

    [HttpPost]
    public IActionResult Review(int doctorId, int score, string? comment)
    {
        bool hasCompleted = _db.Appointments.Any(a => a.DoctorId == doctorId
            && a.PatientId == PatientId && a.Status == AppointmentStatus.Completed);
        if (!hasCompleted)
        {
            TempData["Error"] = "Sadece tamamlanmış randevu yaptığın doktora puan verebilirsin.";
            return RedirectToAction("MyAppointments");
        }
        if (score < 1 || score > 5)
        {
            TempData["Error"] = "Puan 1-5 arası olmalı.";
            return RedirectToAction("Review", new { doctorId });
        }

        var existing = _db.Reviews.FirstOrDefault(r => r.DoctorId == doctorId && r.PatientId == PatientId);
        if (existing != null)
        {
            existing.Score = score;
            existing.Comment = comment ?? "";
            existing.CreatedAt = DateTime.Now;
            _db.SaveChanges();
            TempData["Msg"] = "Yorumun güncellendi.";
        }
        else
        {
            _db.Reviews.Add(new Review
            {
                DoctorId = doctorId, PatientId = PatientId,
                Score = score, Comment = comment ?? ""
            });
            _db.SaveChanges();
            TempData["Msg"] = "Yorumun kaydedildi, teşekkürler!";
        }
        return RedirectToAction("MyAppointments");
    }
    [HttpGet]
    public IActionResult AvailableSlots(int doctorId, DateTime date)
    {
        var doc = _db.Doctors.FirstOrDefault(d => d.Id == doctorId && d.IsActive);
        if (doc == null) return Json(new int[0]);

        var taken = _db.Appointments
            .Where(a => a.DoctorId == doctorId
                && a.AppointmentDate.Date == date.Date
                && a.Status != AppointmentStatus.Cancelled)
            .Select(a => a.AppointmentDate.Hour)
            .ToHashSet();

        var now = DateTime.Now;
        var slots = new List<int>();
        for (int h = doc.WorkStartHour; h < doc.WorkEndHour; h++)
        {
            if (taken.Contains(h)) continue;
            if (date.Date.AddHours(h) < now) continue;
            slots.Add(h);
        }
        return Json(slots);
    }
    [HttpPost]
    public IActionResult CancelAppt(int id)
    {
        var appt = _db.Appointments.FirstOrDefault(a => a.Id == id && a.PatientId == PatientId);
        if (appt == null) return RedirectToAction("MyAppointments");

        if (appt.Status != AppointmentStatus.Pending && appt.Status != AppointmentStatus.Approved)
        {
            TempData["Error"] = "Bu randevu iptal edilemez.";
            return RedirectToAction("MyAppointments");
        }

        // 24 saatten az kala iptal yok
        if ((appt.AppointmentDate - DateTime.Now).TotalHours < 24)
        {
            TempData["Error"] = "Randevuya 24 saatten az kaldığı için iptal edilemez.";
            return RedirectToAction("MyAppointments");
        }

        appt.Status = AppointmentStatus.Cancelled;
        _db.SaveChanges();
        TempData["Msg"] = "Randevunuz iptal edildi.";
        return RedirectToAction("MyAppointments");
    }

}
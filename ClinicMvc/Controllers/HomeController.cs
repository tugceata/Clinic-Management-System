using System.Diagnostics;
using ClinicMvc.Data;
using ClinicMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicMvc.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role == "Doctor")
        {
            int docId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var today = DateTime.Today;
            int diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            var weekStart = today.AddDays(-diff);

            ViewBag.TodayCount = _db.Appointments.Count(a => a.DoctorId == docId
                && a.AppointmentDate.Date == today
                && (a.Status == AppointmentStatus.Approved || a.Status == AppointmentStatus.Completed));
            ViewBag.PendingCount = _db.Appointments.Count(a => a.DoctorId == docId
                && a.Status == AppointmentStatus.Pending);
            ViewBag.WeekPatientCount = _db.Appointments
                .Where(a => a.DoctorId == docId
                    && a.AppointmentDate >= weekStart
                    && a.AppointmentDate < weekStart.AddDays(7))
                .Select(a => a.PatientId).Distinct().Count();
            ViewBag.CompletedCount = _db.Appointments.Count(a => a.DoctorId == docId
                && a.Status == AppointmentStatus.Completed);
        }
        else if (role == "Patient")
        {
            int patId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var now = DateTime.Now;
            ViewBag.UpcomingCount = _db.Appointments.Count(a => a.PatientId == patId
                && a.AppointmentDate >= now
                && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved));
            ViewBag.PendingCount = _db.Appointments.Count(a => a.PatientId == patId && a.Status == AppointmentStatus.Pending);
            ViewBag.CompletedCount = _db.Appointments.Count(a => a.PatientId == patId && a.Status == AppointmentStatus.Completed);
        }
        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
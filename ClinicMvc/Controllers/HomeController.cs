using System.Diagnostics;
using ClinicMvc.Data;
using ClinicMvc.Models;
using ClinicMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicMvc.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("Role");
        var vm = new HomeIndexViewModel
        {
            Role = role,
            Name = HttpContext.Session.GetString("UserName")
        };

        if (role == "Doctor")
        {
            int docId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var today = DateTime.Today;
            int diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            var weekStart = today.AddDays(-diff);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var doc = _db.Doctors.Find(docId);

            var reviewsQ = _db.Reviews.Where(r => r.DoctorId == docId);

            vm.Doctor = new DoctorDashboardViewModel
            {
                TodayCount = _db.Appointments.Count(a => a.DoctorId == docId
                    && a.AppointmentDate.Date == today
                    && (a.Status == AppointmentStatus.Approved || a.Status == AppointmentStatus.Completed)),
                PendingCount = _db.Appointments.Count(a => a.DoctorId == docId
                    && a.Status == AppointmentStatus.Pending),
                WeekPatientCount = _db.Appointments
                    .Where(a => a.DoctorId == docId
                        && a.AppointmentDate >= weekStart
                        && a.AppointmentDate < weekStart.AddDays(7))
                    .Select(a => a.PatientId).Distinct().Count(),
                CompletedCount = _db.Appointments.Count(a => a.DoctorId == docId
                    && a.Status == AppointmentStatus.Completed),

                TotalReviews = reviewsQ.Count(),
                AverageRating = reviewsQ.Any() ? reviewsQ.Average(r => (double)r.Score) : 0,
                ThisMonthPatientCount = _db.Appointments
                    .Where(a => a.DoctorId == docId
                        && a.AppointmentDate >= monthStart
                        && a.Status == AppointmentStatus.Completed)
                    .Select(a => a.PatientId).Distinct().Count(),
                RecentReviews = _db.Reviews
                    .Include(r => r.Patient)
                    .Where(r => r.DoctorId == docId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new ReviewSummary
                    {
                        PatientName = r.Patient!.FullName,
                        Score = r.Score,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToList(),
                MaxAppointments = doc?.MaxAppointments ?? 0,
                TodayUsed = _db.Appointments.Count(a => a.DoctorId == docId
                    && a.AppointmentDate.Date == today
                    && a.Status != AppointmentStatus.Cancelled)
            };
        }
        else if (role == "Patient")
        {
            int patId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var now = DateTime.Now;

            var grouped = _db.Appointments
                .Where(a => a.PatientId == patId && a.Status == AppointmentStatus.Completed)
                .GroupBy(a => a.DoctorId)
                .Select(g => new { DoctorId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            string? doctorName = grouped == null ? null :
                _db.Doctors.Where(d => d.Id == grouped.DoctorId).Select(d => d.FullName).FirstOrDefault();

            vm.Patient = new PatientDashboardViewModel
            {
                UpcomingCount = _db.Appointments.Count(a => a.PatientId == patId
                    && a.AppointmentDate >= now
                    && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved)),
                PendingCount = _db.Appointments.Count(a => a.PatientId == patId && a.Status == AppointmentStatus.Pending),
                CompletedCount = _db.Appointments.Count(a => a.PatientId == patId && a.Status == AppointmentStatus.Completed),
                TotalCount = _db.Appointments.Count(a => a.PatientId == patId),
                MostVisitedDoctor = doctorName,
                MostVisitedCount = grouped?.Count ?? 0
            };
        }
        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
using ClinicMvc.Data;
using ClinicMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ClinicMvc.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    public AccountController(AppDbContext db) => _db = db;

    private const string AdminUser = "admin";
    private const string AdminPass = "admin123";

    // ---------- HASTA (varsayılan giriş) ----------
    [HttpGet] public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var p = _db.Patients.FirstOrDefault(x => x.Email == email && x.Password == password);
        if (p != null) { SetSession("Patient", p.Id, p.FullName); return RedirectToAction("Index", "Home"); }
        TempData["Error"] = "E-posta veya şifre hatalı.";
        return RedirectToAction("Login");
    }

    // ---------- ADMIN ----------
    [HttpGet] public IActionResult AdminLogin() => View();

    [HttpPost]
    public IActionResult AdminLogin(string username, string password)
    {
        if (username == AdminUser && password == AdminPass) { SetSession("Admin", 0, "Yönetici"); return RedirectToAction("Index", "Home"); }
        TempData["Error"] = "Yönetici bilgileri hatalı.";
        return RedirectToAction("AdminLogin");
    }

    // ---------- DOKTOR ----------
    [HttpGet] public IActionResult DoctorLogin() => View();

    [HttpPost]
    public IActionResult DoctorLogin(string doctorNumber, string password)
    {
        var doc = _db.Doctors.FirstOrDefault(d => d.DoctorNumber == doctorNumber && d.Password == password && d.IsActive);
        if (doc != null) { SetSession("Doctor", doc.Id, doc.FullName); return RedirectToAction("Index", "Home"); }
        TempData["Error"] = "Doktor no veya şifre hatalı.";
        return RedirectToAction("DoctorLogin");
    }

    // ---------- HASTA KAYDI ----------
    [HttpGet] public IActionResult Register() => View(new Patient());

    [HttpPost]
    public IActionResult Register(Patient model)
    {
        if (!ModelState.IsValid) return View(model);
        if (_db.Patients.Any(x => x.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Bu e-posta zaten kayıtlı.");
            return View(model);
        }
        _db.Patients.Add(model);
        _db.SaveChanges();
        SetSession("Patient", model.Id, model.FullName);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private void SetSession(string role, int id, string name)
    {
        HttpContext.Session.SetString("Role", role);
        HttpContext.Session.SetInt32("UserId", id);
        HttpContext.Session.SetString("UserName", name);
    }
}
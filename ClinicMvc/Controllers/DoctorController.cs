using ClinicMvc.Data;
using ClinicMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicMvc.Controllers;

public class DoctorController : Controller
{
    private readonly AppDbContext _db;
    public DoctorController(AppDbContext db) => _db = db;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            context.Result = RedirectToAction("AdminLogin", "Account");
        base.OnActionExecuting(context);
    }

    public IActionResult Index()
    {
        var docs = _db.Doctors.Include(d => d.Department).ToList();
        return View(docs);
    }

    private void LoadDepartments(int? selected = null)
        => ViewBag.Departments = new SelectList(_db.Departments.ToList(), "Id", "Name", selected);

    [HttpGet]
    public IActionResult Create() { LoadDepartments(); return View(new Doctor()); }

    [HttpPost]
    public IActionResult Create(Doctor model)
    {
        if (!ModelState.IsValid) { LoadDepartments(model.DepartmentId); return View(model); }
        if (_db.Doctors.Any(d => d.DoctorNumber == model.DoctorNumber))
        {
            ModelState.AddModelError("DoctorNumber", "Bu doktor no zaten kullanılıyor.");
            LoadDepartments(model.DepartmentId);
            return View(model);
        }
        _db.Doctors.Add(model);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var doc = _db.Doctors.Find(id);
        if (doc == null) return RedirectToAction("Index");
        LoadDepartments(doc.DepartmentId);
        return View(doc);
    }

    [HttpPost]
    public IActionResult Edit(Doctor model)
    {
        if (!ModelState.IsValid) { LoadDepartments(model.DepartmentId); return View(model); }
        _db.Doctors.Update(model);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var doc = _db.Doctors.Include(d => d.Department).FirstOrDefault(d => d.Id == id);
        if (doc == null) return RedirectToAction("Index");
        return View(doc);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var doc = _db.Doctors.FirstOrDefault(d => d.Id == id);
        if (doc == null) return RedirectToAction("Index");

        bool hasRecords = _db.Appointments.Any(a => a.DoctorId == id) || _db.Reviews.Any(r => r.DoctorId == id);
        if (hasRecords)
        {
            doc.IsActive = false;     // geçmişi olan doktor silinmez, pasifleştirilir
            _db.SaveChanges();
            TempData["Error"] = "Doktorun geçmiş kayıtları var; silinmek yerine pasifleştirildi.";
        }
        else
        {
            _db.Doctors.Remove(doc);
            _db.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
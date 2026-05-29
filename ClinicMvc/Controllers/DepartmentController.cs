using ClinicMvc.Data;
using ClinicMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ClinicMvc.Controllers;

public class DepartmentController : Controller
{
    private readonly AppDbContext _db;
    public DepartmentController(AppDbContext db) => _db = db;

    // Sadece admin erişebilsin
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            context.Result = RedirectToAction("AdminLogin", "Account");
        base.OnActionExecuting(context);
    }

    public IActionResult Index()
    {
        var list = _db.Departments.Include(d => d.Doctors).ToList();
        return View(list);
    }

    [HttpGet] public IActionResult Create() => View(new Department());

    [HttpPost]
    public IActionResult Create(Department model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Departments.Add(model);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var dep = _db.Departments.Find(id);
        if (dep == null) return RedirectToAction("Index");
        return View(dep);
    }

    [HttpPost]
    public IActionResult Edit(Department model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Departments.Update(model);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var dep = _db.Departments.Include(d => d.Doctors).FirstOrDefault(d => d.Id == id);
        if (dep == null) return RedirectToAction("Index");
        return View(dep);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var dep = _db.Departments.Include(d => d.Doctors).FirstOrDefault(d => d.Id == id);
        if (dep == null) return RedirectToAction("Index");

        if (dep.Doctors.Any())
        {
            TempData["Error"] = "Bu branşta doktor var, önce doktorları taşı/sil.";
            return RedirectToAction("Index");
        }
        _db.Departments.Remove(dep);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
}
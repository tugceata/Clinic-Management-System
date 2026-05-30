using ClinicMvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClinicMvc.Controllers;

public class NotificationController : Controller
{
    private readonly AppDbContext _db;
    public NotificationController(AppDbContext db) => _db = db;

    private string? Role => HttpContext.Session.GetString("Role");
    private int Uid => HttpContext.Session.GetInt32("UserId") ?? 0;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (Role != "Doctor" && Role != "Patient")
            context.Result = RedirectToAction("Index", "Home");
        base.OnActionExecuting(context);
    }

    public IActionResult Index()
    {
        var list = _db.Notifications
            .Where(n => n.TargetRole == Role && n.TargetUserId == Uid)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToList();
        return View(list);
    }

    [HttpPost]
    public IActionResult MarkRead(int id)
    {
        var n = _db.Notifications.FirstOrDefault(x => x.Id == id && x.TargetRole == Role && x.TargetUserId == Uid);
        if (n != null) { n.IsRead = true; _db.SaveChanges(); }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult MarkAllRead()
    {
        var unread = _db.Notifications.Where(n => n.TargetRole == Role && n.TargetUserId == Uid && !n.IsRead).ToList();
        foreach (var n in unread) n.IsRead = true;
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
}
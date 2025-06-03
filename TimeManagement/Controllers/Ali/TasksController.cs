// ✅ TasksController.cs (ASP.NET Core MVC)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeManagement.Models;
using Task = TimeManagement.Models.Task;

namespace TimeManagement.Controllers.Ali
{
    public class TasksController : Controller
    {
        private readonly MyDbContext _context;

        public TasksController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Board");
        }

        // GET: /Ali/Tasks/Board
        public IActionResult Board()
        {
            ViewBag.Todo = _context.Tasks
                .Where(t => t.Status == "todo")
                .Include(t => t.Project)
                .ToList();

            ViewBag.InProgress = _context.Tasks
                .Where(t => t.Status == "in-progress")
                .Include(t => t.Project)
                .ToList();

            ViewBag.NeedsReview = _context.Tasks
                .Where(t => t.Status == "review")
                .Include(t => t.Project)
                .ToList();

            ViewBag.Completed = _context.Tasks
                .Where(t => t.Status == "completed")
                .Include(t => t.Project)
                .ToList();

            ViewBag.Projects = new SelectList(_context.Projects.ToList(), "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "Username");

            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("ProjectId,TaskName,Category,StartDate,DueDate,AssignedTo,Priority,Description")] Task task)
        {
            // تعيين الحالة بشكل صريح
            task.Status = "todo";
            task.CreatedAt = DateTime.Now;
            task.UpdatedAt = DateTime.Now;
            task.CompletionPercentage = 0;

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return RedirectToAction("Board");
        }


        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var task = _context.Tasks.Find(id);
            if (task == null) return NotFound();

            task.Status = status;
            task.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Ok();
        }

    }
}
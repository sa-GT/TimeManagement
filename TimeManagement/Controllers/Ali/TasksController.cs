using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeManagement.Models;
using TimeManagement.Services;
using Task = TimeManagement.Models.Task;

namespace TimeManagement.Controllers.Ali
{
    public class TasksController : Controller
    {
        private readonly MyDbContext _context;
        private readonly EmailService _emailService;

        public TasksController(MyDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Board");
        }

        public IActionResult Board(int? projectId = null)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
                return RedirectToAction("Login", "Users");

            var ownedProjects = _context.Projects
                .Where(p => p.ManagerId == currentUserId)
                .ToList();

            var ownedProjectIds = ownedProjects.Select(p => p.Id).ToList();
            var selectedProjectId = projectId ?? ownedProjectIds.FirstOrDefault();

            ViewBag.Todo = _context.Tasks
                .Where(t => t.Status == "todo" && t.ProjectId == selectedProjectId)
                .Include(t => t.Project)
                .ToList();

            ViewBag.InProgress = _context.Tasks
                .Where(t => t.Status == "in-progress" && t.ProjectId == selectedProjectId)
                .Include(t => t.Project)
                .ToList();

            ViewBag.NeedsReview = _context.Tasks
                .Where(t => t.Status == "review" && t.ProjectId == selectedProjectId)
                .Include(t => t.Project)
                .ToList();

            ViewBag.Completed = _context.Tasks
                .Where(t => t.Status == "completed" && t.ProjectId == selectedProjectId)
                .Include(t => t.Project)
                .ToList();

            ViewBag.Projects = new SelectList(ownedProjects, "Id", "Name", selectedProjectId);

            // ✅ فقط أعضاء المشروع
            var projectMembers = _context.ProjectMembers
                .Where(pm => pm.ProjectId == selectedProjectId && pm.User != null)
                .Select(pm => pm.User)
                .ToList();

            ViewBag.Users = new SelectList(projectMembers, "Id", "Username");
            ViewBag.SelectedProjectId = selectedProjectId;

            return View();
        }

		public IActionResult MyTasks(int? projectId = null)
		{
			var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
                return RedirectToAction("Login", "Users");
			// جلب المعرفات الخاصة بالمشاريع المرتبط بها المستخدم
			var projectIds = _context.ProjectMembers
				.Where(pm => pm.UserId == currentUserId)
				.Select(pm => pm.ProjectId)
				.Distinct()
				.ToList();

			// جلب المشاريع التي ينتمي إليها المستخدم
			var userProjects = _context.Projects
				.Where(p => projectIds.Contains(p.Id))
				.ToList();

			if (!userProjects.Any())
			{
				TempData["Error"] = "You are not assigned to any project.";
				ViewBag.Projects = new SelectList(new List<Project>(), "Id", "Name");
				return View();
			}

			// اختيار أول مشروع افتراضيًا إن لم يتم تحديده
			int selectedProjectId = projectId ?? userProjects.First().Id;

			// جلب المهام حسب الحالة والمشروع والمستخدم الحالي
			ViewBag.Todo = _context.Tasks
				.Where(t => t.Status == "todo" && t.AssignedTo == currentUserId && t.ProjectId == selectedProjectId)
				.Include(t => t.Project)
				.Include(t => t.TaskAttachments)
				.ToList();

			ViewBag.InProgress = _context.Tasks
				.Where(t => t.Status == "in-progress" && t.AssignedTo == currentUserId && t.ProjectId == selectedProjectId)
				.Include(t => t.Project)
				.Include(t => t.TaskAttachments)
				.ToList();

			ViewBag.NeedsReview = _context.Tasks
				.Where(t => t.Status == "review" && t.AssignedTo == currentUserId && t.ProjectId == selectedProjectId)
				.Include(t => t.Project)
				.Include(t => t.TaskAttachments)
				.ToList();

			ViewBag.Completed = _context.Tasks
				.Where(t => t.Status == "completed" && t.AssignedTo == currentUserId && t.ProjectId == selectedProjectId)
				.Include(t => t.Project)
				.Include(t => t.TaskAttachments)
				.ToList();

			// تمرير قائمة المشاريع و المشروع المحدد
			ViewBag.Projects = new SelectList(userProjects, "Id", "Name", selectedProjectId);
			ViewBag.SelectedProjectId = selectedProjectId;

			return View();
		}



		[HttpPost]
        public IActionResult Create([Bind("ProjectId,TaskName,Category,StartDate,DueDate,AssignedTo,Priority,Description")] Task task)
        {
            task.Status = "todo";
            task.CreatedAt = DateTime.Now;
            task.UpdatedAt = DateTime.Now;
            task.CompletionPercentage = 0;

            _context.Tasks.Add(task);
            _context.SaveChanges();

            var user = _context.Users.FirstOrDefault(u => u.Id == task.AssignedTo);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                string subject = "📌 Task Assigned to You";
                string body = $@"
                    <h3>Hi {user.Username},</h3>
                    <p>You have been assigned a new task: <strong>{task.TaskName}</strong></p>
                    <p><b>Due Date:</b> {task.DueDate?.ToString("dd MMM yyyy")}</p>
                    <p><b>Project:</b> {_context.Projects.FirstOrDefault(p => p.Id == task.ProjectId)?.Name}</p>
                    <p>Please check your dashboard for full task details.</p>
                    <hr/>
                    <small>This is an automated message from Task Management System.</small>";

                _emailService.SendEmailAsync(user.Email, subject, body);
            }

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

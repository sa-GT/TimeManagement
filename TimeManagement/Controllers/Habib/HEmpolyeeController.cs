using Microsoft.AspNetCore.Mvc;
using TimeManagement.Models;

namespace TimeManagement.Controllers.Habib
{
	public class HEmpolyeeController : Controller
	{
		private readonly MyDbContext _context;

		public HEmpolyeeController(MyDbContext context)
		{
			_context = context;
		}

		public IActionResult ProjectEmployee()
		{
			int userId = 4;

			// أولاً: جلب الميمبرز اللي اليوزر داخل فيها
			var userProjectMemberEntries = _context.ProjectMembers
				.Where(pm => pm.UserId == userId)
				.ToList();

			// جلب الـ ProjectIds اللي الموظف مشارك فيها
			var userProjectIds = userProjectMemberEntries
				.Select(pm => pm.ProjectId)
				.Distinct()
				.ToList();

			// جلب المشاريع اللي لها علاقة باليوزر
			var filteredProjects = _context.Projects
				.Where(p => userProjectIds.Contains(p.Id))
				.ToList();

			// جلب كل الميمبرز اللي شغالين على نفس هاي المشاريع
			var relatedProjectMembers = _context.ProjectMembers
				.Where(pm => userProjectIds.Contains(pm.ProjectId))
				.ToList();

			// جلب كل اليوزرات اللي هم ميمبرز في هاي المشاريع
			var relatedUserIds = relatedProjectMembers
				.Select(pm => pm.UserId)
				.Distinct()
				.ToList();

			var relatedUsers = _context.Users
				.Where(u => relatedUserIds.Contains(u.Id))
				.ToList();

			// جلب كل الملفات التابعة للمشاريع
			var relatedDocs = _context.ProjectDocuments
				.Where(doc => userProjectIds.Contains(doc.ProjectId))
				.ToList();

			// إنشاء الموديل
			var model = Tuple.Create(filteredProjects, relatedProjectMembers, relatedUsers, relatedDocs);

			return View(model);
		}

	}

}

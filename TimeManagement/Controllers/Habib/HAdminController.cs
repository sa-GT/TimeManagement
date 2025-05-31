using Microsoft.AspNetCore.Mvc;
using TimeManagement.Models;

namespace TimeManagement.Controllers.Habib
{
	public class HAdminController : Controller
	{
		private readonly MyDbContext myDbContext;
		public HAdminController(MyDbContext myDbContext)
		{
			this.myDbContext = myDbContext;
		}
		public IActionResult Projectlist()
		{
			var all_projects = myDbContext.Projects.ToList();
			var all_project_memmber = myDbContext.ProjectMembers.ToList();
			var all_users = myDbContext.Users.ToList();
			var model = Tuple.Create(all_projects,all_project_memmber,all_users);
			return View(model);
		}
	}
}

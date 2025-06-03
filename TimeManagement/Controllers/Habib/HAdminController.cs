using Microsoft.AspNetCore.Mvc;
using TimeManagement.Models;
using Microsoft.AspNetCore.Hosting;

namespace TimeManagement.Controllers.Habib
{
	public class HAdminController : Controller
	{
		private readonly MyDbContext myDbContext;
		private readonly IWebHostEnvironment _env; 

		public HAdminController(MyDbContext myDbContext, IWebHostEnvironment env) 
		{
			this.myDbContext = myDbContext;
			this._env = env; 
		}
		public IActionResult Projectlist()
		{
			var all_projects = myDbContext.Projects.ToList();
			var all_project_memmber = myDbContext.ProjectMembers.ToList();
			var all_users = myDbContext.Users.ToList();
			var all_projectsDocs = myDbContext.ProjectDocuments.ToList();
			var model = Tuple.Create(all_projects, all_project_memmber, all_users,all_projectsDocs);
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> AddProject(Project project, List<IFormFile> Documents)
		{
			project.ManagerId = 3;
			project.Status = "active";

			myDbContext.Projects.Add(project);
			await myDbContext.SaveChangesAsync();

			int newProjectId = project.Id;

			var selectedUserIds = Request.Form["SelectedUserIds"];

			foreach (var userIdStr in selectedUserIds)
			{
				if (int.TryParse(userIdStr, out int userId))
				{
					var member = new ProjectMember
					{
						ProjectId = newProjectId,
						UserId = userId,
						Role = "member",
						JoinDate = DateOnly.FromDateTime(DateTime.Today),
						IsActive = true,
						CreatedAt = DateTime.Now,
						UpdatedAt = DateTime.Now
					};

					myDbContext.ProjectMembers.Add(member);
				}
			}

			// إضافة الملفات
			var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "project_docs");
			Directory.CreateDirectory(uploadsFolder);

			foreach (var file in Documents)
			{
				if (file.Length > 0)
				{
					var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await file.CopyToAsync(stream);
					}

					var doc = new ProjectDocument
					{
						ProjectId = newProjectId,
						FileName = file.FileName,
						FilePath = "/uploads/project_docs/" + uniqueFileName
					};

					myDbContext.ProjectDocuments.Add(doc);
				}
			}

			await myDbContext.SaveChangesAsync();

			return RedirectToAction("Projectlist");
		}
		public IActionResult DeleteProject(int id)
		{
			var project = myDbContext.Projects.Find(id);
			if (project != null)
			{
				myDbContext.Projects.Remove(project);
				var projectMembers = myDbContext.ProjectMembers.Where(pm => pm.ProjectId == id).ToList();
				foreach (var member in projectMembers)
				{
					myDbContext.ProjectMembers.Remove(member);
				}
				var projectDocuments = myDbContext.ProjectDocuments.Where(pd => pd.ProjectId == id).ToList();
				foreach (var doc in projectDocuments)
				{
					myDbContext.ProjectDocuments.Remove(doc);
					var filePath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
					if (System.IO.File.Exists(filePath))
					{
						System.IO.File.Delete(filePath);
					}
				}
				myDbContext.SaveChanges();
			}
			return RedirectToAction("Projectlist");
		}
	}

}

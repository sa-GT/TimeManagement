using Microsoft.AspNetCore.Mvc;

namespace TimeManagement.Controllers.Habib
{
	public class UserController : Controller
	{
		public IActionResult LandingPage()
		{
			return View();
		}
	}
}

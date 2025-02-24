using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelManagement2.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult BlankPage()
		{
			return View();
		}
		public IActionResult AboutPage()
		{
			return View();
		}		
		public IActionResult SignIn()
		{
			return View();
		}
	}
}

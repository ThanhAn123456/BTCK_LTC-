using BTCK_LTC_.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BTCK_LTC_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly QuanLyBaiDangCongTyContext _context;

		public HomeController(ILogger<HomeController> logger, QuanLyBaiDangCongTyContext context)
        {
            _logger = logger;
			_context = context;
		}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		// GET: Logins
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login(Employee employee)
		{

			var user = _context.Employees.Where(e => e.Username == employee.Username && e.Password == employee.Password).FirstOrDefault();
			if (user != null)
			{
				HttpContext.Session.SetString("Username", user.Username);
				HttpContext.Session.SetString("Role", user.RoleId.ToString());
				return RedirectToAction(nameof(Index));
			}

			ModelState.AddModelError("", "Invalid username or password");

			return View();
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}
	}
}

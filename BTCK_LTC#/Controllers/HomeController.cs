using BTCK_LTC_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BTCK_LTC_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly QuanLyBaiDangCongTyContext _context;
		private readonly IConfiguration _configuration;

		public HomeController(ILogger<HomeController> logger, QuanLyBaiDangCongTyContext context, IConfiguration configuration)
        {
            _logger = logger;
			_context = context;
			_configuration = configuration;
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

		// GET: Login
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login(Employee employee)
		{

			var user = _context.Employees.FirstOrDefault(e => e.Username == employee.Username && e.Password == employee.Password);
			if (user != null)
			{
				var token = GenerateJwtToken(user);
				HttpContext.Session.SetString("JWToken", token);
				HttpContext.Session.SetString("Name", user.Name);
				HttpContext.Session.SetString("Avatar", user.Avatar);
				return RedirectToAction(nameof(Index));
			}

			ModelState.AddModelError("", "Invalid username or password");

			return View();
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}

		// GET: Account
		public IActionResult Account()
		{
			return View();
		}

		private string GenerateJwtToken(Employee employee)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
			var user = _context.Roles.FirstOrDefault(x => x.Id == employee.RoleId);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, employee.Username)
			};

			// Add multiple roles
			if (user != null)
			{
				Console.WriteLine(user.Post.GetValueOrDefault());
				Console.WriteLine(user.Manage.GetValueOrDefault());
				if (user.Post.GetValueOrDefault())
				{
					claims.Add(new Claim(ClaimTypes.Role, "Post"));
				}
				if (user.Manage.GetValueOrDefault())
				{
					claims.Add(new Claim(ClaimTypes.Role, "Manage"));
				}
			}

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}

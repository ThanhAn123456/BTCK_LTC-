using BTCK_LTC_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

		public HomeController(ILogger<HomeController> logger, QuanLyBaiDangCongTyContext context)
        {
            _logger = logger;
			_context = context;
		}

		public async Task<IActionResult> Index()
        {
			var BaiDangCongTyContext = _context.Posts.Include(p => p.Category).Include(p => p.Employee).ThenInclude(e => e.Company);

			return View(await BaiDangCongTyContext.ToListAsync());
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
	}
}

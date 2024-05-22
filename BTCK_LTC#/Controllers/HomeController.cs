using BTCK_LTC_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using X.PagedList;

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

		public async Task<IActionResult> Index(string searchdocs, string CategoryId, string CompanyId, int? pageNumber)
        {

			IQueryable<Post> BaiDangCongTyContext = _context.Posts.Include(p => p.Category).Include(p => p.Employee).ThenInclude(e => e.Company);

			if (!string.IsNullOrEmpty(searchdocs))
            {
                BaiDangCongTyContext = BaiDangCongTyContext.Where(p => p.Title.Contains(searchdocs));
			}

			if (!string.IsNullOrEmpty(CategoryId))
			{
				BaiDangCongTyContext = BaiDangCongTyContext.Where(p => p.CategoryId == Convert.ToInt32(CategoryId));
			}

			if (!string.IsNullOrEmpty(CompanyId))
			{
				BaiDangCongTyContext = BaiDangCongTyContext.Where(p => p.Employee.CompanyId == Convert.ToInt32(CompanyId));
			}

			BaiDangCongTyContext = BaiDangCongTyContext.OrderByDescending(p => p.PostDate);

            int pageSize = 4;
            int currentPage = pageNumber ?? 1;

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
			ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
			return View(await BaiDangCongTyContext.ToPagedListAsync(currentPage, pageSize));
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var CategoriesId = _context.Posts.FirstOrDefault(p => p.Id == id).CategoryId;

            ViewData["CategoryId"] = CategoriesId;

            return View(post);
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

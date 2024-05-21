using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTCK_LTC_.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BTCK_LTC_.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly QuanLyBaiDangCongTyContext _context;

        public CategoriesController(QuanLyBaiDangCongTyContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string searchdocs)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Manage"))
			{
				return Forbid();
			}

            IQueryable<Category> CategoriesContext = _context.Categories;

            if (!string.IsNullOrEmpty(searchdocs))
            {
                CategoriesContext = CategoriesContext.Where(c => c.Name.Contains(searchdocs));
            }

            return View(await CategoriesContext.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Manage"))
			{
				return Forbid();
			}

			if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Manage"))
			{
				return Forbid();
			}

			return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Manage"))
			{
				return Forbid();
			}

			if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Manage"))
			{
				return Forbid();
			}

			if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

		private IEnumerable<Claim> GetClaims()
		{
			var token = HttpContext.Session.GetString("JWToken");
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}

			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
			var claims = jsonToken.Claims;

			return claims;
		}
	}
}

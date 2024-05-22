﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTCK_LTC_.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using X.PagedList;
using System.ComponentModel.Design;

namespace BTCK_LTC_.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly QuanLyBaiDangCongTyContext _context;
		private readonly IConfiguration _configuration;

		public DepartmentsController(QuanLyBaiDangCongTyContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
		}

        // GET: Departments
        public async Task<IActionResult> Index(string searchdocs, int? pageNumber)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Home");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Manage"))
			{
				return Forbid();
			}

            IQueryable<Department> DepartmentsContext = _context.Departments;

            if (!string.IsNullOrEmpty(searchdocs))
            {
                DepartmentsContext = DepartmentsContext.Where(d => d.Name.Contains(searchdocs));
            }

			DepartmentsContext = DepartmentsContext.OrderBy(d => d.Id);

			int pageSize = Convert.ToInt32(_configuration["PageList:PageSize"]);
			int currentPage = pageNumber ?? 1;

			ViewData["CurrentSearchDocs"] = searchdocs;
			return View(await DepartmentsContext.ToPagedListAsync(currentPage, pageSize));
        }

        // GET: Departments/Details/5
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

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
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

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
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

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            return View(department);
        }

        // GET: Departments/Delete/5
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

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
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

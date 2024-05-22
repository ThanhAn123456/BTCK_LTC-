using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTCK_LTC_.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using X.PagedList;

namespace BTCK_LTC_.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly QuanLyBaiDangCongTyContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IConfiguration _configuration;

		public EmployeesController(QuanLyBaiDangCongTyContext context, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
			_configuration = configuration;
		}

		// GET: Employees
		public async Task<IActionResult> Index(string searchdocs, string Gender, string CompanyId, string DerpartmentId, string RoleId, int? pageNumber)
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

            IQueryable<Employee> EmployeesContext = _context.Employees.Include(e => e.Company).Include(e => e.Derpartment).Include(e => e.Role);

            if (!string.IsNullOrEmpty(searchdocs))
            {
                EmployeesContext = EmployeesContext.Where(e => e.Name.Contains(searchdocs) || e.Email.Contains(searchdocs) || e.PhoneNumber.Contains(searchdocs));
            }

            if (!string.IsNullOrEmpty(Gender))
            {
                EmployeesContext = EmployeesContext.Where(e => e.Gender == Gender);
            }

            if (!string.IsNullOrEmpty(CompanyId))
            {
                EmployeesContext = EmployeesContext.Where(e => e.CompanyId == Convert.ToInt32(CompanyId));
            }

            if (!string.IsNullOrEmpty(DerpartmentId))
            {
                EmployeesContext = EmployeesContext.Where(e => e.DerpartmentId == Convert.ToInt32(DerpartmentId));
            }
            if (!string.IsNullOrEmpty(RoleId))
            {
                EmployeesContext = EmployeesContext.Where(e => e.RoleId == Convert.ToInt32(RoleId));
            }

			EmployeesContext = EmployeesContext.OrderBy(e => e.Id);

			int pageSize = Convert.ToInt32(_configuration["PageList:PageSize"]);
			int currentPage = pageNumber ?? 1;

			ViewData["CompanyIdList"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["DerpartmentIdList"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["RoleIdList"] = new SelectList(_context.Roles, "Id", "Name");
			ViewData["CurrentSearchDocs"] = searchdocs;
            ViewData["CurrentGender"] = Gender;
            ViewData["CurrentCompanyId"] = CompanyId;
			ViewData["CurrentDerpartmentId"] = DerpartmentId;
			ViewData["CurrentRoleId"] = RoleId;
			return View(await EmployeesContext.ToPagedListAsync(currentPage, pageSize));
        }

        // GET: Employees/Details/5
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

            var employee = await _context.Employees
                .Include(e => e.Company)
                .Include(e => e.Derpartment)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
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

			ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["DerpartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Birthday,Address,Email,PhoneNumber,Avatar,Username,Password,DerpartmentId,CompanyId,RoleId")] Employee employee, IFormFile? AvatarFile)
        {
            if (ModelState.IsValid)
            {
                if (AvatarFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "avatars");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + AvatarFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await AvatarFile.CopyToAsync(fileStream);
                    }
                    employee.Avatar = uniqueFileName;
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
            ViewData["DerpartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DerpartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", employee.RoleId);
            return View(employee);
        }

        // GET: Employees/Edit/5
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

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
            ViewData["DerpartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DerpartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", employee.RoleId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Birthday,Address,Email,PhoneNumber,Avatar,Username,Password,DerpartmentId,CompanyId,RoleId")] Employee employee, IFormFile? AvatarFile)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (AvatarFile != null)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "avatars");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + AvatarFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await AvatarFile.CopyToAsync(fileStream);
                        }

                        // Delete the old avatar file if it exists
                        if (!string.IsNullOrEmpty(employee.Avatar))
                        {
                            string oldFilePath = Path.Combine(uploadsFolder, employee.Avatar);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        employee.Avatar = uniqueFileName;
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
            ViewData["DerpartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DerpartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", employee.RoleId);
            return View(employee);
        }

        // GET: Employees/Delete/5
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

            var employee = await _context.Employees
                .Include(e => e.Company)
                .Include(e => e.Derpartment)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                // Xóa ảnh trong thư mục avatars
                if (!string.IsNullOrEmpty(employee.Avatar))
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "avatars");
                    string filePath = Path.Combine(uploadsFolder, employee.Avatar);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
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

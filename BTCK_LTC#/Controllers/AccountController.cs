using BTCK_LTC_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BTCK_LTC_.Controllers
{
	public class AccountController : Controller
	{
		private readonly QuanLyBaiDangCongTyContext _context;
		private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(QuanLyBaiDangCongTyContext context, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

		public async Task<IActionResult> Index(int id)
		{
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

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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
                return RedirectToAction("Index", new { id = employee.Id });
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
            ViewData["DerpartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DerpartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", employee.RoleId);
            return View(employee);
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
				HttpContext.Session.SetString("ID", user.Id.ToString());
				HttpContext.Session.SetString("Name", user.Name);
				HttpContext.Session.SetString("Avatar", user.Avatar);
				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError("", "Invalid username or password");

			return View();
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index","Home");
		}

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
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

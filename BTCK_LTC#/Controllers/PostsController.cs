﻿using System;
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
using System.ComponentModel.Design;

namespace BTCK_LTC_.Controllers
{
    public class PostsController : Controller
    {
        private readonly QuanLyBaiDangCongTyContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IConfiguration _configuration;

		public PostsController(QuanLyBaiDangCongTyContext context, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
			_configuration = configuration;
		}

		// GET: Posts
		public async Task<IActionResult> Index(string searchdocs, string PostDate, string CategoryId, int? pageNumber)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Post"))
			{
				return Forbid();
			}

            var EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("ID"));

            IQueryable<Post> quanLyBaiDangCongTyContext = _context.Posts.Where(p => p.EmployeeId == EmployeeId).Include(p => p.Category).Include(p => p.Employee);

            if (!string.IsNullOrEmpty(searchdocs))
            {
                quanLyBaiDangCongTyContext = quanLyBaiDangCongTyContext.Where(p => p.Title.Contains(searchdocs));
            }

            if (!string.IsNullOrEmpty(CategoryId))
            {
                quanLyBaiDangCongTyContext = quanLyBaiDangCongTyContext.Where(p => p.CategoryId == Convert.ToInt32(CategoryId));
            }

            if (PostDate == "1" || PostDate == null)
            {
                quanLyBaiDangCongTyContext = quanLyBaiDangCongTyContext.OrderByDescending(p => p.PostDate);
            }
            else
            {
                quanLyBaiDangCongTyContext = quanLyBaiDangCongTyContext.OrderBy(p => p.PostDate);
            }

			int pageSize = Convert.ToInt32(_configuration["PageList:PageSize"]);
			int currentPage = pageNumber ?? 1;

			ViewData["CategoryIdList"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["PostDate"] = PostDate;
			ViewData["CurrentSearchDocs"] = searchdocs;
			ViewData["CurrentCategoryId"] = CategoryId;
			return View(await quanLyBaiDangCongTyContext.ToPagedListAsync(currentPage, pageSize));
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Post"))
			{
				return Forbid();
			}

			if (id == null)
            {
                return NotFound();
            }

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

        // GET: Posts/Create
        public IActionResult CreateThongBao()
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Post"))
			{
				return Forbid();
			}

            var EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            var CategoriesId = _context.Categories.FirstOrDefault(c => c.Name == "Thông báo").Id;

            ViewData["CategoryId"] = CategoriesId;
            ViewData["EmployeeId"] = EmployeeId;
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThongBao([Bind("Id,Title,Content,Thumbnail,Date,PostDate,EmployeeId,CategoryId")] Post post, IFormFile? ThumbnailFile)
        {
            if (ModelState.IsValid)
            {
                if (ThumbnailFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "thumbnails");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ThumbnailFile.CopyToAsync(fileStream);
                    }
                    post.Thumbnail = uniqueFileName;
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            var CategoriesId = _context.Categories.FirstOrDefault(c => c.Name == "Thông báo").Id;

            ViewData["CategoryId"] = CategoriesId;
            ViewData["EmployeeId"] = EmployeeId;
            return View(post);
        }

        // GET: Posts/Create
        public IActionResult CreateLichTrinh()
        {
            //check role
            var claims = GetClaims();
            if (claims == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (!claims.Any(c => c.Type == "role" && c.Value == "Post"))
            {
                return Forbid();
            }

            var EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            var CategoriesId = _context.Categories.FirstOrDefault(c => c.Name == "Lịch trình").Id;

            ViewData["CategoryId"] = CategoriesId;
            ViewData["EmployeeId"] = EmployeeId;
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLichTrinh([Bind("Id,Title,Content,Thumbnail,Date,PostDate,EmployeeId,CategoryId")] Post post, IFormFile? ThumbnailFile)
        {
            if (ModelState.IsValid)
            {
                if (ThumbnailFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "thumbnails");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ThumbnailFile.CopyToAsync(fileStream);
                    }
                    post.Thumbnail = uniqueFileName;
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            var CategoriesId = _context.Categories.FirstOrDefault(c => c.Name == "Lịch trình").Id;

            ViewData["CategoryId"] = CategoriesId;
            ViewData["EmployeeId"] = EmployeeId;
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Post"))
			{
				return Forbid();
			}

			if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            var CategoriesId = _context.Posts.FirstOrDefault(p => p.Id == id).CategoryId;

            ViewData["CategoryId"] = CategoriesId;
            ViewData["EmployeeId"] = EmployeeId;
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Thumbnail,Date,PostDate,EmployeeId,CategoryId")] Post post, IFormFile? ThumbnailFile)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ThumbnailFile != null)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "thumbnails");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ThumbnailFile.CopyToAsync(fileStream);
                        }

                        // Delete the old avatar file if it exists
                        if (!string.IsNullOrEmpty(post.Thumbnail))
                        {
                            string oldFilePath = Path.Combine(uploadsFolder, post.Thumbnail);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        post.Thumbnail = uniqueFileName;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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

            var EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            var CategoriesId = _context.Posts.FirstOrDefault(p => p.Id == id).CategoryId;

            ViewData["CategoryId"] = CategoriesId;
            ViewData["EmployeeId"] = EmployeeId;
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
			//check role
			var claims = GetClaims();
			if (claims == null)
			{
				return RedirectToAction("Login", "Account");
			}
			if (!claims.Any(c => c.Type == "role" && c.Value == "Post"))
			{
				return Forbid();
			}

			if (id == null)
            {
                return NotFound();
            }

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

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                // Xóa ảnh trong thư mục avatars
                if (!string.IsNullOrEmpty(post.Thumbnail))
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "thumbnails");
                    string filePath = Path.Combine(uploadsFolder, post.Thumbnail);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
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

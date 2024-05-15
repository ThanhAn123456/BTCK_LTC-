using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTCK_LTC_.Models;
using Microsoft.Extensions.Hosting;

namespace BTCK_LTC_.Controllers
{
    public class PostsController : Controller
    {
        private readonly QuanLyBaiDangCongTyContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostsController(QuanLyBaiDangCongTyContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var quanLyBaiDangCongTyContext = _context.Posts.Include(p => p.Category).Include(p => p.Employee);
            return View(await quanLyBaiDangCongTyContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,Thumbnail,Date,PostDate,EmployeeId,CategoryId")] Post post, IFormFile ThumbnailFile)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", post.EmployeeId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", post.EmployeeId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Thumbnail,Date,PostDate,EmployeeId,CategoryId")] Post post, IFormFile ThumbnailFile)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", post.EmployeeId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
    }
}

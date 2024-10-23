using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTJob.Data;
using FPTJob.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace FPTJob.Controllers
{
    public class CVsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public CVsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CVs
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User); // Lấy user hiện tại
            if (currentUser == null)
            {
                return NotFound();
            }
            var cv = await _context.CV
                                .Where(j => j.ApplicantId == currentUser.Id) // Chỉ lấy các công việc do user này đăng
                                .ToListAsync();
            return View(cv);
        }

        // GET: CVs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cV = await _context.CV
                .FirstOrDefaultAsync(m => m.CVID == id);
            if (cV == null)
            {
                return NotFound();
            }

            return View(cV);
        }

        // GET: CVs/Create
        [Authorize(Roles = "Jobseeker")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CVs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CV cv, IFormFile file)
        {
            var currentUser = await _userManager.GetUserAsync(User); // Lấy user hiện tại đang đăng nhập
            cv.ApplicantId = currentUser.Id; // Gán ApplicantId là ID của user hiện tại
            if (ModelState.IsValid)
            {
                // Kiểm tra xem có file được upload hay không
                if (file != null && file.Length > 0)
                {
                    // Đặt tên file duy nhất bằng cách thêm GUID
                    var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    // Đặt đường dẫn file lưu trữ
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // Lưu file vào project folder
                        await file.CopyToAsync(stream);
                    }
                    // Lưu đường dẫn file vào thuộc tính cv.FilePath
                    cv.file = "/files/" + fileName;
                }
                // Thêm CV vào cơ sở dữ liệu
                _context.CV.Add(cv);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(cv);
        }

        // GET: CVs/Edit/5
        [Authorize(Roles = "Jobseeker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cv = await _context.CV.FindAsync(id);
            if (cv == null)
            {
                return NotFound();
            }
            return View(cv);
        }

        // POST: CVs/Edit/5
        // POST: CVs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CV cv, IFormFile newFile)
        {
            if (id != cv.CVID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingCV = await _context.CV.FindAsync(id);

                if (existingCV == null)
                {
                    return NotFound();
                }

                // Update non-file related properties
                existingCV.Description = cv.Description;

                // Handle new file upload
                if (newFile != null && newFile.Length > 0)
                {
                    // Ensure the file is a PDF
                    if (Path.GetExtension(newFile.FileName).ToLower() == ".pdf")
                    {
                        // Generate a new file name
                        var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(newFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);

                        // Save the new file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await newFile.CopyToAsync(stream);
                        }

                        // Remove the old file if it exists
                        if (!string.IsNullOrEmpty(existingCV.file))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCV.file.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Update the file path in the database
                        existingCV.file = "/files/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Only PDF files are allowed.");
                        return View(cv);
                    }
                }

                try
                {
                    _context.Update(existingCV);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CVExists(cv.CVID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(cv);
        }


        // GET: CVs/Delete/5
        [Authorize(Roles = "Jobseeker")]

        public async Task<IActionResult> Delete(int? id)
        {
            var cV = await _context.CV
                .FirstOrDefaultAsync(m => m.CVID == id);
            var currentUser = await _userManager.GetUserAsync(User); // Lấy user hiện tại
            if (cV.ApplicantId != currentUser.Id)
            {
                return Unauthorized(); // Nếu công việc này không thuộc về user hiện tại, trả về lỗi 403
            }
            if (id == null)
            {
                return NotFound();
            }
            if (cV == null)
            {
                return NotFound();
            }

            return View(cV);
        }

        // POST: CVs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cV = await _context.CV.FindAsync(id);
            _context.CV.Remove(cV);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CVExists(int id)
        {
            return _context.CV.Any(e => e.CVID == id);
        }
    }
}

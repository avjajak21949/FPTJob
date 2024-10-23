﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTJob.Data;
using FPTJob.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FPTJob.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public JobsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Jobs
        public async Task<IActionResult> Index()
        {
            // Get the current logged-in user
            var currentUserId = _userManager.GetUserId(User);

            // Retrieve all jobs
            var jobs = await _context.Job.Include(j => j.Category).ToListAsync();

            // Retrieve all CVs associated with the logged-in user (ApplicantId)
            var cvs = await _context.CV.Where(cv => cv.ApplicantId == currentUserId).ToListAsync();
            ViewBag.CVs = cvs;
            return View(jobs);
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Salary,Place,Time,CategoryID")] Job job)
        {
            var currentUser = await _userManager.GetUserAsync(User); // Lấy user hiện tại đang đăng nhập
            job.EmployerId = currentUser.Id; // Gán EmployerId là ID của user hiện tại
            if (ModelState.IsValid)
            {
                job.IsApproved = false;
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "Id", "Name", job.CategoryID);
            return View(job);
        }

        // GET: Jobs/Edit/5
        [Authorize(Roles = "Employer")]

        public async Task<IActionResult> Edit(int? id)
        {
            var job = await _context.Job.FindAsync(id);
            var currentUser = await _userManager.GetUserAsync(User); // Lấy user hiện tại
            if (job.EmployerId != currentUser.Id)
            {
                return Unauthorized(); // Nếu công việc này không thuộc về user hiện tại, trả về lỗi 403
            }
            else if (job == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "Id", "Name", job.CategoryID);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Salary,Place,Time,CategoryID")] Job job)
        {
            var currentUser = await _userManager.GetUserAsync(User); // Lấy user hiện tại
            if (job.EmployerId != currentUser.Id)
            {
                return Unauthorized(); // Nếu công việc này không thuộc về user hiện tại, trả về lỗi 403
            }
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            ViewData["CategoryID"] = new SelectList(_context.Category, "Id", "Name", job.CategoryID);
            return View(job);
        }

        // GET: Jobs/Delete/5
        [Authorize(Roles = "Employer")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            var currentUser = await _userManager.GetUserAsync(User);

            if (job.EmployerId != currentUser.Id)
            {
                return Unauthorized(); // Kiểm tra quyền sở hữu, nếu không đúng thì từ chối truy cập
            }
            else if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Job.FindAsync(id);
            _context.Job.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> MyJobs()
        {
            var currentUser = await _userManager.GetUserAsync(User); // Lấy user hiện tại
            var jobs = await _context.Job
                                .Where(j => j.EmployerId == currentUser.Id) // Chỉ lấy các công việc do user này đăng
                                .ToListAsync();
            return View(jobs);
        }
        private bool JobExists(int id)
        {
            return _context.Job.Any(e => e.Id == id);
        }

        // Approve a job
        [HttpPost]
        public async Task<IActionResult> ApproveJob(int jobId)
        {
            var job = await _context.Job.FindAsync(jobId);
            if (job != null)
            {
                job.IsApproved = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Jobseeker")]
        // Reject a job (delete it)
        [HttpPost]
        public async Task<IActionResult> RejectJob(int jobId)
        {
            var job = await _context.Job.FindAsync(jobId);
            if (job != null)
            {
                _context.Job.Remove(job);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ApplyForJob(int jobId, int cvId)
        {
            // Lấy CV dựa trên cvId
            var cv = await _context.CV.FindAsync(cvId);

            if (cv == null)
            {
                return NotFound("CV không tồn tại");
            }

            // Lấy đường dẫn tới file CV
            string cvFilePath = cv.file;

            // Xử lý logic apply vào công việc ở đây (ví dụ lưu JobCV vào DB)
            var jobApplication = new JobCV
            {
                JobId = jobId,
                CVID = cvId,
                IsApproved = false // Hoặc logic khác cho việc approval
            };

            _context.JobCV.Add(jobApplication);
            await _context.SaveChangesAsync();

            // Optionally, trả về file CV hoặc thông báo apply thành công
            return Ok("Đã apply công việc thành công!");
        }

    }
}
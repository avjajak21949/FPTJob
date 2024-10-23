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
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace FPTJob.Controllers
{
    public class JobCVsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public JobCVsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobCVs
        public async Task<IActionResult> Index()
        {
            var jobCvs = _context.JobCV
                     .Include(j => j.Job)  // Bao gồm Job
                     .Include(c => c.CV)   // Bao gồm CV
                     .ToList();

            return View(await _context.JobCV.ToListAsync());
        }

        // GET: JobCVs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobCV = await _context.JobCV
                .FirstOrDefaultAsync(m => m.JobCVID == id);
            if (jobCV == null)
            {
                return NotFound();
            }

            return View(jobCV);
        }

        // GET: JobCVs/Create
        [Authorize(Roles = "Jobseeker")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobCVs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobCVID")] JobCV jobCV)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobCV);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobCV);
        }

        // GET: JobCVs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobCV = await _context.JobCV.FindAsync(id);
            if (jobCV == null)
            {
                return NotFound();
            }
            return View(jobCV);
        }

        // POST: JobCVs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Jobseeker")]
        public async Task<IActionResult> Edit(int id, [Bind("JobCVID")] JobCV jobCV)
        {
            if (id != jobCV.JobCVID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobCV);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobCVExists(jobCV.JobCVID))
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
            return View(jobCV);
        }

        // GET: JobCVs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobCV = await _context.JobCV
                .FirstOrDefaultAsync(m => m.JobCVID == id);
            if (jobCV == null)
            {
                return NotFound();
            }

            return View(jobCV);
        }

        // POST: JobCVs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer, Jobseeker")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobCV = await _context.JobCV.FindAsync(id);
            _context.JobCV.Remove(jobCV);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobCVExists(int id)
        {
            return _context.JobCV.Any(e => e.JobCVID == id);
        }

        // Hiển thị danh sách CV nộp vào công việc
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> ViewJobCVs(int jobId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var job = await _context.Job
                .Include(j => j.JobCV) // Lấy danh sách CV nộp vào công việc
                .ThenInclude(jc => jc.CV) // Lấy thông tin CV
                .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == currentUser.Id);

            if (job == null)
            {
                return Unauthorized(); // Chỉ employer sở hữu job mới có thể xem CV
            }

            return View(job.JobCV.Select(jc => jc.CV).ToList()); // Trả về view danh sách CV nộp
        }

        [Authorize(Roles = "Employer")]
        [HttpPost]
        public async Task<IActionResult> ApproveCV(int jobCVID)
        {
            var jobCV = await _context.JobCV.FindAsync(jobCVID);

            if (jobCV == null)
            {
                return NotFound();
            }

            jobCV.IsApproved = true; // Chấp nhận CV

            _context.Update(jobCV);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Action để từ chối CV
        [Authorize(Roles = "Employer")]
        [HttpPost]
        public async Task<IActionResult> RejectCV(int jobCVID)
        {
            var jobCV = await _context.JobCV.FindAsync(jobCVID);

            if (jobCV == null)
            {
                return NotFound();
            }
                _context.JobCV.Remove(jobCV);
                await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

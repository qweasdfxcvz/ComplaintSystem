using ComplaintSystem.Data;
using ComplaintSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComplaintSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدير النظام")]
    public class ComplaintStatusesController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public ComplaintStatusesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ComplaintStatuses
        public async Task<IActionResult> Index()
        {
            return View(await _context.ComplaintStatuses.ToListAsync());
        }

        // GET: ComplaintStatuses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ComplaintStatuses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintStatus status)
        {
            if (ModelState.IsValid)
            {
                _context.Add(status);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(status);
        }

        // GET: ComplaintStatuses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var status = await _context.ComplaintStatuses.FindAsync(id);
            if (status == null) return NotFound();

            return View(status);
        }

        // POST: ComplaintStatuses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComplaintStatus status)
        {
            if (id != status.StatusId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(status);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ComplaintStatuses.Any(e => e.StatusId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(status);
        }

        // POST: Admin/ComplaintStatuses/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _context.ComplaintStatuses.FindAsync(id);
            if (status == null)
                return NotFound();

            _context.ComplaintStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}

using ComplaintSystem.Data;
using ComplaintSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComplaintSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدير النظام")]
    public class ComplaintTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ComplaintTypesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/ComplaintTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ComplaintTypes.ToListAsync());
        }

        // GET: Admin/ComplaintTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ComplaintTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintType complaintType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(complaintType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(complaintType);
        }

        // GET: Admin/ComplaintTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var type = await _context.ComplaintTypes.FindAsync(id);
            if (type == null) return NotFound();

            return View(type);
        }

        // POST: Admin/ComplaintTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComplaintType complaintType)
        {
            if (id != complaintType.ComplaintTypeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(complaintType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ComplaintTypes.Any(e => e.ComplaintTypeId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(complaintType);
        }

        // POST: Admin/ComplaintTypes/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var type = await _context.ComplaintTypes.FindAsync(id);
            if (type == null) return NotFound();

            _context.ComplaintTypes.Remove(type);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}

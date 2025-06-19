using ComplaintSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComplaintSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدير النظام")]
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComplaintsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string search, int? statusId, int? days)
        {
            var complaintsQuery = _context.Complaints
                .Include(c => c.Status)
                .Include(c => c.ComplaintType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                complaintsQuery = complaintsQuery.Where(c =>
                    c.CitizenName.Contains(search) ||
                    c.NationalId.Contains(search) ||
                    c.ComplaintNumber.Contains(search) ||
                    c.Description.Contains(search));
            }

            if (statusId.HasValue)
            {
                complaintsQuery = complaintsQuery.Where(c => c.StatusId == statusId.Value);
            }

            if (days.HasValue)
            {
                var cutoffDate = DateTime.Now.AddDays(-days.Value);
                complaintsQuery = complaintsQuery.Where(c => c.CreatedAt >= cutoffDate);
            }

            var complaints = await complaintsQuery
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // إرسال جميع الحالات لعرضها في القائمة المنسدلة
            ViewBag.Statuses = await _context.ComplaintStatuses.ToListAsync();

            return View(complaints);
        }

        public async Task<IActionResult> Details(int id)
        {
            var complaint = await _context.Complaints
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.Assignments)
                    .ThenInclude(a => a.FromUser)
                .Include(c => c.Assignments)
                    .ThenInclude(a => a.ToUser)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

    }
}

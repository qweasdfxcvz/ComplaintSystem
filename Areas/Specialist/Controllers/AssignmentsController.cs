using ComplaintSystem.Data;
using ComplaintSystem.Models;
using ComplaintSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ComplaintSystem.Areas.Specialist.Controllers
{
    [Area("Specialist")]
    [Authorize(Roles = "مختص")]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // الحصول على معرف المستخدم الحالي من الـ Claims
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // جلب الشكاوى المحولة للمستخدم الحالي فقط، وحالتها "قيد المعالجة"
            var assignments = await _context.Assignments
                .Include(a => a.Complaint)
                    .ThenInclude(c => c.Status)
                .Include(a => a.Complaint)
                    .ThenInclude(c => c.ComplaintType)
                .Include(a => a.FromUser)
                .Where(a => a.ToUserId == currentUserId && a.Complaint.StatusId == 2) // 2: قيد المعالجة
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();

            return View(assignments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var complaint = await _context.Complaints
                .Include(c => c.ComplaintType)                
                .Include(c => c.Status)
                .Include(c => c.Assignments)                     // تحميل التحويلات
                     .ThenInclude(a => a.FromUser)                // الموظف المحول منه
                .Include(c => c.Assignments)
                     .ThenInclude(a => a.ToUser)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
            {
                TempData["Error"] = "لم يتم العثور على الشكوى المطلوبة.";
                return RedirectToAction("Index");
            }

            // استرجاع الموظفين المختصين فقط
            var specialists = await _context.Users
                .Where(u => u.RoleId == 3) // بافتراض RoleId = 3 هو الأخصائي
                .ToListAsync();

            ViewBag.Specialists = specialists;

            return View(complaint);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> End(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            complaint.StatusId = 3; // منتهية
            _context.Update(complaint);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Reassign(int id)
        {
            var complaint = await _context.Complaints
        .Include(c => c.Assignments)
        .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
                return NotFound();

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var specialists = await _context.Users
                .Where(u => u.Role.Name!= "مدير النظام" && u.RoleId != currentUserId)
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.Name
                })
                .ToListAsync();

            // جلب الحالات المتاحة فقط (قيد المعالجة = 2، منتهية = 3)
            var statuses = await _context.ComplaintStatuses
                .Where(s => s.StatusId == 2 || s.StatusId == 3)
                .Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            var viewModel = new ReassignComplaintViewModel
            {
                ComplaintId = complaint.ComplaintId,
                ComplaintNumber = complaint.ComplaintNumber,
                Specialists = specialists,
                AvailableStatuses = statuses,
                SelectedStatusId = complaint.StatusId
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reassign(int complaintId, int toUserId, string notes, int? selectedStatusId)
        {
            var complaint = await _context.Complaints.FindAsync(complaintId);
            if (complaint == null)
                return NotFound();

            var fromUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var assignment = new Assignment
            {
                ComplaintId = complaintId,
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Notes = notes,
                AssignedAt = DateTime.Now
            };

            _context.Assignments.Add(assignment);
            // تحديث الحالة     
            if (selectedStatusId.HasValue)
            {
                complaint.StatusId = selectedStatusId.Value;
                _context.Complaints.Update(complaint);
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم تحويل الشكوى بنجاح.";
            return RedirectToAction("Details", new { id = complaintId });
        }

    }
}

using ComplaintSystem.Data;
using ComplaintSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComplaintSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدير النظام")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalComplaints = await _context.Complaints.CountAsync(),
                ProcessingComplaints = await _context.Complaints.Where(c => c.StatusId != 3 && c.StatusId != 5).CountAsync(),
                AssignedComplaints = await _context.Assignments.CountAsync(),
                TotalComplaintsClosed = await _context.Complaints.Where(C => C.StatusId == 5).CountAsync(),
                DelayedComplaints = await _context.Complaints
                        .Where(c => c.StatusId != 5 && EF.Functions.DateDiffHour(c.CreatedAt, DateTime.Now) > 48)
                        .CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                Specialists = await _context.Users.CountAsync(u => u.Role.Name == "مختص"),
            };
            return View(model);
        }
    }
}

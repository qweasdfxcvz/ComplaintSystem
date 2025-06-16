using ComplaintSystem.Data;
using ComplaintSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace ComplaintSystem.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComplaintsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Complaints/Create
        public IActionResult Create()
        {
            ViewBag.ComplaintTypeId = new SelectList(_context.ComplaintTypes, "ComplaintTypeId", "Name");
            ViewBag.Statuses = _context.ComplaintStatuses.ToList();
            return View();
        }

        private string GenerateComplaintNumber()
        {
            // مثلاً: CMP-20250612-XYZ1
            return $"CMP-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        }


        // POST: Complaints/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Complaint complaint)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("ModelState Error: " + error.ErrorMessage);
            }
            // ✅ توليد رقم الشكوى قبل التحقق من ModelState
            complaint.ComplaintNumber = GenerateComplaintNumber();
            complaint.StatusId = 1;
            complaint.CreatedAt = DateTime.Now;

            // التحقق من النموذج بعد تعبئة جميع الحقول المطلوبة
            if (ModelState.IsValid)
            {
                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();

                TempData["ComplaintNumber"] = complaint.ComplaintNumber;
                return RedirectToAction(nameof(Create)); // أو إلى صفحة نجاح منفصلة
            }

            ViewBag.ComplaintTypeId = new SelectList(_context.ComplaintTypes, "ComplaintTypeId", "Name", complaint.ComplaintTypeId);
            return View(complaint);
        }

        public IActionResult Success()
        {
            return View();
        }

        // GET: Complaints/Search
        public IActionResult Search()
        {
            return View();
        }

        // POST: Complaints/Search
        [HttpPost]
        public IActionResult Search(string complaintNumber, string nationalId)
        {
            var query = _context.Complaints.AsQueryable();

            if (!string.IsNullOrEmpty(complaintNumber))
            {
                query = query.Where(c => c.ComplaintNumber == complaintNumber);
            }

            if (!string.IsNullOrEmpty(nationalId))
            {
                query = query.Where(c => c.NationalId == nationalId);
            }

            var results = query.Include(c => c.ComplaintType)
                               .Include(c => c.Status)
                               .Include(c => c.MessageReplies)
                               .ToList();

            if (results.Count == 0)
            {
                TempData["NotFound"] = "لم يتم العثور على أي شكوى بالمعلومات المدخلة.";
            }

            return View(results);
        }

        

    }
}

using ComplaintSystem.Data;
using ComplaintSystem.Models;
using ComplaintSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ComplaintSystem.Areas.Communicator.Controllers
{
    [Area("Communicator")]
    [Authorize(Roles = "تواصل")]
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;       

        private readonly ILogger<ComplaintsController> _logger;
                
        public ComplaintsController(ApplicationDbContext context, ILogger<ComplaintsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var complaints = _context.Complaints
        .Include(c => c.Status)
        .Include(c => c.ComplaintType)
        .Where(c => c.Status != null && c.Status.Name == "جديدة")
        .ToList();

            return View(complaints);
        }

        //Get 
        public async Task<IActionResult> Details(int id)
        {
            var complaint = await _context.Complaints
                .Include(c => c.ComplaintType)
                .Include(c => c.Status)
                .Include(c => c.MessageReplies)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
            {
                return NotFound();
            }

            var viewModel = new ComplaintDetailsViewModel
            {
                Complaint = complaint,
                Replies = complaint.MessageReplies
                    .OrderBy(r => r.SentAt)
                    .ToList()
            };

            return View(viewModel);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, string NewReply)
        {
            _logger.LogInformation("وصل إلى POST: Create");

            var complaint = await _context.Complaints
                .Include(c => c.MessageReplies)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(NewReply))
            {
                try
                {
                    var reply = new MessageReply
                    {
                        ComplaintId = id,
                        Message = NewReply,
                        SentAt = DateTime.Now
                    };

                    _context.MessageReplies.Add(reply);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"تم إنشاء شكوى جديدة برقم: {complaint.ComplaintNumber}");


                    TempData["Success"] = "";
                }
                catch
                {
                    TempData["Error"] = "";
                }
            }
            else
            {
                TempData["Error"] = "نص الرد لا يمكن أن يكون فارغًا.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Complaints/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ComplaintTypes = await _context.ComplaintTypes.ToListAsync();
            ViewBag.Employees = await _context.Users
                .Where(u => u.Role!.Name == "مختص")
                .ToListAsync();

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
        public async Task<IActionResult> Create(CreateComplaintViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ComplaintTypes = await _context.ComplaintTypes.ToListAsync();
                ViewBag.Employees = await _context.Users
                    .Where(u => u.Role!.Name == "مختص")
                    .ToListAsync();
                return View(model);
            }



            // استخراج معرف المستخدم من Claims
            int fromUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            try
            {
                // إنشاء الشكوى
                var complaint = new Complaint
                {
                    ComplaintNumber = GenerateComplaintNumber(),
                    CitizenName = model.CitizenName,
                    NationalId = model.NationalId,
                    Phone = model.Phone,
                    Email = model.Email,
                    TransactionNo = model.TransactionNo,
                    Description = model.Description,
                    ComplaintTypeId = model.ComplaintTypeId,
                    StatusId = 1, // حالة أولية مثلاً "جديدة"
                    CreatedAt = DateTime.Now
                };

                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();

                // إنشاء تحويل (Assignment)
                var assignment = new Assignment
                {
                    ComplaintId = complaint.ComplaintId,
                    FromUserId = fromUserId,
                    ToUserId = model.ToUserId,
                    AssignedAt = DateTime.Now,
                    Notes = model.Notes
                };

                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                TempData["Error"] = " " + ex.Message;

                // إعادة تحميل القوائم عند الخطأ
                ViewBag.ComplaintTypes = await _context.ComplaintTypes.ToListAsync();
                ViewBag.Employees = await _context.Users
                    .Where(u => u.Role!.Name == "مختص")
                    .ToListAsync();
                return View(model);
            }

            
        }

        // عرض التحويلات حالة منتهية
        public async Task<IActionResult> Assigned()
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

        //الشكاوي المنتهية
        public async Task<IActionResult> Finshed()
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
                .Where(a => a.ToUserId == currentUserId && a.Complaint.StatusId == 3) // منتهية
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();

            return View(assignments);
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
                               .ToList();

            if (results.Count == 0)
            {
                TempData["NotFound"] = "لم يتم العثور على أي شكوى بالمعلومات المدخلة.";
            }

            return View(results);
        }

        // إغلاق معاملة
        [IgnoreAntiforgeryToken] // يسمح بالوصول من JS مباشرة
        [HttpPost]
        public async Task<IActionResult> Close(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
                return NotFound();

            complaint.StatusId = 5; // مغلقة
            _context.Complaints.Update(complaint);
            await _context.SaveChangesAsync();

            var redirectUrl = Url.Action("Create", "Messages", new { area = "Communicator", complaintId = id });
            return Json(new { redirectUrl });
        }

        public async Task<IActionResult> Assign(int id)
        {
            var complaint = _context.Complaints.Find(id);
            if (complaint == null) return NotFound();

            // جلب الموظفين المختصين (Specialists فقط)
            var specialists = await _context.Users
                .Where(u => u.Role.Name == "مختص") 
                .ToListAsync();

            ViewBag.Specialists = specialists;

            return View(complaint);
        }


        [HttpPost]
        public async Task<IActionResult> Assign(int complaintId, string notes, string specialistId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var complaint = await _context.Complaints.FindAsync(complaintId);
            if (complaint == null) return NotFound();
            


            // إنشاء سجل تحويل جديد
            var assignment = new Assignment
            {
                ComplaintId = complaintId,
                FromUserId = currentUserId, // موظف التواصل الحالي
                ToUserId = int.Parse(specialistId), // الموظف المختص
                AssignedAt = DateTime.Now,
                Notes = notes
            };

            // تحديث حالة الشكوى إلى "قيد المعالجة"
            complaint.StatusId = 2;

            // الحفظ
            _context.Assignments.Add(assignment);
            _context.Complaints.Update(complaint);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم تحويل الشكوى بنجاح.";
            return RedirectToAction("Details", new { id = complaintId });
        }

    }
}

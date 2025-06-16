using ComplaintSystem.Data;
using ComplaintSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComplaintSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدير النظام")]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        // GET: Admin/Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            Console.WriteLine("Role.Name = " + role.Name);

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("ModelState Error: " + error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(role);
                    await _context.SaveChangesAsync();               
                    Console.WriteLine("تم الحفظ بنجاح.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("حدث خطأ أثناء الحفظ: " + ex.Message);
                    Console.WriteLine("التفاصيل: " + ex.InnerException?.Message);
                }
            }

            return View(role);
        }

        // GET: Admin/Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();

            return View(role);
        }

        // POST: Admin/Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId,Name")] Role role)
        {
            if (id != role.RoleId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // فقط نقوم بإرفاق الكيان وتحديث الخصائص التي نحتاجها
                    _context.Attach(role).Property(r => r.Name).IsModified = true;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Roles.Any(e => e.RoleId == id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // POST: Admin/Roles/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}

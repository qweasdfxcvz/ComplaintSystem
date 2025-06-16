using ComplaintSystem.Data;
using Microsoft.AspNetCore.Identity;
using ComplaintSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ComplaintSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدير النظام")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department);
            return View(await users.ToListAsync());
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "Name");
            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "Name");
            ;
            return View();
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("ModelState Error: " + error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                // تشفير كلمة المرور
                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(user, user.PasswordHash);

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "Name", user.RoleId);
            ViewBag.DepartmentId = new SelectList(_context.Departments, "DepartmentId", "Name", user.DepartmentId);
            ;
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "Name", user.RoleId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", user.DepartmentId);
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.UserId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "Name", user.RoleId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", user.DepartmentId);
            return View(user);
        }

        // POST: Admin/Users/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}

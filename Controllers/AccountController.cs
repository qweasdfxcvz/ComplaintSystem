using ComplaintSystem.Data;
using ComplaintSystem.Models;
using ComplaintSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ComplaintSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ابحث عن المستخدم حسب الاسم
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Name == model.UserName);

            if (user == null)
            {
                TempData["LoginError"] = "اسم المستخدم غير صحيح";
                return View(model);
            }

            // تحقق من كلمة المرور
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                TempData["LoginError"] = "كلمة المرور غير صحيحة";
                return View(model);
            }

            // إعداد Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            // تسجيل الدخول بالكوكيز
            await HttpContext.SignInAsync("MyCookieAuth", principal);

            // تخزين الاسم في الجلسة (اختياري للعرض فقط)
            HttpContext.Session.SetString("UserName", user.Name);

            // التوجيه حسب الدور
            return user.Role?.Name switch
            {
                "مدير النظام" => RedirectToAction("Index", "Dashboard", new { area = "Admin" }),
                "تواصل" => RedirectToAction("Index", "Complaints", new { area = "Communicator" }),
                "مختص" => RedirectToAction("Index", "Assignments", new { area = "Specialist" }),
                _ => RedirectToAction("Login")
            };
        }

        // تسجيل الخروج
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

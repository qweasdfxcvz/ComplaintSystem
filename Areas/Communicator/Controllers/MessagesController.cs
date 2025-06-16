using ComplaintSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ComplaintSystem.Models;

namespace ComplaintSystem.Areas.Communicator.Controllers
{
    [Area("Communicator")]
    [Authorize(Roles = "تواصل")]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Create(int id)
        {
            var model = new MessageReply { ComplaintId = id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessageReply model)
        {
            if (ModelState.IsValid)
            {
                _context.MessageReplies.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Finshed", "Complaints", new { area = "Communicator", id = model.ComplaintId });
            }
            return View(model);
        }
    }
}

using ComplaintSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.ViewModels
{
    public class ComplaintDetailsViewModel
    {
        public Complaint Complaint { get; set; } = new();
        public List<MessageReply> Replies { get; set; } = new();

        // لرد جديد
        [Required]
        public string NewReply { get; set; } = string.Empty;
    }
}

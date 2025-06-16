using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.Models
{
    public class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }

        
        [StringLength(20)]
        public string ComplaintNumber { get; set; } = string.Empty;

        [Required]
        public string CitizenName { get; set; } = string.Empty;

        [Required, StringLength(11)]
        public string NationalId { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(11)]
        public string TransactionNo { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // FK
        public int ComplaintTypeId { get; set; }
        public ComplaintType? ComplaintType { get; set; }

        public int StatusId { get; set; }
        public ComplaintStatus? Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public ICollection<MessageReply> MessageReplies { get; set; } = new List<MessageReply>();

    }
}

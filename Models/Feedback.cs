using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        // FK
        public int ComplaintId { get; set; }
        public Complaint Complaint { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public string SubmittedByName { get; set; }

        [StringLength(11)]
        public string NationalId { get; set; }
    }
}

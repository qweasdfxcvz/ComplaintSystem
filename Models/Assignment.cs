using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        // FK
        public int ComplaintId { get; set; }
        public Complaint Complaint { get; set; } 

        public int FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public User FromUser { get; set; }

        public int ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public User ToUser { get; set; } 

        public DateTime AssignedAt { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
}

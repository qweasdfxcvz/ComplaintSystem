using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.Models
{
    public class ComplaintStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        public required string Name { get; set; }

        // Navigation
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}

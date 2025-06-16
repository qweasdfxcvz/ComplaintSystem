using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.Models
{
    public class ComplaintType
    {
        [Key]
        public int ComplaintTypeId { get; set; }

        [Required]
        public required string Name { get; set; }

        // Navigation
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}

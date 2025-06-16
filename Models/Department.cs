using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        public required string Name { get; set; }

        public required string ContactPhone { get; set; }

        public required string Email { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

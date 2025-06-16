using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplaintSystem.Models
{
    public class MessageReply
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.Now;
        
        //FK
        public int ComplaintId { get; set; }
        
        [ForeignKey("ComplaintId")]
        public Complaint? Complaint { get; set; }
    }
}

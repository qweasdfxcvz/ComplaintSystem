using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.ViewModels
{
    public class CreateComplaintViewModel
    {
        // بيانات الشكوى
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

        [Required]
        public int ComplaintTypeId { get; set; }

        // تحويل لموظف
        [Required]
        public int ToUserId { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.ViewModels
{
    public class ComplaintWithAssignmentViewModel
    {
        // بيانات الشكوى

        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "اسم المواطن")]
        public string CitizenName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرقم الوطني مطلوب")]
        [StringLength(11, ErrorMessage = "الرقم الوطني يجب أن يكون 11 رقم")]
        [Display(Name = "الرقم الوطني")]
        public string NationalId { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
        [Display(Name = "رقم الهاتف")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم المعاملة مطلوب")]
        [StringLength(11, ErrorMessage = "رقم المعاملة يجب أن يكون 11 رقم")]
        [Display(Name = "رقم المعاملة")]
        public string TransactionNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "الوصف مطلوب")]
        [Display(Name = "وصف الشكوى")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "نوع الشكوى مطلوب")]
        [Display(Name = "نوع الشكوى")]
        public int ComplaintTypeId { get; set; }

        // التحويل

        [Required(ErrorMessage = "يجب اختيار الموظف المحول إليه")]
        [Display(Name = "تحويل إلى موظف")]
        public int ToUserId { get; set; }

        [Display(Name = "ملاحظات التحويل")]
        public string Notes { get; set; } = string.Empty;
    }
}

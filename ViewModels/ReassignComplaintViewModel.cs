using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.ViewModels
{
    public class ReassignComplaintViewModel
    {
        public int ComplaintId { get; set; }

        [Required(ErrorMessage = "الرجاء اختيار الموظف المختص.")]
        public int ToUserId { get; set; }

        [Required(ErrorMessage = "الرجاء إدخال الملاحظات.")]
        public string Notes { get; set; } = string.Empty;

        public string ComplaintNumber { get; set; } = string.Empty;
        public int? SelectedStatusId { get; set; } // اختياري
        public List<SelectListItem> AvailableStatuses { get; set; }

        public List<SelectListItem> Specialists { get; set; } = new();
    }
}

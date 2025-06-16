using System.ComponentModel.DataAnnotations;

namespace ComplaintSystem.ViewModels
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

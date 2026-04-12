using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name / Email")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.AdminUsers
{
    public class AdminUserEditViewModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string SelectedRole { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public List<string> AvailableRoles { get; set; } = new();
    }
}
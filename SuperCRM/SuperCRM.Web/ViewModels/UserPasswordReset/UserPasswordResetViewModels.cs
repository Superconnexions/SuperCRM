using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.UserPasswordReset
{
    public class UserPasswordResetListViewModel
    {
        public string? Search { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalUsers { get; set; }
        public List<UserPasswordResetRowViewModel> Users { get; set; } = new();
    }

    public class UserPasswordResetRowViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminResetUserPasswordViewModel
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? UserType { get; set; }
        public bool IsActive { get; set; }

        [Required, DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        [Display(Name = "New Password")]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.EmailSettings
{
    public class TestEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Test Recipient Email")]
        public string ToEmail { get; set; } = string.Empty;
    }
}

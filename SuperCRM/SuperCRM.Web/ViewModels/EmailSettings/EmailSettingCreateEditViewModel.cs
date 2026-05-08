using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.EmailSettings
{
    public class EmailSettingCreateEditViewModel
    {
        public Guid? EmailSettingId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Setting Name")]
        public string SettingName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "SMTP Server")]
        public string SmtpServer { get; set; } = "smtp.gmail.com";

        [Required]
        public int Port { get; set; } = 587;

        [Required]
        [StringLength(200)]
        [Display(Name = "Sender Name")]
        public string SenderName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        [Display(Name = "Sender Email")]
        public string SenderEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Username { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Enable SSL/TLS")]
        public bool EnableSsl { get; set; } = true;

        [Display(Name = "Default")]
        public bool IsDefault { get; set; } = true;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}

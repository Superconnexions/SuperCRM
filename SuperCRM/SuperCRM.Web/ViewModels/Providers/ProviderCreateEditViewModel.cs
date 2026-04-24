using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.Providers;

public class ProviderCreateEditViewModel
{
    public Guid? ProviderId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Provider Name")]
    public string ProviderName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(150)]
    [Display(Name = "Contact Email")]
    public string? ContactEmail { get; set; }

    [StringLength(50)]
    [Display(Name = "Contact Phone")]
    public string? ContactPhone { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Provider URL")]
    [StringLength(500)]
    [Url(ErrorMessage = "Please enter a valid URL.")]
    public string? ProviderUrl { get; set; }

    [Display(Name = "Provider Address")]
    [StringLength(200)]
    public string? ProviderAddress { get; set; }


}
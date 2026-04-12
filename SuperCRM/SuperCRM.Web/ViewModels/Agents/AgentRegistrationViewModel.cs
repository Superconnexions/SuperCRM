using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuperCRM.Web.ViewModels.AgentRegistration
{
    /// <summary>
    /// UI ViewModel for Agent self-registration page.
    /// Keeps UI-specific concerns in Web layer:
    /// - input validation
    /// - dropdown collections
    /// - confirm password
    /// </summary>
    public class AgentRegistrationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Phone No")]
        public string? PhoneNo { get; set; }

        [StringLength(50)]
        [Display(Name = "Mobile No")]
        public string? MobileNo { get; set; }

        [StringLength(100)]
        [Display(Name = "House No")]
        public string? HouseNo { get; set; }

        [StringLength(150)]
        [Display(Name = "Road Name")]
        public string? RoadName { get; set; }

        [StringLength(200)]
        [Display(Name = "Address Line 1")]
        public string? AddressLine1 { get; set; }

        [StringLength(200)]
        [Display(Name = "Address Line 2")]
        public string? AddressLine2 { get; set; }

        [StringLength(20)]
        [Display(Name = "Post Code")]
        public string? PostCode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Required]
        [Display(Name = "Region")]
        public int? RegionId { get; set; }

        [Required]
        [Display(Name = "City")]
        public int? CityId { get; set; }

        /// <summary>
        /// Dropdown data sources.
        /// </summary>
        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> Regions { get; set; } = new();
        public List<SelectListItem> Cities { get; set; } = new();
    }
}
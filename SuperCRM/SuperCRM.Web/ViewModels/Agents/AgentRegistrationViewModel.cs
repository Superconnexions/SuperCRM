using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.Agents
{
    /// <summary>
    /// Public form model for agent self-registration.
    /// This model is bound in the Web layer and then mapped to an Application DTO.
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

        [StringLength(100)]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(20)]
        [Display(Name = "Post Code")]
        public string? PostCode { get; set; }

        [Display(Name = "Country Id")]
        public int? CountryId { get; set; }
    }
}

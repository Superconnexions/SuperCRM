using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.AgentManagement
{
    /// <summary>
    /// View/Edit registration page model for SuperAdmin/SuperCRMAdmin.
    /// IsApproved is display-only and is controlled by service workflow.
    /// </summary>
    public class AgentRegistrationEditViewModel
    {
        public Guid AgentId { get; set; }
        public Guid UserId { get; set; }

        [Display(Name = "Agent Code")]
        public string AgentCode { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

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

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Is Commission Eligible")]
        public bool IsCommissionEligible { get; set; }

        [Required]
        [Display(Name = "Registration Status")]
        public byte RegistrationStatus { get; set; }

        [Display(Name = "Approved At")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }

        public List<SelectListItem> RegistrationStatusOptions { get; set; } = new();
    }
}

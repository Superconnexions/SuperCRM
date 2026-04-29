using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.AgentManagement
{
    /// <summary>
    /// Agent management list with search criteria and result rows.
    /// </summary>
    public class AgentListViewModel
    {
        [Display(Name = "Search Text")]
        public string? SearchText { get; set; }

        [Display(Name = "Is Approved")]
        public bool? IsApproved { get; set; }

        [Display(Name = "Registration Status")]
        public byte? RegistrationStatus { get; set; }

        public List<SelectListItem> IsApprovedOptions { get; set; } = new();
        public List<SelectListItem> RegistrationStatusOptions { get; set; } = new();
        public List<AgentListItemViewModel> Items { get; set; } = new();
    }
}

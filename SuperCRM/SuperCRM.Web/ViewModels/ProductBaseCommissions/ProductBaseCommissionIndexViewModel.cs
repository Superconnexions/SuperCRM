using SuperCRM.Application.DTOs.ProductBaseCommissions;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.ProductBaseCommissions
{
    public class ProductBaseCommissionIndexViewModel
    {
        public string? ProductKeyword { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        [Display(Name = "Include Inactive")]
        public bool IncludeInactive { get; set; }
        public List<ProductBaseCommissionDto> Items { get; set; } = new();
    }
}

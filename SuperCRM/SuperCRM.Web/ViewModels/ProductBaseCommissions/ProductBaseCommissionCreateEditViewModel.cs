using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Web.ViewModels.ProductBaseCommissions
{
    public class ProductBaseCommissionCreateEditViewModel
    {
        public Guid? ProductBaseCommissionId { get; set; }

        [Required]
        [Display(Name = "Product")]
        public Guid ProductId { get; set; }

        [Required]
        [Display(Name = "Commission Type")]
        public CommissionType CommissionType { get; set; }

        [Display(Name = "Fixed Amount")]
        public decimal? FixedAmount { get; set; }

        [Display(Name = "Percentage")]
        public decimal? Percentage { get; set; }

        [Display(Name = "Effective From")]
        public DateTime? EffectiveFrom { get; set; }

        [Display(Name = "Effective To")]
        public DateTime? EffectiveTo { get; set; }

        [Display(Name = "Note")]
        [StringLength(500)]
        public string? Note { get; set; }
        
        [Required(ErrorMessage = "Currency is required.")]
        [Display(Name = "Currency")]
        public string CurrencyCode { get; set; } = string.Empty;

        public List<SelectListItem> CurrencyOptions { get; set; } = new();

        public List<SelectListItem> ProductOptions { get; set; } = new();
    }
}

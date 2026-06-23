using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.ProductBaseCommissions
{
    public class ProductVariantCommissionOverrideCreateEditViewModel
    {
        public Guid ProductVariantCommissionOverrideId { get; set; }

        [Required]
        [Display(Name = "Product")]
        public Guid ProductId { get; set; }

        [Required]
        [Display(Name = "Variant")]
        public Guid ProductVariantId { get; set; }

        [Required]
        [Display(Name = "Extra Commission Amount")]
        public decimal ExtraCommissionAmount { get; set; }

        [Display(Name = "Effective From")]
        public DateTime? EffectiveFrom { get; set; }

        [Display(Name = "Effective To")]
        public DateTime? EffectiveTo { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public string? Note { get; set; }

        public List<SelectListItem> Products { get; set; } = new();
        public List<SelectListItem> Variants { get; set; } = new();
    }
}
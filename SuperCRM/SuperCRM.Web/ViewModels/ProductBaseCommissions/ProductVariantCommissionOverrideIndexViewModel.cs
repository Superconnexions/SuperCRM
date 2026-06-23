using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.ProductBaseCommissions
{
    public class ProductVariantCommissionOverrideIndexViewModel
    {
        [Display(Name = "Product")]
        public string? ProductKeyword { get; set; }

        public List<ProductVariantCommissionOverrideListItemViewModel> Items { get; set; } = new();
    }

    public class ProductVariantCommissionOverrideListItemViewModel
    {
        public Guid ProductVariantCommissionOverrideId { get; set; }

        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";

        public string VariantCode { get; set; } = "";
        public string VariantName { get; set; } = "";

        public decimal ExtraCommissionAmount { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; }
        public string? Note { get; set; }
    }
}
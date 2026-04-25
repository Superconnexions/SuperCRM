using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.ProductVariants
{
    /// <summary>
    /// UI ViewModel for Product Variant Create/Edit page.
    /// </summary>
    public class ProductVariantCreateEditViewModel
    {
        public Guid? ProductVariantId { get; set; }

        [Required]
        [Display(Name = "Product")]
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Variant Code")]
        public string VariantCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Variant Type")]
        public string VariantTypeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Variant Name")]
        public string VariantName { get; set; } = string.Empty;

        //[Required]
        //[Display(Name = "Display Style")]
        //[Range(1, 255)]
        //public byte DisplayStyle { get; set; } = 0;

        //public DisplayStyle? DisplayStyle { get; set; }

        [Required(ErrorMessage = "Display Style is required.")]
        [Display(Name = "Display Style")]
        public DisplayStyle? DisplayStyle { get; set; }


        [Required]
        [Display(Name = "Display Order")]
        [Range(1, 255)]
        public int DisplayOrder { get; set; } = 0;

        [Display(Name = "Base Price")]
        [Range(0, 999999999)]
        public decimal? BasePrice { get; set; }

        public bool IsEditMode { get; set; }

        public List<SelectListItem> ProductOptions { get; set; } = new();
        public List<SelectListItem> VariantTypeOptions { get; set; } = new();
        public List<SelectListItem> DisplayStyleOptions { get; set; } = new();
    }
}

using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.ProductVariantTypes
{
    public class ProductVariantTypeCreateEditViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Type Code")]
        public string TypeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Type Value")]
        public string TypeValue { get; set; } = string.Empty;

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public bool IsEditMode { get; set; }
    }
}
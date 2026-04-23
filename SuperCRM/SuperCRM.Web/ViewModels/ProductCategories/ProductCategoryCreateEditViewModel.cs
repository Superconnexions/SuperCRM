using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.ProductCategories
{
    /// <summary>
    /// UI ViewModel for Create/Edit Product Category page.
    /// Keeps Web-layer validation and page-only flags.
    /// </summary>
    public class ProductCategoryCreateEditViewModel
    {
        public Guid? CategoryId { get; set; }

        [StringLength(50)]
        [Display(Name = "Category Code")]
        public string? CategoryCode { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Category Image URL")]
        public string? CategoryImageUrl { get; set; }

        [StringLength(500)]
        [Display(Name = "Display Notes")]
        public string? DisplayNotes { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public bool IsEditMode { get; set; }
    }
}

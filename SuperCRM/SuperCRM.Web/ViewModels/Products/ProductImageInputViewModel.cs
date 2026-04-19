using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.Products
{
    /// <summary>
    /// UI ViewModel for one image row on Product Create/Edit page.
    /// </summary>
    public class ProductImageInputViewModel
    {
        public Guid? ProductImageId { get; set; }

        [Display(Name = "Existing Image URL")]
        public string? ExistingImageUrl { get; set; }

        [Display(Name = "Image File")]
        public IFormFile? UploadFile { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Primary")]
        public bool IsPrimary { get; set; }

        [Display(Name = "Remove")]
        public bool RemoveImage { get; set; }
    }
}

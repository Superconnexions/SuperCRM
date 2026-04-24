using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.ProviderProducts
{
    public class ProviderProductCreateEditViewModel
    {
        public Guid? ProviderProductId { get; set; }

        [Display(Name = "Provider")]
        [Required(ErrorMessage = "Provider is required.")]
        public Guid ProviderId { get; set; }

        [Display(Name = "Product")]
        [Required(ErrorMessage = "Product is required.")]
        public Guid ProductId { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; } = string.Empty;

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        public bool IsEditMode { get; set; }

        public List<SelectListItem> ProviderOptions { get; set; } = new();
        public List<SelectListItem> ProductOptions { get; set; } = new();
    }
}

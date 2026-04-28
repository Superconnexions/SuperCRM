using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.Products
{
    /// <summary>
    /// UI ViewModel for Product Create/Edit page.
    /// </summary>
    public class ProductCreateEditViewModel
    {
        public Guid? ProductId { get; set; }

        [Required]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        [Required]
        [Display(Name = "Sales Unit")]
        public int SalesUnitId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Product Display Name")]
        public string ProductDisplayName { get; set; } = string.Empty;

        [Display(Name = "Product Type")]
        public ProductType ProductType { get; set; } = ProductType.SimpleProduct;

        [Display(Name = "Customer Type")]
        public ProductCustomerType? CustomerType { get; set; } = ProductCustomerType.Both;

        [StringLength(1000)]
        [Display(Name = "Product Description")]
        public string? ProductDescription { get; set; }

        [Display(Name = "Third Party Product")]
        public bool IsThirdPartyProduct { get; set; }

        [Display(Name = "Installment Applicable")]
        public bool InstallmentApplicable { get; set; }

        [Display(Name = "Down Payment Amount")]
        [Range(0, 999999999)]
        public decimal? DownPaymentAmount { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Currency Code")]
        public string CurrencyCode { get; set; } = "£"; // GBP

        [Display(Name = "Required Bank Information")]
        public bool IsRequiredBankInformation { get; set; }

        [Display(Name = "Provider Delivery Product")]
        public bool IsProviderDeliveryProduct { get; set; }

        [Display(Name = "Base Price Type")]
        public ProductBasePriceType BasePriceType { get; set; } = ProductBasePriceType.SimplePrice;

        [Display(Name = "Base Price")]
        [Range(0, 999999999)]
        public decimal BasePrice { get; set; }

        [Display(Name = "Price Editable")]
        public bool IsPriceEditable { get; set; }

        [Display(Name = "Portal Visible")]
        public bool IsPortalVisible { get; set; }

        [Display(Name = "Portal Order Enabled")]
        public bool IsPortalOrderEnabled { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "No of Installment")]
        public int NoOfInstallment { get; set; }

        [Display(Name = "Monthly Installment Amount")]
        [Range(0, 999999999)]
        public decimal MonthlyInstallmentAmount { get; set; }

        [StringLength(1000)]
        [Display(Name = "Product Display Notes")]
        public string? ProductDisplayNotes { get; set; }

        [StringLength(1000)]
        [Display(Name = "Payment Notes")]
        public string? PaymentNotes { get; set; }

        [StringLength(1000)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public bool IsEditMode { get; set; }

        public List<ProductImageInputViewModel> Images { get; set; } = new()
        {
            new ProductImageInputViewModel
            {
                DisplayOrder = 1,
                IsPrimary = true
            }
        };

        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<SelectListItem> SalesUnitOptions { get; set; } = new();
        public List<SelectListItem> ProductTypeOptions { get; set; } = new();
        public List<SelectListItem> CustomerTypeOptions { get; set; } = new();
        public List<SelectListItem> BasePriceTypeOptions { get; set; } = new();
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderCustomerCreationViewModel
    {
        public Guid SalesOrderDraftId { get; set; }
        public string DraftNo { get; set; } = string.Empty;
        public bool HasResidentialProduct { get; set; }
        public bool HasBusinessProduct { get; set; }
        public bool HasMixedBusinessResidential { get; set; }
        public bool IsResidentialOnly { get; set; }
        public bool IsBusinessFlow { get; set; }
        public bool RequiresBankInformation { get; set; }
        public string ScenarioName { get; set; } = string.Empty;

        public Guid? ExistingCustomerId { get; set; }
        public string? ExistingCustomerDisplayText { get; set; }

        [Display(Name = "Business Type")]
        public byte BusinessType { get; set; } = 1;

        public bool IsBusinessAddressSameAsPersonal { get; set; } = false;

        public SalesOrderCustomerPersonInputViewModel Customer { get; set; } = new();
        public SalesOrderCustomerAddressInputViewModel PersonalAddress { get; set; } = new();
        public SalesOrderCustomerBusinessInputViewModel Business { get; set; } = new();
        public SalesOrderCustomerAddressInputViewModel BusinessAddress { get; set; } = new();
        public SalesOrderCustomerBankAccountInputViewModel BankAccount { get; set; } = new();
        public List<SalesOrderSelectedProductSummaryViewModel> Products { get; set; } = new();
        public List<SelectListItem> CountryOptions { get; set; } = new();
        public List<SelectListItem> RegionOptions { get; set; } = new();
        public bool ShowPersonalAddress { get; set; }
        public bool ShowBusinessAddressSameCheckbox { get; set; }
        public bool IsCustomerSavedForOrder { get; set; }
    }

    public class SalesOrderCustomerPersonInputViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        [Display(Name = "Alternate Email")]
        public string? AlternativeEmail { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
    }

    public class SalesOrderCustomerAddressInputViewModel
    {
        [Display(Name = "House No")]
        public string? HouseNo { get; set; }
        [Display(Name = "Road Name")]
        public string? RoadName { get; set; }
        [Display(Name = "Post Code")]
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        [Display(Name = "Address Line")]
        public string? AddressLine { get; set; }

        public List<SelectListItem> CountryOptions { get; set; } = new();
        public List<SelectListItem> RegionOptions { get; set; } = new();
    }

    public class SalesOrderCustomerBusinessInputViewModel
    {
        [Display(Name = "Business Name")]
        public string? BusinessName { get; set; }
        [Display(Name = "Business Email")]
        public string? BusinessEmail { get; set; }
        [Display(Name = "Trading Name")]
        public string? TradingName { get; set; }
        [Display(Name = "Registration No")]
        public string? RegistrationNo { get; set; }
        [Display(Name = "Contact Person Name")]
        public string? ContactPersonName { get; set; }
        [Display(Name = "Contact Person Phone")]
        public string? ContactPersonPhone { get; set; }
    }

    public class SalesOrderCustomerBankAccountInputViewModel
    {
        [Display(Name = "Bank Name")]
        public string? BankName { get; set; }
        [Display(Name = "Account Name")]
        public string? AccountName { get; set; }
        [Display(Name = "Account Number")]
        public string? AccountNumber { get; set; }
        [Display(Name = "Sort Code")]
        public string? SortCode { get; set; }
    }

    public class SalesOrderSelectedProductSummaryViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string? VariantName { get; set; }
        public string ProviderName { get; set; } = "SuperCRM";
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal LineTotalAmount { get; set; }
        public string? CurrencyCode { get; set; }
        public bool IsInstallmentSelected { get; set; }
        public string InstallmentSummary { get; set; } = string.Empty;
        public string? SalesUnitCode { get; set; }
    }
}

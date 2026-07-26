using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class CustomerEditViewModel
    {
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string RegistrationSourceText { get; set; } = string.Empty;
        public CustomerBasicInfoEditViewModel Customer { get; set; } = new();
        public List<CustomerAddressEditViewModel> Addresses { get; set; } = new();
        public CustomerBusinessEditViewModel Business { get; set; } = new();
        public CustomerBankAccountEditViewModel BankAccount { get; set; } = new();
        public List<SelectListItem> CountryOptions { get; set; } = new();
        public List<SelectListItem> RegionOptions { get; set; } = new();
        public List<SelectListItem> AddressTypeOptions { get; set; } = new();
        public List<SelectListItem> BusinessTypeOptions { get; set; } = new();
    }

    public class CustomerBasicInfoEditViewModel
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public bool IsCompanyDirector { get; set; }
    }

    public class CustomerAddressEditViewModel
    {
        public Guid CustomerId { get; set; }
        public Guid? CustomerAddressId { get; set; }
        public byte AddressType { get; set; } = (byte)SuperCRM.Domain.Enums.AddressType.Personal;
        public string AddressTypeText { get; set; } = string.Empty;
        public string? AddressLine { get; set; }
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? CityId { get; set; }
        public string? CountryName { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CustomerBusinessEditViewModel
    {
        public Guid CustomerId { get; set; }
        public Guid? CustomerBusinessId { get; set; }
        public byte BusinessType { get; set; } = (byte)CustomerBusinessType.Solo;
        public string? BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public string? TradingName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonPhone { get; set; }
    }

    public class CustomerBankAccountEditViewModel
    {
        public Guid CustomerId { get; set; }
        public Guid? CustomerBankAccountId { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SortCode { get; set; }
    }
}

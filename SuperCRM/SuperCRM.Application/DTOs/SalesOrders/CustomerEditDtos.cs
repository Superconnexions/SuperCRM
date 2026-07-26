using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class CustomerEditPageDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public byte RegistrationSource { get; set; }
        public string RegistrationSourceText { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public bool IsCompanyDirector { get; set; }
        public List<CustomerEditAddressDto> Addresses { get; set; } = new();
        public CustomerEditBusinessDto Business { get; set; } = new();
        public CustomerEditBankAccountDto BankAccount { get; set; } = new();
        public List<SalesOrderLookupOptionDto> Countries { get; set; } = new();
        public List<SalesOrderLookupOptionDto> Regions { get; set; } = new();
    }

    public class CustomerEditAddressDto
    {
        public Guid CustomerAddressId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? CustomerBusinessId { get; set; }
        public byte AddressType { get; set; }
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
        public string? RegionName { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CustomerEditBusinessDto
    {
        public Guid? CustomerBusinessId { get; set; }
        public Guid CustomerId { get; set; }
        public byte BusinessType { get; set; } = (byte)CustomerBusinessType.Solo;
        public string? BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public string? TradingName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonPhone { get; set; }
    }

    public class CustomerEditBankAccountDto
    {
        public Guid? CustomerBankAccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SortCode { get; set; }
    }

    public class UpdateCustomerBasicInfoDto
    {
        public Guid CustomerId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool CanManageAllCustomers { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public bool IsCompanyDirector { get; set; }
    }

    public class SaveCustomerAddressDto
    {
        public Guid CustomerId { get; set; }
        public Guid? CustomerAddressId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool CanManageAllCustomers { get; set; }
        public byte AddressType { get; set; }
        public string? AddressLine { get; set; }
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? CityId { get; set; }
        public bool IsDefault { get; set; }
    }

    public class UpdateCustomerBusinessDto
    {
        public Guid CustomerId { get; set; }
        public Guid? CustomerBusinessId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool CanManageAllCustomers { get; set; }
        public byte BusinessType { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public string? TradingName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonPhone { get; set; }
    }

    public class UpdateCustomerBankAccountDto
    {
        public Guid CustomerId { get; set; }
        public Guid? CustomerBankAccountId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool CanManageAllCustomers { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SortCode { get; set; }
    }

    public class CustomerEditResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid? RecordId { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }
}

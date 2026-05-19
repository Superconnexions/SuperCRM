namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderCustomerCreationPageDto
    {
        public Guid SalesOrderDraftId { get; set; }
        public string DraftNo { get; set; } = string.Empty;
        public SalesOrderCustomerRequirementDto Requirement { get; set; } = new();
        public List<SalesOrderSelectedProductSummaryDto> Products { get; set; } = new();
        public List<SalesOrderLookupOptionDto> Countries { get; set; } = new();
        public List<SalesOrderLookupOptionDto> Regions { get; set; } = new();
        public Guid? SelectedCustomerId { get; set; }
        public Guid? SelectedCustomerBusinessId { get; set; }
        public Guid? SelectedCustomerAddressId { get; set; }
        public Guid? SelectedCustomerBankAccountId { get; set; }
        public SalesOrderCustomerDto? Customer { get; set; }
        public SalesOrderCustomerAddressDto? PersonalAddress { get; set; }
        public SalesOrderBusinessDto? Business { get; set; }
        public SalesOrderCustomerAddressDto? BusinessAddress { get; set; }
        public SalesOrderBankAccountDto? BankAccount { get; set; }

    }

    public class SalesOrderCustomerDto
    {
        public Guid CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public bool IsCompanyDirector { get; set; }
    }

    public class SalesOrderCustomerAddressDto
    {
        public Guid? CustomerAddressId { get; set; }
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public string? AddressLine { get; set; }
    }

    public class SalesOrderBusinessDto
    {
        public Guid? CustomerBusinessId { get; set; }
        public byte? BusinessType { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public string? TradingName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonPhone { get; set; }
    }

    public class SalesOrderBankAccountDto
    {
        public Guid? CustomerBankAccountId { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SortCode { get; set; }
    }
}

using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SaveSalesOrderCustomerDto
    {
        public Guid SalesOrderDraftId { get; set; }
        public Guid CurrentUserId { get; set; }
        public Guid? ExistingCustomerId { get; set; }
        public byte BusinessType { get; set; }
        public bool IsBusinessFlow { get; set; }
        public bool RequiresBankInformation { get; set; }
        public bool IsBusinessAddressSameAsPersonal { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public RegistrationSource RegistrationSource { get; set; }

        //public SaveSalesOrderAddressDto PersonalAddress { get; set; } = new();
        public SaveSalesOrderAddressDto? PersonalAddress { get; set; }
        public SaveSalesOrderBusinessDto Business { get; set; } = new();
        public SaveSalesOrderAddressDto BusinessAddress { get; set; } = new();
        public SaveSalesOrderBankAccountDto BankAccount { get; set; } = new();
    }

    public class SaveSalesOrderAddressDto
    {
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int? RegionId { get; set; }
        public string? AddressLine { get; set; }
    }

    public class SaveSalesOrderBusinessDto
    {
        public string? BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public string? TradingName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonPhone { get; set; }
    }

    public class SaveSalesOrderBankAccountDto
    {
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SortCode { get; set; }
    }
}

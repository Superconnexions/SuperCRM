namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class CustomerManagementListDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public byte RegistrationSource { get; set; }
        public string RegistrationSourceText { get; set; } = "";
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? EmailOrMobile { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SalesOrderCount { get; set; }
        public bool HasBusiness { get; set; }
        public bool HasBank { get; set; }
    }

    public class CustomerSalesOrderListDto
    {
        public Guid SaleId { get; set; }
        public string OrderNo { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public string ProviderName { get; set; } = "SuperCRM";
        public byte SalesOrderStatus { get; set; }
        public string SalesOrderStatusText { get; set; } = "";
        public decimal OrderTotal { get; set; }
    }

    public class CustomerAddressListDto
    {
        public byte AddressType { get; set; }
        public string AddressTypeText { get; set; } = "";
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public string? CountryName { get; set; }
    }

    public class CustomerBusinessViewDto
    {
        public string? BusinessName { get; set; }
        public byte BusinessType { get; set; }
        public string BusinessTypeText { get; set; } = "";
        public string? BusinessEmail { get; set; }
        public string? TradingName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonPhone { get; set; }
    }

    public class CustomerBankAccountViewDto
    {
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SortCode { get; set; }
    }
}
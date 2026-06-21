using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class CreateSalesOrderFromDraftRequestDto
    {
        public Guid SalesOrderDraftId { get; set; }
        public Guid CurrentUserId { get; set; }
        public OrderSourceType OrderSourceType { get; set; }
        public List<CreateSalesOrderLineSpecialNoteDto> LineSpecialNotes { get; set; } = new();
    }

    public class CreateSalesOrderLineSpecialNoteDto
    {
        public Guid SalesOrderDraftLineId { get; set; }
        public string? SpecialNotes { get; set; }
    }
    public class CreateSalesOrderResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid SalesOrderDraftId { get; set; }
        public List<Guid> SaleIds { get; set; } = new();
    }

    public class SalesOrderCreatedSummaryDto
    {
        public Guid SalesOrderDraftId { get; set; }
        public string DraftNo { get; set; } = string.Empty;
        public SalesOrderCustomerSummaryDto Customer { get; set; } = new();
        public SalesOrderBusinessSummaryDto? Business { get; set; }
        public SalesOrderAddressSummaryDto? HomeAddress { get; set; }
        public SalesOrderAddressSummaryDto? BusinessAddress { get; set; }
        public SalesOrderBankAccountSummaryDto? BankAccount { get; set; }
        public List<SalesOrderProviderSummaryDto> Orders { get; set; } = new();
    }

    public class SalesOrderProviderSummaryDto
    {
        public Guid SaleId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public Guid? ProviderId { get; set; }
        public string ProviderName { get; set; } = "SuperCRM";
        public string? ProviderEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public decimal OrderTotal { get; set; }
        public decimal AgentCommissionAmount { get; set; }
        public bool HasResidentialLines { get; set; }
        public bool HasBusinessLines { get; set; }
        public byte SalesOrderStatus { get; set; }
        public string SalesOrderStatusText { get; set; } = string.Empty;
        public List<SalesOrderLineSummaryDto> Lines { get; set; } = new();
        public List<SalesOrderInstallmentScheduleSummaryDto> Installments { get; set; } = new();
    }

    public class SalesOrderLineSummaryDto
    {
        public Guid SaleLineId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? VariantName { get; set; }
        public string? ProviderProductName { get; set; }
        public int Quantity { get; set; }
        public string SalesUnitCode { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal LineTotalAmount { get; set; }
        public bool IsInstallment { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
        public int? NoOfInstallment { get; set; }
        public decimal CalculatedAgentCommission { get; set; }
        public decimal FinalAgentCommission { get; set; }
        public string CurrencyCode { get; set; } = "£";
        public string? SpecialNotes { get; set; }
    }

    public class SalesOrderInstallmentScheduleSummaryDto
    {
        public Guid InstallmentScheduleId { get; set; }
        public Guid SaleLineId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int InstallmentNo { get; set; }
        public decimal InstallmentAmount { get; set; }
        public DateTime DueDate { get; set; }
        public byte PaymentStatus { get; set; }
        public string? PaymentStatusText { get; set; }
    }

    public class SalesOrderCustomerSummaryDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
    }

    public class SalesOrderBusinessSummaryDto
    {
        public Guid CustomerBusinessId { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public string? TradingName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonPhone { get; set; }
        public byte BusinessType { get; set; }
    }

    public class SalesOrderAddressSummaryDto
    {
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public string? AddressLine { get; set; }
        public string? CountryName { get; set; }
        public string? RegionName { get; set; }
    }

    public class SalesOrderBankAccountSummaryDto
    {
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SortCode { get; set; }
    }
}

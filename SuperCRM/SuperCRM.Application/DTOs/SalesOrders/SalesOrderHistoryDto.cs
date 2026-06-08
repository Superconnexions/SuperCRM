namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderHistoryDto
    {
        public Guid SaleId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string? CustomerMobile { get; set; }

        public Guid? ProviderId { get; set; }
        public string ProviderName { get; set; } = "SuperCRM";

        public byte SalesOrderStatus { get; set; }
        public string SalesOrderStatusText { get; set; } = string.Empty;

        public bool IsCommissionApplicable { get; set; }
        public string CommissionApplicableText => IsCommissionApplicable ? "Yes" : "No";
        public string CommissionFinalizedText { get; set; } = "No";

        public DateTime? ServiceStartDate { get; set; }
        public DateTime? NextRenewDate { get; set; }
        public int NoOfRenew { get; set; }

        public decimal OrderTotal { get; set; }
        public decimal AgentCommissionAmount { get; set; }

        public int TotalLines { get; set; }
        public int CompletedLines { get; set; }
        public int CancelledOrRejectedLines { get; set; }

        public Guid? SoldByUserId { get; set; }
        public Guid? SoldByAgentId { get; set; }
        public string? SoldByAgentCode { get; set; }

        public bool EmailSentToCustomer { get; set; }
        public bool EmailSentToProvider { get; set; }
    }
}
namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class AdminUserOptionDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class SalesOrderManagementDetailDto
    {
        public Guid SaleId { get; set; }
        public string OrderNo { get; set; } = "";
        public byte SalesOrderStatus { get; set; }

        public DateTime? ServiceStartDate { get; set; }
        public DateTime? NextRenewDate { get; set; }
        public int NoOfRenew { get; set; }
        public bool EmailSentToProvider { get; set; }
        public bool EmailSentToCustomer { get; set; }
        public string? SpecialNotes { get; set; }

        public DateTime? SentToProviderDate { get; set; }
        public Guid? SentToProviderUserId { get; set; }

        public DateTime? ProviderAcceptedDate { get; set; }
        public Guid? ProviderAcceptedUserId { get; set; }

        public DateTime? ProviderRejectedDate { get; set; }
        public Guid? ProviderRejectedUserId { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime? OnHoldDate { get; set; }
        public Guid? OnHoldByUserId { get; set; }
        public string? OnHoldReason { get; set; }

        public DateTime? CancelledDate { get; set; }
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledReason { get; set; }
        public decimal ProviderCommissionEarned { get; set; }
        public bool IsProviderCommissionReceived { get; set; }

        public List<AdminUserOptionDto> AdminUsers { get; set; } = new();
        public List<SalesOrderManagementLineDto> Lines { get; set; } = new();
    }

    public class SalesOrderManagementLineDto
    {
        public Guid SaleLineId { get; set; }
        public string ProductName { get; set; } = "";
        public string? VariantName { get; set; }
        public decimal LineTotalAmount { get; set; }
        public decimal CalculatedAgentCommission { get; set; }
        public decimal FinalAgentCommission { get; set; }
        public bool IsCommissionFinalized { get; set; }
        public int Quantity { get; set; }
        public decimal SuperCRMCommissionEarned { get; set; }
    }

    public class UpdateSalesInformationDto
    {
        public Guid SaleId { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? NextRenewDate { get; set; }
        public int NoOfRenew { get; set; }
        public bool EmailSentToProvider { get; set; }
        public bool EmailSentToCustomer { get; set; }
        public string? SpecialNotes { get; set; }
        public Guid UpdatedByUserId { get; set; }
    }

    public class UpdateSalesCommissionDto
    {
        public Guid SaleId { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public List<UpdateSalesCommissionLineDto> Lines { get; set; } = new();
    }

    public class UpdateSalesCommissionLineDto
    {
        public Guid SaleLineId { get; set; }
        public decimal FinalAgentCommission { get; set; }
    }

    public class UpdateSalesOrderStatusDto
    {
        public Guid SaleId { get; set; }
        public byte SalesOrderStatus { get; set; }

        public DateTime? SentToProviderDate { get; set; }
        public Guid? SentToProviderUserId { get; set; }

        public DateTime? ProviderAcceptedDate { get; set; }
        public Guid? ProviderAcceptedUserId { get; set; }

        public DateTime? ProviderRejectedDate { get; set; }
        public Guid? ProviderRejectedUserId { get; set; }

        public DateTime? CompletedDate { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? NextRenewDate { get; set; }

        public DateTime? OnHoldDate { get; set; }
        public Guid? OnHoldByUserId { get; set; }
        public string? OnHoldReason { get; set; }

        public DateTime? CancelledDate { get; set; }
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledReason { get; set; }

        public Guid UpdatedByUserId { get; set; }
    }

    public class UpdateSuperCRMCommissionDto
    {
        public Guid SaleId { get; set; }
        public bool IsProviderCommissionReceived { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public List<UpdateSuperCRMCommissionLineDto> Lines { get; set; } = new();
    }

    public class UpdateSuperCRMCommissionLineDto
    {
        public Guid SaleLineId { get; set; }
        public decimal SuperCRMCommissionEarned { get; set; }
    }
}
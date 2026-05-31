namespace SuperCRM.Domain.Entities
{
    public class Sale
    {
        public Guid SaleId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? CustomerBusinessId { get; set; }
        public Guid? ProviderId { get; set; }
        public byte OrderSourceType { get; set; }
        public byte SaleChannelType { get; set; }
        public Guid? SoldByUserId { get; set; }
        public Guid? SoldByAgentId { get; set; }
        public string? SoldByAgentCode { get; set; }
        public bool IsCommissionApplicable { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public decimal ProviderCommissionEarned { get; set; }
        public decimal AgentCommissionAmount { get; set; }
        public bool IsProviderCommissionReceived { get; set; }
        public bool IsAgentCommissionDistributed { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public Customer? Customer { get; set; }
        public CustomerBusiness? CustomerBusiness { get; set; }
        public Provider? Provider { get; set; }
        public byte SalesOrderStatus { get; set; }

        public DateTime? SentToProviderDate { get; set; }
        public Guid? SentToProviderUserId { get; set; }

        public DateTime? ProviderAcceptedDate { get; set; }
        public Guid? ProviderAcceptedUserId { get; set; }

        public DateTime? ProviderRejectedDate { get; set; }
        public Guid? ProviderRejectedUserId { get; set; }

        public DateTime? DeliveredDate { get; set; }
        public Guid? DeliveredDateUpdatedBy { get; set; }

        public DateTime? CancelledDate { get; set; }
        public string? CancelledReason { get; set; }
        public Guid? CancelledByUserId { get; set; }

        public DateTime? OnHoldDate { get; set; }
        public string? OnHoldReason { get; set; }
        public Guid? OnHoldByUserId { get; set; }

        public string? SpecialNotes { get; set; }

        public bool EmailSentToCustomer { get; set; }
        public bool EmailSentToProvider { get; set; }

        public DateTime? CompletedDate { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? NextRenewDate { get; set; }

        public int NoOfRenew { get; set; }
        public string? RenewNotes { get; set; }

        public Guid? ManagerUserId { get; set; }
        public ICollection<SaleLine> SaleLines { get; set; } = new List<SaleLine>();
    }
}

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

        public Customer? Customer { get; set; }
        public CustomerBusiness? CustomerBusiness { get; set; }
        public Provider? Provider { get; set; }
        public ICollection<SaleLine> SaleLines { get; set; } = new List<SaleLine>();
    }
}

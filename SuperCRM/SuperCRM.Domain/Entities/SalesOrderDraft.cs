namespace SuperCRM.Domain.Entities
{
    public class SalesOrderDraft
    {
        public Guid SalesOrderDraftId { get; set; }
        public string DraftNo { get; set; } = string.Empty;
        public Guid? CustomerId { get; set; }
        public Guid? CustomerBusinessId { get; set; }
        public Guid? CustomerAddressId { get; set; }
        public Guid? CustomerBankAccountId { get; set; }
        public byte DraftStatus { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public ICollection<SalesOrderDraftLine> DraftLines { get; set; } = new List<SalesOrderDraftLine>();
    }
}

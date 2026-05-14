namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderDraftDto
    {
        public Guid SalesOrderDraftId { get; set; }
        public string DraftNo { get; set; } = string.Empty;
        public byte DraftStatus { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<SalesOrderDraftLineDto> Lines { get; set; } = new();
    }
}

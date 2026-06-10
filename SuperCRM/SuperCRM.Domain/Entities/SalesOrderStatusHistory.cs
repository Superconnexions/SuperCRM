namespace SuperCRM.Domain.Entities
{
    public class SalesOrderStatusHistory
    {
        public Guid SalesOrderStatusHistoryId { get; set; }
        public Guid SaleId { get; set; }
        public byte? OldStatus { get; set; }
        public byte NewStatus { get; set; }
        public string? Remarks { get; set; }
        public Guid ChangedByUserId { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
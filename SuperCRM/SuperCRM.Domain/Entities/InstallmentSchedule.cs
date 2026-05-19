namespace SuperCRM.Domain.Entities
{
    public class InstallmentSchedule
    {
        public Guid InstallmentScheduleId { get; set; }
        public Guid SaleLineId { get; set; }
        public Guid CustomerId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
        public int InstallmentNo { get; set; }
        public decimal InstallmentAmount { get; set; }
        public DateTime DueDate { get; set; }
        public byte PaymentStatus { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public Guid? CollectedByUserId { get; set; }
        public string? Remarks { get; set; }
        public string? PaymentNotes { get; set; }
        public DateTime CreatedAt { get; set; }

        public SaleLine? SaleLine { get; set; }
        public Customer? Customer { get; set; }
    }
}

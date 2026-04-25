using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.ProductBaseCommissions
{
    public class ProductBaseCommissionHistoryDto
    {
        public Guid HistoryId { get; set; }
        public Guid ProductBaseCommissionId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public CommissionType? OldCommissionType { get; set; }
        public decimal? OldFixedAmount { get; set; }
        public decimal? OldPercentage { get; set; }
        public CommissionType? NewCommissionType { get; set; }
        public decimal? NewFixedAmount { get; set; }
        public decimal? NewPercentage { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }
    }
}

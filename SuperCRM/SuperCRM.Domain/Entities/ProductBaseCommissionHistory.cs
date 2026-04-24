using SuperCRM.Domain.Enums;

namespace SuperCRM.Domain.Entities
{
    public class ProductBaseCommissionHistory
    {
        public Guid HistoryId { get; set; }
        public Guid ProductBaseCommissionId { get; set; }
        public Guid ProductId { get; set; }

        public CommissionType? OldCommissionType { get; set; }
        public decimal? OldFixedAmount { get; set; }
        public decimal? OldPercentage { get; set; }

        public CommissionType? NewCommissionType { get; set; }
        public decimal? NewFixedAmount { get; set; }
        public decimal? NewPercentage { get; set; }

        public DateTime ChangedAt { get; set; }
        public Guid ChangedByUserId { get; set; }
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }

        public ProductBaseCommission? ProductBaseCommission { get; set; }
        public Product? Product { get; set; }
        //public ApplicationUser? ChangedByUser { get; set; }
        //public ApplicationUser? CreatedByUser { get; set; }
    }
}

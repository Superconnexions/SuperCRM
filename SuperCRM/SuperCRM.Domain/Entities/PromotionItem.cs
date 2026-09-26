using SuperCRM.Domain.Enums;

namespace SuperCRM.Domain.Entities
{
    public class PromotionItem
    {
        public Guid PromotionItemId { get; set; }

        public Guid PromotionId { get; set; }

        public string PromotionCode { get; set; } = string.Empty;

        public Guid ProductBaseCommissionId { get; set; }

        public Guid ProductId { get; set; }

        public decimal StandardCommission { get; set; }

        public PromotionType PromotionType { get; set; }

        public decimal? PromotionPercentage { get; set; }

        public decimal PromotionAmount { get; set; }

        public decimal FinalCommissionAmount { get; set; }

        // Every newly-created promotion product is Active by default.
        // Admin can later uncheck Active in Edit and click Update.
        public bool IsActive { get; set; } = true;

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public PromotionSetup? PromotionSetup { get; set; }

        public ProductBaseCommission? ProductBaseCommission { get; set; }

        public Product? Product { get; set; }
    }
}

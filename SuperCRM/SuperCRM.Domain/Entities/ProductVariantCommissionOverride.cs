namespace SuperCRM.Domain.Entities
{
    public class ProductVariantCommissionOverride
    {
        public Guid ProductVariantCommissionOverrideId { get; set; }

        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;

        public Guid? ProductVariantId { get; set; }
        public string VariantCode { get; set; } = string.Empty;

        public decimal ExtraCommissionAmount { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}
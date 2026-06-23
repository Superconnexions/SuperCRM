namespace SuperCRM.Application.DTOs.ProductBaseCommissions
{
    public class ProductVariantCommissionOverrideDto
    {
        public Guid ProductVariantCommissionOverrideId { get; set; }

        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";

        public Guid ProductVariantId { get; set; }
        public string VariantCode { get; set; } = "";
        public string VariantName { get; set; } = "";

        public decimal ExtraCommissionAmount { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; }
        public string? Note { get; set; }
    }

    public class SaveProductVariantCommissionOverrideDto
    {
        public Guid ProductVariantCommissionOverrideId { get; set; }

        public Guid ProductId { get; set; }
        public Guid ProductVariantId { get; set; }

        public decimal ExtraCommissionAmount { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; }
        public string? Note { get; set; }

        public Guid CurrentUserId { get; set; }
    }
}
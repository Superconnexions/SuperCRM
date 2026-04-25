using SuperCRM.Domain.Enums;

namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Product variant master entity.
    /// Stores one selectable option row under a product, such as Size or Package.
    /// </summary>
    public class ProductVariant
    {
        public Guid ProductVariantId { get; set; }
        public Guid ProductId { get; set; }
        public string VariantCode { get; set; } = string.Empty;
        public string VariantTypeCode { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        //public byte DisplayStyle { get; set; }

        public DisplayStyle DisplayStyle { get; set; }

        public int DisplayOrder { get; set; }
        public decimal? BasePrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public Product? Product { get; set; }
        public ProductVariantType? VariantType { get; set; }
    }
}

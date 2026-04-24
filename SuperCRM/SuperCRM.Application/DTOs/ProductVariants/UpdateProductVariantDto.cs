using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.ProductVariants
{
    /// <summary>
    /// Update DTO for Product Variant setup.
    /// </summary>
    public class UpdateProductVariantDto
    {
        public Guid ProductVariantId { get; set; }
        public Guid ProductId { get; set; }
        public string VariantCode { get; set; } = string.Empty;
        public string VariantTypeCode { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        //public byte DisplayStyle { get; set; }

        public DisplayStyle DisplayStyle { get; set; }
        public decimal? BasePrice { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public int DisplayOrder { get; set; }
    }
}

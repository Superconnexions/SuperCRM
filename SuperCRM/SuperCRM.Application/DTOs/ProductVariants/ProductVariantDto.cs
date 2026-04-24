using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.ProductVariants
{
    /// <summary>
    /// Standard DTO for Product Variant list/detail use.
    /// </summary>
    public class ProductVariantDto
    {
        public Guid ProductVariantId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string VariantCode { get; set; } = string.Empty;
        public string VariantTypeCode { get; set; } = string.Empty;
        public string VariantTypeName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        //public byte DisplayStyle { get; set; }
        public DisplayStyle DisplayStyle { get; set; }
        public decimal? BasePrice { get; set; }

        public int DisplayOrder { get; set; }
    }
}

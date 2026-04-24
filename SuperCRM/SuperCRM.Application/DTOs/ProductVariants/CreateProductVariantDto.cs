using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.ProductVariants
{
    /// <summary>
    /// Create DTO for Product Variant setup.
    /// </summary>
    public class CreateProductVariantDto
    {
        public Guid ProductId { get; set; }
        public string VariantCode { get; set; } = string.Empty;
        public string VariantTypeCode { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        //public byte DisplayStyle { get; set; } = 1;
        public DisplayStyle DisplayStyle { get; set; } = DisplayStyle.Dropdown;
        public decimal? BasePrice { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public int DisplayOrder { get; set; }
    }
}

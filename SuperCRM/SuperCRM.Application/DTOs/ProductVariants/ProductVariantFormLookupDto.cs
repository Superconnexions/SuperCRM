namespace SuperCRM.Application.DTOs.ProductVariants
{
    /// <summary>
    /// Aggregated lookup DTO for Create/Edit Product Variant page.
    /// </summary>
    public class ProductVariantFormLookupDto
    {
        public List<ProductVariantLookupDto> ProductOptions { get; set; } = new();
        public List<ProductVariantLookupDto> VariantTypeOptions { get; set; } = new();
        public List<ProductVariantLookupDto> DisplayStyleOptions { get; set; } = new();
    }
}

namespace SuperCRM.Application.DTOs.Products
{
    /// <summary>
    /// All lookup collections required by Product Create/Edit pages.
    /// </summary>
    public class ProductFormLookupDto
    {
        public List<ProductLookupItemDto> Categories { get; set; } = new();
        public List<ProductLookupItemDto> SalesUnits { get; set; } = new();
    }
}

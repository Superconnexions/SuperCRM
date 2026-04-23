namespace SuperCRM.Application.DTOs.Products
{
    /// <summary>
    /// Lightweight lookup DTO for dropdown binding.
    /// </summary>
    public class ProductLookupItemDto
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}

namespace SuperCRM.Application.DTOs.Products
{
    /// <summary>
    /// Read/write DTO for product images after file upload has been converted to URL.
    /// </summary>
    public class ProductImageDto
    {
        public Guid? ProductImageId { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
    }
}

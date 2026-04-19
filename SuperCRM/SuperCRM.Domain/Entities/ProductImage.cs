namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Product image entity aligned with dbo.ProductImages.
    /// Stores one or more image URLs per product.
    /// </summary>
    public class ProductImage
    {
        public Guid ProductImageId { get; set; }
        public Guid ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product? Product { get; set; }
    }
}

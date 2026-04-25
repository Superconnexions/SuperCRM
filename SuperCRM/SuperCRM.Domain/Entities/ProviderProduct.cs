namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Provider-specific product mapping entity aligned with dbo.ProviderProducts.
    /// Stores selected provider, selected product, and denormalized product code/name snapshot.
    /// </summary>
    public class ProviderProduct
    {
        public Guid ProviderProductId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public Provider? Provider { get; set; }
        public Product? Product { get; set; }
    }
}

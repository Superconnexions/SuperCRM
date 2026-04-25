namespace SuperCRM.Application.DTOs.ProviderProducts
{
    public class ProviderProductDto
    {
        public Guid ProviderProductId { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
    }
}

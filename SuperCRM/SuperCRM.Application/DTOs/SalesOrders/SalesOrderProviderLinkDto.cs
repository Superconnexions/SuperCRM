
namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderProviderLinkDto
    {
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string? WebsiteUrl { get; set; }
    }
}

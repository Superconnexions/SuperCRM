namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderProviderOptionViewModel
    {
        public Guid ProviderProductId { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
    }
}

namespace SuperCRM.Web.ViewModels.ProviderProducts
{
    public class ProviderProductListItemViewModel
    {
        public Guid ProviderProductId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
    }
}

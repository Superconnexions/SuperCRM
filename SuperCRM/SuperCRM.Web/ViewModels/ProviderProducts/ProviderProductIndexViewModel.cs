namespace SuperCRM.Web.ViewModels.ProviderProducts
{
    public class ProviderProductIndexViewModel
    {
        public string? SearchProviderName { get; set; }
        public string? SearchProductName { get; set; }
        public List<ProviderProductListItemViewModel> Items { get; set; } = new();
    }
}

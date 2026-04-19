namespace SuperCRM.Web.ViewModels.Products
{
    /// <summary>
    /// Lightweight ViewModel for Product list page.
    /// </summary>
    public class ProductListItemViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductDisplayName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SalesUnitCode { get; set; } = string.Empty;
        public string BasePriceType { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public int ImageCount { get; set; }
        public bool IsActive { get; set; }
    }
}

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderProductVariantOptionViewModel
    {
        public Guid ProductVariantId { get; set; }
        public string VariantCode { get; set; } = string.Empty;
        public string VariantTypeCode { get; set; } = string.Empty;
        public string VariantTypeName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public byte DisplayStyle { get; set; }
        public string DisplayStyleName => DisplayStyle == 1 ? "Dropdown" : DisplayStyle == 2 ? "Checkbox" : string.Empty;
        public decimal? BasePrice { get; set; }
        public int? DisplayOrder { get; set; }
    }
}

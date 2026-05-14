namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderProductVariantViewModel
    {
        public Guid ProductVariantId { get; set; }
        public string VariantCode { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int DisplayOrder { get; set; }
    }
}

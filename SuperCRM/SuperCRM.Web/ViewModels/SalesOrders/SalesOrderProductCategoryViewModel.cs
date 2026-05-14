namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderProductCategoryViewModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public List<SalesOrderProductItemViewModel> Products { get; set; } = new();
        public string HeaderImageUrl => !string.IsNullOrWhiteSpace(CategoryImageUrl) ? CategoryImageUrl : string.Empty;
    }
}

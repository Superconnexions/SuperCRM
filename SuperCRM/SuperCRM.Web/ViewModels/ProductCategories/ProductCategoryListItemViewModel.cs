namespace SuperCRM.Web.ViewModels.ProductCategories
{
    /// <summary>
    /// Lightweight ViewModel for Product Category list page.
    /// </summary>
    public class ProductCategoryListItemViewModel
    {
        public Guid CategoryId { get; set; }
        public string? CategoryCode { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}

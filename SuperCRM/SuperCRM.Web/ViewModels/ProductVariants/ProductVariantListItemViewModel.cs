using SuperCRM.Domain.Enums;

namespace SuperCRM.Web.ViewModels.ProductVariants
{
    /// <summary>
    /// List page view model for Product Variant.
    /// </summary>
    public class ProductVariantListItemViewModel
    {
        public Guid ProductVariantId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string VariantCode { get; set; } = string.Empty;
        public string VariantTypeName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        //public byte DisplayStyle { get; set; }
        public DisplayStyle DisplayStyle { get; set; }

        public string DisplayStyleName => DisplayStyle.ToString();
        public int DisplayOrder { get; set; }

        public decimal? BasePrice { get; set; }

    }
}

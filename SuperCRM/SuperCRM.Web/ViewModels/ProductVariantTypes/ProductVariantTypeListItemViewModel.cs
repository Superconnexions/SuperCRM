namespace SuperCRM.Web.ViewModels.ProductVariantTypes
{
    public class ProductVariantTypeListItemViewModel
    {
        public string TypeCode { get; set; } = string.Empty;
        public string TypeValue { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
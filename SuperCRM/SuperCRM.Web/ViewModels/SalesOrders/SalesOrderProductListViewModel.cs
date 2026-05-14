namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderProductListViewModel
    {
        public Guid? SalesOrderDraftId { get; set; }
        public string? DraftNo { get; set; }
        public List<SalesOrderProductCategoryViewModel> BusinessCategories { get; set; } = new();
        public List<SalesOrderProductCategoryViewModel> ResidentialCategories { get; set; } = new();

        public List<SalesOrderDraftLineViewModel> DraftLines { get; set; } = new();
    }
}

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderProductSelectionPostViewModel
    {
        public Guid? SalesOrderDraftId { get; set; }
        public List<SalesOrderProductSelectionLinePostViewModel> Lines { get; set; } = new();
    }

    public class SalesOrderProductSelectionLinePostViewModel
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public Guid? ProviderProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal SalePrice { get; set; }
        public DateTime? FirstInstallmentDate { get; set; }
        public bool IsInstallmentSelected { get; set; }

        public bool InstallmentApplicable { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public int? NoOfInstallment { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
    }
}

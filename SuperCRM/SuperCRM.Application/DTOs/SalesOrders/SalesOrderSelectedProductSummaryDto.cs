namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderSelectedProductSummaryDto
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public Guid? ProductVariantId { get; set; }
        public string? VariantName { get; set; }
        public Guid? ProviderId { get; set; }
        public string ProviderName { get; set; } = "SuperCRM";
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal LineTotalAmount { get; set; }
        public string? CurrencyCode { get; set; }
        public bool InstallmentApplicable { get; set; }
        public bool IsInstallmentSelected { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public int? NoOfInstallment { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
        public DateTime? FirstInstallmentDate { get; set; }
        public string? SalesUnitCode { get; set; }
    }
}

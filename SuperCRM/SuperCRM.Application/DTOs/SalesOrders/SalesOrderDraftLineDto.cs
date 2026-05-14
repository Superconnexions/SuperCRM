namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderDraftLineDto
    {
        public Guid SalesOrderDraftLineId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public Guid? ProductVariantId { get; set; }
        public string? VariantCode { get; set; }
        public string? VariantName { get; set; }
        public Guid? ProviderProductId { get; set; }
        public Guid? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public int Quantity { get; set; }
        public byte BasePriceType { get; set; }
        public decimal BasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal LineTotalAmount { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime? FirstInstallmentDate { get; set; }
        public bool IsInstallmentSelected { get; set; }

        public bool InstallmentApplicable { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public int? NoOfInstallment { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
    }
}

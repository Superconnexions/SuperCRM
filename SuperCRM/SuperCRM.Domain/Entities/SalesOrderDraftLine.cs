namespace SuperCRM.Domain.Entities
{
    public class SalesOrderDraftLine
    {
        public Guid SalesOrderDraftLineId { get; set; }
        public Guid SalesOrderDraftId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
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
        public bool IsPriceEditable { get; set; }
        public bool InstallmentApplicable { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public int? NoOfInstallment { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
        public DateTime? FirstInstallmentDate { get; set; }

        public bool IsInstallmentSelected { get; set; }
        public DateTime CreatedAt { get; set; }

        public SalesOrderDraft? SalesOrderDraft { get; set; }
        public Product? Product { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public ProviderProduct? ProviderProduct { get; set; }
        public Provider? Provider { get; set; }
    }
}

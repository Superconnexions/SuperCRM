namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderProductItemDto
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductDisplayName { get; set; } = string.Empty;
        public byte ProductType { get; set; }
        public string ProductTypeName => ProductType == 1 ? "SimpleProduct" : ProductType == 2 ? "VariantProduct" : string.Empty;
        public byte? CustomerType { get; set; }
        public byte BasePriceType { get; set; }
        public string BasePriceTypeName => BasePriceType == 1 ? "SimplePrice" : BasePriceType == 2 ? "OpenPrice" : BasePriceType == 3 ? "VariantPrice" : string.Empty;
        public decimal BasePrice { get; set; }
        public bool IsPriceEditable { get; set; }
        public bool InstallmentApplicable { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public int? NoOfInstallment { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductDisplayNotes { get; set; } = string.Empty;
        public string PaymentNotes { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string PrimaryImageUrl { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;

        public List<SalesOrderProviderOptionDto> Providers { get; set; } = new();
        public List<SalesOrderProductVariantOptionDto> Variants { get; set; } = new();
    }
}

using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.Products
{
    /// <summary>
    /// Read DTO for Product list and edit load.
    /// </summary>
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int SalesUnitId { get; set; }
        public string SalesUnitCode { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductDisplayName { get; set; } = string.Empty;
        public ProductType ProductType { get; set; }
        public ProductCustomerType? CustomerType { get; set; }
        public ProductBasePriceType BasePriceType { get; set; }
        public decimal BasePrice { get; set; }
        public string CurrencyCode { get; set; } = "GBP";
        public bool IsActive { get; set; }
        public bool IsPortalVisible { get; set; }
        public bool IsPortalOrderEnabled { get; set; }
        public int DisplayOrder { get; set; }
        public string? ProductDescription { get; set; }
        public bool IsThirdPartyProduct { get; set; }
        public bool InstallmentApplicable { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public bool IsRequiredBankInformation { get; set; }
        public bool IsProviderDeliveryProduct { get; set; }
        public bool IsPriceEditable { get; set; }
        public string? ProductDisplayNotes { get; set; }
        public string? PaymentNotes { get; set; }
        public string? Remarks { get; set; }

        public decimal? MonthlyInstallmentAmount { get; set; }
        public int NoOfInstallment { get; set; }

        public List<ProductImageDto> Images { get; set; } = new();
    }
}

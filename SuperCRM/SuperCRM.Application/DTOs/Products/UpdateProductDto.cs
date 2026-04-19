using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.Products
{
    /// <summary>
    /// Update DTO for product setup.
    /// </summary>
    public class UpdateProductDto
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public int SalesUnitId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductDisplayName { get; set; } = string.Empty;
        public ProductType ProductType { get; set; }
        public ProductCustomerType? CustomerType { get; set; }
        public string? ProductDescription { get; set; }
        public bool IsThirdPartyProduct { get; set; }
        public bool InstallmentApplicable { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public string CurrencyCode { get; set; } = "GBP";
        public bool IsRequiredBankInformation { get; set; }
        public bool IsProviderDeliveryProduct { get; set; }
        public ProductBasePriceType BasePriceType { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsPriceEditable { get; set; }
        public bool IsPortalVisible { get; set; }
        public bool IsPortalOrderEnabled { get; set; }
        public int DisplayOrder { get; set; }
        public string? ProductDisplayNotes { get; set; }
        public string? PaymentNotes { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
    }
}

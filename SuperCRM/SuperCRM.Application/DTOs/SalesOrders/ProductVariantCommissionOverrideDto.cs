namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class ProductVariantCommissionOverrideDto
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public Guid? ProductVariantId { get; set; }
        public string VariantCode { get; set; } = string.Empty;
        public decimal ExtraCommissionAmount { get; set; }
    }
}
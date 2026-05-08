namespace SuperCRM.Web.ViewModels.ProductBaseCommissions
{
    public class ProductBaseCommissionDeleteViewModel
    {
        public Guid ProductBaseCommissionId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CommissionTypeText { get; set; } = string.Empty;
        public decimal? FixedAmount { get; set; }
        public decimal? Percentage { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Note { get; set; }
    }
}

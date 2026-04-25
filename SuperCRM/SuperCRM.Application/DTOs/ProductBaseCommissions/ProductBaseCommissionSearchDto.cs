namespace SuperCRM.Application.DTOs.ProductBaseCommissions
{
    public class ProductBaseCommissionSearchDto
    {
        public string? ProductKeyword { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IncludeInactive { get; set; }
    }
}

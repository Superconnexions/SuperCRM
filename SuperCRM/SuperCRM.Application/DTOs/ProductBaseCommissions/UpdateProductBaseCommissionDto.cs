using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.ProductBaseCommissions
{
    public class UpdateProductBaseCommissionDto
    {
        public Guid ProductBaseCommissionId { get; set; }
        public Guid ProductId { get; set; }
        public CommissionType CommissionType { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? Percentage { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public string? Note { get; set; }
    }
}

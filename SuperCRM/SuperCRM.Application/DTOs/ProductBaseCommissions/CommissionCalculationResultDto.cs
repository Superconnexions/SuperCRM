using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.ProductBaseCommissions
{
    public class CommissionCalculationResultDto
    {
        public bool Found { get; set; }
        public Guid? ProductBaseCommissionId { get; set; }
        public CommissionType? CommissionType { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? Percentage { get; set; }
        public decimal CommissionAmount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.PromotionSetup
{
    public class PromotionProductLookupDto
    {
        public Guid ProductId { get; set; }
        public Guid ProductBaseCommissionId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal StandardCommission { get; set; }
    }

    public class PromotionItemInputDto
    {
        public Guid? PromotionItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductBaseCommissionId { get; set; }

        public PromotionType PromotionType { get; set; }

        public decimal? PromotionPercentage { get; set; }

        public decimal? PromotionAmount { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class SubmitPromotionDto
    {
        public string PromotionName { get; set; } = string.Empty;

        public string? PromotionSummary { get; set; }

        public string? NotificationMessage { get; set; }

        public DateTime PromotionStartDate { get; set; }

        public DateTime PromotionEndDate { get; set; }

        public string? Remarks { get; set; }

        public Guid UserId { get; set; }

        public List<PromotionItemInputDto> Items { get; set; } = new();
    }

    public class UpdatePromotionDto : SubmitPromotionDto
    {
        public Guid PromotionId { get; set; }

        // Promotion-level Active status is editable only after creation.
        public bool IsActive { get; set; } = true;
    }

    public class PromotionItemDto
    {
        public Guid PromotionItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductBaseCommissionId { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal StandardCommission { get; set; }

        public PromotionType PromotionType { get; set; }

        public decimal? PromotionPercentage { get; set; }

        public decimal PromotionAmount { get; set; }

        public decimal FinalCommissionAmount { get; set; }

        public bool IsActive { get; set; }
    }

    public class PromotionDto
    {
        public Guid PromotionId { get; set; }

        public string PromotionCode { get; set; } = string.Empty;

        public string PromotionName { get; set; } = string.Empty;

        public string? PromotionSummary { get; set; }

        public string? NotificationMessage { get; set; }

        public DateTime PromotionStartDate { get; set; }

        public DateTime PromotionEndDate { get; set; }

        public bool Published { get; set; }

        public bool Cancelled { get; set; }

        public DateTime? CancelledAt { get; set; }

        public Guid? CancelledByUserId { get; set; }

        public bool IsActive { get; set; }

        public bool Submitted { get; set; }

        public bool EmailNotificationToAgents { get; set; }

        public byte NoOfEmailNotificationSend { get; set; }

        public string? Remarks { get; set; }

        public List<PromotionItemDto> Items { get; set; } = new();
    }

    public class PromotionOperationResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public Guid? PromotionId { get; set; }
    }

    public class ApplicablePromotionCommissionDto
    {
        public Guid PromotionId { get; set; }

        public string PromotionCode { get; set; }
            = string.Empty;

        public string PromotionName { get; set; }
            = string.Empty;

        public Guid PromotionItemId { get; set; }

        public Guid ProductId { get; set; }

        public decimal StandardCommission { get; set; }

        public decimal PromotionAmount { get; set; }

        public decimal FinalCommissionAmount { get; set; }
    }
}

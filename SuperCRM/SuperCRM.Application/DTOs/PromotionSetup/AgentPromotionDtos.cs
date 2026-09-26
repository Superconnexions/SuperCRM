namespace SuperCRM.Application.DTOs.PromotionSetup
{
    /// <summary>
    /// Header notification summary for the logged-in Agent.
    /// ActivePromotionCount counts active PromotionSetup records that contain
    /// at least one active PromotionItem.
    /// </summary>
    public class AgentPromotionNotificationDto
    {
        public int ActivePromotionCount { get; set; }

        public int NewPromotionCount { get; set; }

        public bool HasNewPromotion => NewPromotionCount > 0;
    }

    public class AgentActivePromotionDto
    {
        public Guid PromotionId { get; set; }

        public string PromotionCode { get; set; } = string.Empty;

        public string PromotionName { get; set; } = string.Empty;

        public string? PromotionSummary { get; set; }

        public DateTime PromotionStartDate { get; set; }

        public DateTime PromotionEndDate { get; set; }

        public int ActiveItemCount { get; set; }

        public bool IsNew { get; set; }
    }

    public class AgentPromotionItemDto
    {
        public Guid PromotionItemId { get; set; }

        public string PromotionCode { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal StandardCommission { get; set; }

        public decimal PromotionAmount { get; set; }

        public decimal FinalCommissionAmount { get; set; }
    }

    public class AgentPromotionDetailsDto
    {
        public Guid PromotionId { get; set; }

        public string PromotionCode { get; set; } = string.Empty;

        public string PromotionName { get; set; } = string.Empty;

        public List<AgentPromotionItemDto> Items { get; set; } = new();
    }
}

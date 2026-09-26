namespace SuperCRM.Domain.Entities
{
    public class PromotionSetup
    {
        public Guid PromotionId { get; set; }

        public string PromotionCode { get; set; } = string.Empty;

        public string PromotionName { get; set; } = string.Empty;

        public string? PromotionSummary { get; set; }

        public bool EmailNotificationToAgents { get; set; }

        public byte NoOfEmailNotificationSend { get; set; }

        public string? NotificationMessage { get; set; }

        public DateTime PromotionStartDate { get; set; }

        public DateTime PromotionEndDate { get; set; }

        public bool Published { get; set; }

        public bool Cancelled { get; set; }

        public DateTime? CancelledAt { get; set; }

        public Guid? CancelledByUserId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool Submitted { get; set; }

        public DateTime SubmittedAt { get; set; }

        public Guid SubmittedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public DateTime? LastEmailNotificationAt { get; set; }

        public Guid? LastEmailNotificationByUserId { get; set; }

        public string? Remarks { get; set; }

        public ICollection<PromotionItem> Items { get; set; }
            = new List<PromotionItem>();
    }
}

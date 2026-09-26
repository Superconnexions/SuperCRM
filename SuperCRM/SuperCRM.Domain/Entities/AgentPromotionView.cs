namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Tracks whether a specific Agent/Identity user has viewed a promotion.
    /// A row is created only when the Agent opens the promotion items.
    /// </summary>
    public class AgentPromotionView
    {
        public Guid AgentPromotionViewId { get; set; }

        public Guid PromotionId { get; set; }

        public Guid AgentUserId { get; set; }

        public DateTime ViewedAt { get; set; }

        public PromotionSetup? PromotionSetup { get; set; }
    }
}

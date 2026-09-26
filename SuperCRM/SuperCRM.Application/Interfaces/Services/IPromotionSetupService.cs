using SuperCRM.Application.DTOs.PromotionSetup;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IPromotionSetupService
    {
        Task<PromotionListResultDto> SearchAsync(
            PromotionListFilterDto filter,
            CancellationToken ct = default);

        Task<List<PromotionProductLookupDto>> GetProductOptionsAsync(
            DateTime asOfDate,
            CancellationToken ct = default);

        Task<PromotionDto?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task<PromotionOperationResultDto> SubmitAsync(
            SubmitPromotionDto request,
            CancellationToken ct = default);

        Task<PromotionOperationResultDto> UpdateAsync(
            UpdatePromotionDto request,
            CancellationToken ct = default);

        Task<PromotionOperationResultDto> PublishAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default);

        Task<PromotionOperationResultDto> PublishWithEmailAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default);

        Task<PromotionOperationResultDto> ResendEmailAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default);

        Task<PromotionOperationResultDto> CancelAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default);

        Task<PromotionOperationResultDto> SendTestEmailAsync(
            SendPromotionTestEmailDto request,
            CancellationToken ct = default);

        // -----------------------------------------------------
        // AGENT ACTIVE PROMOTIONS / NOTIFICATION
        // -----------------------------------------------------
        Task<List<AgentActivePromotionDto>> GetAgentActivePromotionsAsync(
            Guid agentUserId,
            DateTime currentDate,
            CancellationToken ct = default);

        Task<AgentPromotionNotificationDto> GetAgentPromotionNotificationAsync(
            Guid agentUserId,
            DateTime currentDate,
            CancellationToken ct = default);

        Task<AgentPromotionDetailsDto?> GetAgentPromotionItemsAsync(
            Guid promotionId,
            DateTime currentDate,
            CancellationToken ct = default);

        Task MarkAgentPromotionViewedAsync(
            Guid promotionId,
            Guid agentUserId,
            DateTime viewedAt,
            CancellationToken ct = default);

    }
}

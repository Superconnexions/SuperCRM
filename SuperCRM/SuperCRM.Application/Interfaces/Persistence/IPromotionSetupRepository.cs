using SuperCRM.Application.DTOs.PromotionSetup;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IPromotionSetupRepository
    {
        Task<PromotionListResultDto> SearchAsync(
            PromotionListFilterDto filter,
            CancellationToken ct = default);

        Task<PromotionSetup?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task<List<PromotionProductLookupDto>> GetProductCommissionOptionsAsync(
            DateTime asOfDate,
            CancellationToken ct = default);

        Task<ProductBaseCommission?> GetProductBaseCommissionAsync(
            Guid id,
            CancellationToken ct = default);

        Task<int> GetNextMonthlySequenceAsync(
            int year,
            int month,
            CancellationToken ct = default);

        Task<bool> PromotionCodeExistsAsync(
            string code,
            CancellationToken ct = default);

        Task<List<string>> GetActiveAgentEmailsAsync(
            CancellationToken ct = default);

        Task AddAsync(
            PromotionSetup entity,
            CancellationToken ct = default);

        void RemoveItem(PromotionItem item);

        Task SaveChangesAsync(
            CancellationToken ct = default);

        Task<List<PromotionProductConflictDto>>
        GetActivePromotionProductConflictsAsync(
        Guid currentPromotionId,
        List<Guid> productIds,
        DateTime today,
        CancellationToken ct = default);


        Task<List<ApplicablePromotionCommissionDto>>
        GetApplicablePromotionCommissionsAsync(
        List<Guid> productIds,
        DateTime saleDate,
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

        // END
    }
}

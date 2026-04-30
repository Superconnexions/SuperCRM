using SuperCRM.Application.DTOs.AgentManagement;
using SuperCRM.Domain.Entities;


namespace SuperCRM.Application.Interfaces.Persistence
{
    /// <summary>
    /// Persistence contract for SuperAdmin/SuperCRMAdmin Agent management.
    /// </summary>
    public interface IAgentManagementRepository
    {
        Task<List<AgentListItemDto>> SearchAsync(AgentSearchDto search, CancellationToken cancellationToken = default);
        Task<AgentRegistrationDetailsDto?> GetDetailsAsync(Guid agentId, CancellationToken cancellationToken = default);
        Task<Agent?> GetAgentByIdAsync(Guid agentId, CancellationToken cancellationToken = default);
        Task<UserProfile?> GetUserProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        //Task<ApplicationUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

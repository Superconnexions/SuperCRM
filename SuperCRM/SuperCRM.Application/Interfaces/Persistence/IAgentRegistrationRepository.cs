using Microsoft.EntityFrameworkCore.Storage;
using SuperCRM.Application.DTOs.Agents;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    /// <summary>
    /// Persistence contract for the Agent registration workflow.
    /// Database interaction stays behind this abstraction so controllers never call DbContext directly.
    /// </summary>
    public interface IAgentRegistrationRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task<string> GenerateNextAgentCodeAsync(CancellationToken cancellationToken = default);
        Task AddUserProfileAsync(UserProfile entity, CancellationToken cancellationToken = default);
        Task AddUserAddressAsync(UserAddress entity, CancellationToken cancellationToken = default);
        Task AddAgentAsync(Agent entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<AgentDashboardDto?> GetAgentDashboardByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}

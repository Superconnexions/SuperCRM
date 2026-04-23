using SuperCRM.Application.DTOs.AgentRegistration;
using SuperCRM.Application.DTOs.Agents;

namespace SuperCRM.Application.Interfaces.Services
{
    /// <summary>
    /// Application service contract for public Agent registration and Agent dashboard retrieval.
    /// </summary>
    public interface IAgentRegistrationService
    {
        /// <summary>
        /// Registers a new agent through the public registration flow.
        /// Creates Identity user, UserProfile, UserAddress, and Agent records.
        /// </summary>
        Task<AgentRegistrationResultDto> RegisterAsync(
            AgentRegistrationRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the Agent dashboard read model for the logged-in user.
        /// </summary>
        Task<AgentDashboardDto?> GetDashboardAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
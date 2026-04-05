using SuperCRM.Application.DTOs.Agents;

namespace SuperCRM.Application.Interfaces.Services
{
    /// <summary>
    /// Application service contract for public Agent registration and Agent dashboard retrieval.
    /// </summary>
    public interface IAgentRegistrationService
    {
        Task<AgentRegistrationResultDto> RegisterAsync(AgentRegistrationRequestDto request, CancellationToken cancellationToken = default);
        Task<AgentDashboardDto?> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}

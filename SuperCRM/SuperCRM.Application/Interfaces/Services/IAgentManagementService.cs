using SuperCRM.Application.DTOs.AgentManagement;

namespace SuperCRM.Application.Interfaces.Services
{
    /// <summary>
    /// Application service contract for Agent approval and registration status management.
    /// </summary>
    public interface IAgentManagementService
    {
        Task<List<AgentListItemDto>> SearchAsync(AgentSearchDto search, CancellationToken cancellationToken = default);
        Task<AgentRegistrationDetailsDto?> GetDetailsAsync(Guid agentId, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateRegistrationAsync(UpdateAgentRegistrationDto request, CancellationToken cancellationToken = default);
    }
}

using SuperCRM.Application.DTOs.AgentManagement;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    /// <summary>
    /// SuperAdmin/SuperCRMAdmin service for Agent registration review and approval.
    /// </summary>
    public class AgentManagementService : IAgentManagementService
    {
        private readonly IAgentManagementRepository _repository;

        public AgentManagementService(IAgentManagementRepository repository)
        {
            _repository = repository;
        }

        public Task<List<AgentListItemDto>> SearchAsync(AgentSearchDto search, CancellationToken cancellationToken = default)
        {
            return _repository.SearchAsync(search, cancellationToken);
        }

        public Task<AgentRegistrationDetailsDto?> GetDetailsAsync(Guid agentId, CancellationToken cancellationToken = default)
        {
            return _repository.GetDetailsAsync(agentId, cancellationToken);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateRegistrationAsync(UpdateAgentRegistrationDto request, CancellationToken cancellationToken = default)
        {
            if (request.AgentId == Guid.Empty)
                return (false, "Invalid Agent Id.");

            if (request.UpdatedByUserId == Guid.Empty)
                return (false, "Updated by user is required.");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                return (false, "First Name is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                return (false, "Last Name is required.");

            if (!Enum.IsDefined(typeof(AgentRegistrationStatus), request.RegistrationStatus))
                return (false, "Invalid registration status.");

            var agent = await _repository.GetAgentByIdAsync(request.AgentId, cancellationToken);
            if (agent == null)
                return (false, "Agent not found.");

            var profile = await _repository.GetUserProfileByUserIdAsync(agent.UserId, cancellationToken);
            if (profile == null)
                return (false, "Agent profile not found.");

            var utcNow = DateTime.UtcNow;

            profile.FirstName = request.FirstName.Trim();
            profile.LastName = request.LastName.Trim();
            profile.PhoneNo = CleanOptional(request.PhoneNo);
            profile.MobileNo = CleanOptional(request.MobileNo);
            profile.UpdatedAt = utcNow;
            profile.UpdatedByUserId = request.UpdatedByUserId;

            agent.RegistrationStatus = request.RegistrationStatus;
            agent.IsCommissionEligible = request.IsCommissionEligible;
            agent.UpdatedAt = utcNow;
            agent.UpdatedByUserId = request.UpdatedByUserId;

            if (request.RegistrationStatus == (byte)AgentRegistrationStatus.Active)
            {
                agent.IsApproved = true;
                agent.ApprovedByUserId = request.UpdatedByUserId;
                agent.ApprovedAt = utcNow;

                if (!agent.JoinedAt.HasValue)
                    agent.JoinedAt = utcNow;
            }

            await _repository.SaveChangesAsync(cancellationToken);
            return (true, string.Empty);
        }

        private static string? CleanOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}

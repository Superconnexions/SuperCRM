using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SuperCRM.Application.DTOs.Agents;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Agent registration workflow.
    /// Encapsulates all database access used by the Agent registration service.
    /// </summary>
    public class AgentRegistrationRepository : IAgentRegistrationRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public AgentRegistrationRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<string> GenerateNextAgentCodeAsync(CancellationToken cancellationToken = default)
        {
            var lastNumericPart = await _dbContext.Agents
                .AsNoTracking()
                .Where(x => x.AgentCode.StartsWith("AG"))
                .Select(x => x.AgentCode.Substring(2))
                .ToListAsync(cancellationToken);

            var maxNumber = lastNumericPart
                .Select(x => int.TryParse(x, out var number) ? number : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"AG{(maxNumber + 1):D6}";
        }

        public async Task AddUserProfileAsync(UserProfile entity, CancellationToken cancellationToken = default)
            => await _dbContext.UserProfiles.AddAsync(entity, cancellationToken);

        public async Task AddUserAddressAsync(UserAddress entity, CancellationToken cancellationToken = default)
            => await _dbContext.UserAddresses.AddAsync(entity, cancellationToken);

        public async Task AddAgentAsync(Agent entity, CancellationToken cancellationToken = default)
            => await _dbContext.Agents.AddAsync(entity, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _dbContext.SaveChangesAsync(cancellationToken);

        public async Task<AgentDashboardDto?> GetAgentDashboardByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Agents
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new AgentDashboardDto
                {
                    AgentId = x.AgentId,
                    UserId = x.UserId,
                    AgentCode = x.AgentCode,
                    IsApproved = x.IsApproved,
                    IsCommissionEligible = x.IsCommissionEligible,
                    RegistrationStatus = x.RegistrationStatus,
                    RegistrationStatusText =
                        x.RegistrationStatus == (byte)AgentRegistrationStatus.PendingApproval ? "Pending Approval" :
                        x.RegistrationStatus == (byte)AgentRegistrationStatus.Active ? "Active" :
                        x.RegistrationStatus == (byte)AgentRegistrationStatus.Rejected ? "Rejected" :
                        x.RegistrationStatus == (byte)AgentRegistrationStatus.Suspended ? "Suspended" :
                        "Unknown",
                    JoinedAt = x.JoinedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}

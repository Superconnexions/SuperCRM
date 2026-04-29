using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.AgentManagement;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;
using SuperCRM.Persistence.DbContexts;


namespace SuperCRM.Persistence.Repositories
{
    /// <summary>
    /// EF Core repository for Agent management search and registration approval workflow.
    /// </summary>
    public class AgentManagementRepository : IAgentManagementRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public AgentManagementRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AgentListItemDto>> SearchAsync(AgentSearchDto search, CancellationToken cancellationToken = default)
        {
            var query =
                from a in _dbContext.Agents.AsNoTracking()
                join u in _dbContext.Users.AsNoTracking() on a.UserId equals u.Id
                join p in _dbContext.UserProfiles.AsNoTracking() on u.Id equals p.UserId into profileJoin
                from p in profileJoin.DefaultIfEmpty()
                select new
                {
                    Agent = a,
                    User = u,
                    Profile = p
                };

            if (search.IsApproved.HasValue)
            {
                query = query.Where(x => x.Agent.IsApproved == search.IsApproved.Value);
            }

            if (search.RegistrationStatus.HasValue)
            {
                query = query.Where(x => x.Agent.RegistrationStatus == search.RegistrationStatus.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.SearchText))
            {
                var keyword = search.SearchText.Trim();

                query = query.Where(x =>
                    x.Agent.AgentCode.Contains(keyword) ||
                    (x.User.Email != null && x.User.Email.Contains(keyword)) ||
                    (x.Profile != null && x.Profile.FirstName.Contains(keyword)) ||
                    (x.Profile != null && x.Profile.LastName.Contains(keyword)) ||
                    (x.Profile != null && x.Profile.PhoneNo != null && x.Profile.PhoneNo.Contains(keyword)) ||
                    (x.Profile != null && x.Profile.MobileNo != null && x.Profile.MobileNo.Contains(keyword)));
            }

            return await query
                .OrderBy(x => x.Agent.RegistrationStatus)
                .ThenBy(x => x.Agent.AgentCode)
                .Select(x => new AgentListItemDto
                {
                    AgentId = x.Agent.AgentId,
                    UserId = x.Agent.UserId,
                    AgentCode = x.Agent.AgentCode,
                    Email = x.User.Email ?? string.Empty,
                    FirstName = x.Profile != null ? x.Profile.FirstName : string.Empty,
                    LastName = x.Profile != null ? x.Profile.LastName : string.Empty,
                    PhoneNo = x.Profile != null ? x.Profile.PhoneNo : null,
                    MobileNo = x.Profile != null ? x.Profile.MobileNo : null,
                    IsApproved = x.Agent.IsApproved,
                    RegistrationStatus = x.Agent.RegistrationStatus,
                    RegistrationStatusName = ((AgentRegistrationStatus)x.Agent.RegistrationStatus).ToString()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<AgentRegistrationDetailsDto?> GetDetailsAsync(Guid agentId, CancellationToken cancellationToken = default)
        {
            var result = await (
                from a in _dbContext.Agents.AsNoTracking()
                join u in _dbContext.Users.AsNoTracking() on a.UserId equals u.Id
                join p in _dbContext.UserProfiles.AsNoTracking() on u.Id equals p.UserId into profileJoin
                from p in profileJoin.DefaultIfEmpty()
                where a.AgentId == agentId
                select new AgentRegistrationDetailsDto
                {
                    AgentId = a.AgentId,
                    UserId = a.UserId,
                    AgentCode = a.AgentCode,
                    Email = u.Email ?? string.Empty,
                    FirstName = p != null ? p.FirstName : string.Empty,
                    LastName = p != null ? p.LastName : string.Empty,
                    PhoneNo = p != null ? p.PhoneNo : null,
                    MobileNo = p != null ? p.MobileNo : null,
                    IsApproved = a.IsApproved,
                    IsCommissionEligible = a.IsCommissionEligible,
                    RegistrationStatus = a.RegistrationStatus,
                    ApprovedByUserId = a.ApprovedByUserId,
                    ApprovedAt = a.ApprovedAt,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        public Task<Agent?> GetAgentByIdAsync(Guid agentId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Agents.FirstOrDefaultAsync(x => x.AgentId == agentId, cancellationToken);
        }

        public Task<UserProfile?> GetUserProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return _dbContext.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        //public Task<ApplicationUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        //{
        //    return _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        //}

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

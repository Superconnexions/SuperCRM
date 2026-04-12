using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SuperCRM.Application.DTOs.Agents;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    /// <summary>
    /// EF Core repository for the Agent registration workflow.
    /// Handles database persistence and geo master validation.
    /// </summary>
    public class AgentRegistrationRepository : IAgentRegistrationRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public AgentRegistrationRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Starts a transaction so Identity-linked registration data stays consistent.
        /// </summary>
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Generates next Agent code in AGT-00001 style.
        /// This is simple and acceptable for current phase.
        /// </summary>
        public async Task<string> GenerateNextAgentCodeAsync(CancellationToken cancellationToken = default)
        {
            var count = await _dbContext.Agents.CountAsync(cancellationToken);
            return $"AGT-{(count + 1):D5}";
        }

        /// <summary>
        /// Adds UserProfile entity to DbContext.
        /// SaveChanges is called separately by service orchestration.
        /// </summary>
        public async Task AddUserProfileAsync(UserProfile entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.UserProfiles.AddAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Adds UserAddress entity to DbContext.
        /// SaveChanges is called separately by service orchestration.
        /// </summary>
        public async Task AddUserAddressAsync(UserAddress entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.UserAddresses.AddAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Adds Agent entity to DbContext.
        /// SaveChanges is called separately by service orchestration.
        /// </summary>
        public async Task AddAgentAsync(Agent entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Agents.AddAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Saves all pending registration changes.
        /// </summary>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if selected country exists and is valid for use.
        /// </summary>
        public async Task<bool> CountryExistsAsync(int countryId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Countries
                .AsNoTracking()
                .AnyAsync(x => x.CountryId == countryId, cancellationToken);
        }

        /// <summary>
        /// Validates that selected Region belongs to selected Country.
        /// </summary>
        public async Task<bool> RegionBelongsToCountryAsync(int regionId, int countryId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Regions
                .AsNoTracking()
                .AnyAsync(x =>
                    x.RegionId == regionId &&
                    x.CountryId == countryId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Validates that selected City belongs to selected Region and returns CityName.
        /// CityName is later stored as denormalized text in profile/address tables.
        /// </summary>
        public async Task<(bool Exists, string? CityName)> TryGetCityByRegionAsync(int cityId, int regionId, CancellationToken cancellationToken = default)
        {
            var city = await _dbContext.Cities
                .AsNoTracking()
                .Where(x =>
                    x.CityId == cityId &&
                    x.RegionId == regionId &&
                    x.IsActive)
                .Select(x => new { x.CityName })
                .FirstOrDefaultAsync(cancellationToken);

            if (city == null)
                return (false, null);

            return (true, city.CityName);
        }

        /// <summary>
        /// Loads dashboard read model for the logged-in Agent.
        /// Extend later with order counts, approval details, or commission summary.
        /// </summary>
        public async Task<AgentDashboardDto?> GetAgentDashboardByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await (
                from a in _dbContext.Agents.AsNoTracking()
                join u in _dbContext.Users.AsNoTracking() on a.UserId equals u.Id
                join p in _dbContext.UserProfiles.AsNoTracking() on u.Id equals p.UserId into pJoin
                from p in pJoin.DefaultIfEmpty()
                where a.UserId == userId
                select new AgentDashboardDto
                {
                    AgentId = a.AgentId,
                    UserId = a.UserId,
                    AgentCode = a.AgentCode,
                    //FullName = p != null
                    //    ? ((p.FirstName ?? string.Empty) + " " + (p.LastName ?? string.Empty)).Trim()
                    //    : string.Empty,
                    //Email = u.Email ?? string.Empty,
                    IsApproved = a.IsApproved,
                    IsCommissionEligible = a.IsCommissionEligible,
                    JoinedAt = a.JoinedAt,
                    RegistrationStatus = a.RegistrationStatus
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}
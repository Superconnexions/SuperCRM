using Microsoft.EntityFrameworkCore.Storage;
using SuperCRM.Application.DTOs.Agents;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    /// <summary>
    /// Persistence contract for the Agent registration workflow.
    /// Database interaction stays behind this abstraction so services never call DbContext directly.
    /// </summary>
    public interface IAgentRegistrationRepository
    {
        /// <summary>
        /// Starts a database transaction for the registration workflow.
        /// Identity user creation + profile/address/agent persistence must stay consistent.
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates the next Agent code.
        /// </summary>
        Task<string> GenerateNextAgentCodeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds UserProfile row for the newly created user.
        /// </summary>
        Task AddUserProfileAsync(UserProfile entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds default UserAddress row for the newly created user.
        /// </summary>
        Task AddUserAddressAsync(UserAddress entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds Agent row for the newly created user.
        /// </summary>
        Task AddAgentAsync(Agent entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all pending changes.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads dashboard read model for Agent.
        /// </summary>
        Task<AgentDashboardDto?> GetAgentDashboardByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates Country existence.
        /// </summary>
        Task<bool> CountryExistsAsync(int countryId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates that a Region belongs to the selected Country.
        /// </summary>
        Task<bool> RegionBelongsToCountryAsync(int regionId, int countryId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates that a City belongs to the selected Region and returns CityName.
        /// Used to store denormalized City text in profile/address tables.
        /// </summary>
        Task<(bool Exists, string? CityName)> TryGetCityByRegionAsync(int cityId, int regionId, CancellationToken cancellationToken = default);
    }
}
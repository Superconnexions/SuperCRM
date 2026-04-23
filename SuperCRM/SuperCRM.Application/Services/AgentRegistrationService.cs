using SuperCRM.Application.DTOs.AgentRegistration;
using SuperCRM.Application.DTOs.Agents;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Security;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    /// <summary>
    /// Standard application service for Agent registration.
    /// Business rules, validation, and orchestration live here.
    /// Geo integrity is validated through repository methods.
    /// </summary>
    public class AgentRegistrationService : IAgentRegistrationService
    {
        private readonly IAgentRegistrationRepository _repository;
        private readonly IApplicationUserAccountService _userAccountService;

        public AgentRegistrationService(
            IAgentRegistrationRepository repository,
            IApplicationUserAccountService userAccountService)
        {
            _repository = repository;
            _userAccountService = userAccountService;
        }

        /// <summary>
        /// Registers a new Agent.
        /// Flow:
        /// 1. Validate required input.
        /// 2. Validate Country -> Region -> City relationship.
        /// 3. Ensure email/username do not already exist.
        /// 4. Create ASP.NET Identity user and assign Agent role.
        /// 5. Create UserProfile, UserAddress, and Agent records in one transaction.
        /// 6. Store CityId and denormalized City name in profile/address.
        /// </summary>
        public async Task<AgentRegistrationResultDto> RegisterAsync(
            AgentRegistrationRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Fail("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return Fail("Password is required.");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                return Fail("First Name is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                return Fail("Last Name is required.");

            if (!request.CountryId.HasValue || request.CountryId.Value <= 0)
                return Fail("Country is required.");

            if (!request.RegionId.HasValue || request.RegionId.Value <= 0)
                return Fail("Region is required.");

            if (!request.CityId.HasValue || request.CityId.Value <= 0)
                return Fail("City is required.");

            var countryId = request.CountryId.Value;
            var regionId = request.RegionId.Value;
            var cityId = request.CityId.Value;

            var countryExists = await _repository.CountryExistsAsync(countryId, cancellationToken);
            if (!countryExists)
                return Fail("Invalid country selection.");

            var regionValid = await _repository.RegionBelongsToCountryAsync(
                regionId,
                countryId,
                cancellationToken);

            if (!regionValid)
                return Fail("Selected region does not belong to the selected country.");

            var cityLookup = await _repository.TryGetCityByRegionAsync(
                cityId,
                regionId,
                cancellationToken);

            if (!cityLookup.Exists || string.IsNullOrWhiteSpace(cityLookup.CityName))
                return Fail("Selected city does not belong to the selected region.");

            var cityName = cityLookup.CityName.Trim();

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var normalizedUserName = normalizedEmail; // Current rule: username = email

            if (await _userAccountService.EmailExistsAsync(normalizedEmail, cancellationToken))
                return Fail("Email already exists.");

            if (await _userAccountService.UserNameExistsAsync(normalizedUserName, cancellationToken))
                return Fail("User Name already exists.");

            await using var tx = await _repository.BeginTransactionAsync(cancellationToken);

            try
            {
                var createUserResult = await _userAccountService.CreateAgentUserAsync(
                    normalizedEmail,
                    normalizedUserName,
                    request.Password,
                    string.IsNullOrWhiteSpace(request.MobileNo) ? request.PhoneNo : request.MobileNo,
                    true,
                    cancellationToken);

                if (!createUserResult.Success || createUserResult.UserId == null)
                {
                    await tx.RollbackAsync(cancellationToken);
                    return Fail(createUserResult.ErrorMessage);
                }

                var userId = createUserResult.UserId.Value;
                var agentCode = await _repository.GenerateNextAgentCodeAsync(cancellationToken);

                var profile = new UserProfile
                {
                    UserId = userId,
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    PhoneNo = CleanOptional(request.PhoneNo),
                    MobileNo = CleanOptional(request.MobileNo),
                    CountryId = countryId,
                    RegionId = regionId,
                    CityId = cityId,
                    City = cityName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    UpdatedByUserId = request.UpdatedByUserId
                };

                var address = new UserAddress
                {
                    UserAddressId = Guid.NewGuid(),
                    UserId = userId,
                    AddressType = (byte)AddressType.Personal,
                    HouseNo = CleanOptional(request.HouseNo),
                    RoadName = CleanOptional(request.RoadName),
                    AddressLine1 = CleanOptional(request.AddressLine1),
                    AddressLine2 = CleanOptional(request.AddressLine2),
                    City = cityName,
                    PostCode = CleanOptional(request.PostCode),
                    CountryId = countryId,
                    RegionId = regionId,
                    CityId = cityId,
                    IsDefault = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    UpdatedByUserId = request.UpdatedByUserId
                };

                var agent = new Agent
                {
                    AgentId = Guid.NewGuid(),
                    UserId = userId,
                    AgentCode = agentCode,
                    ManagerUserId = null,
                    IsApproved = false,
                    ApprovedByUserId = null,
                    ApprovedAt = null,
                    IsCommissionEligible = true,
                    RegistrationSource = (byte)RegistrationSource.SelfRegistration,
                    RegistrationStatus = (byte)AgentRegistrationStatus.PendingApproval,
                    JoinedAt = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    UpdatedByUserId = request.UpdatedByUserId
                };

                await _repository.AddUserProfileAsync(profile, cancellationToken);
                await _repository.AddUserAddressAsync(address, cancellationToken);
                await _repository.AddAgentAsync(agent, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);

                return new AgentRegistrationResultDto
                {
                    Success = true,
                    UserId = userId,
                    AgentId = agent.AgentId,
                    AgentCode = agent.AgentCode,
                    RegistrationStatus = agent.RegistrationStatus
                };
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(cancellationToken);
                return Fail($"Agent registration failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the dashboard read model for the logged-in Agent.
        /// </summary>
        public async Task<AgentDashboardDto?> GetDashboardAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetAgentDashboardByUserIdAsync(userId, cancellationToken);
        }

        /// <summary>
        /// Standard failure response builder.
        /// </summary>
        private static AgentRegistrationResultDto Fail(string message)
        {
            return new AgentRegistrationResultDto
            {
                Success = false,
                ErrorMessage = message
            };
        }

        /// <summary>
        /// Trims optional text fields and converts empty strings to null.
        /// Keeps DB values clean and consistent.
        /// </summary>
        private static string? CleanOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
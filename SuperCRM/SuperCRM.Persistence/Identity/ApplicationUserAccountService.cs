using Microsoft.AspNetCore.Identity;
using SuperCRM.Application.Interfaces.Security;
using SuperCRM.Shared;

namespace SuperCRM.Persistence.Identity
{
    /// <summary>
    /// Identity-specific account service implementation.
    /// Keeps ASP.NET Identity framework details out of the Application layer.
    /// </summary>
    public class ApplicationUserAccountService : IApplicationUserAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserAccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Checks whether an email is already used by another ASP.NET Identity user.
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        /// <summary>
        /// Checks whether a username is already used by another ASP.NET Identity user.
        /// </summary>
        public async Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user != null;
        }

        /// <summary>
        /// Creates a new identity user and immediately assigns the Agent role.
        /// </summary>
        public async Task<(bool Success, Guid? UserId, string ErrorMessage)> CreateAgentUserAsync(
            string email,
            string userName,
            string password,
            string? phoneNumber,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = userName,
                NormalizedEmail = email.ToUpperInvariant(),
                NormalizedUserName = userName.ToUpperInvariant(),
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(x => x.Description));
                return (false, null, errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.Agent);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(x => x.Description));
                return (false, null, errors);
            }

            return (true, user.Id, string.Empty);
        }
    }
}

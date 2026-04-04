using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SuperCRM.Persistence.Identity;
using SuperCRM.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCRM.Infrastructure.Services
{
    public class IdentitySeedService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SeedAdminSettings _seedAdminSettings;

        public IdentitySeedService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IOptions<SeedAdminSettings> seedAdminOptions)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _seedAdminSettings = seedAdminOptions.Value;
        }

        public async Task SeedAsync()
        {
            await EnsureRolesAsync();
            //await EnsureSuperAdminAsync();
        }

        private async Task EnsureRolesAsync()
        {
            var roles = new[]
            {
            AppRoles.SuperAdmin,
            AppRoles.SuperCRMAdmin,
            AppRoles.Manager,
            AppRoles.Agent,
            AppRoles.ReadOnly
        };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new ApplicationRole
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant()
                    });
                }
            }
        }

        private async Task EnsureSuperAdminAsync()
        {
            var email = _seedAdminSettings.Email;
            if (string.IsNullOrWhiteSpace(email))
                return;

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, AppRoles.SuperAdmin))
                {
                    await _userManager.AddToRoleAsync(user, AppRoles.SuperAdmin);
                }
                return;
            }

            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = _seedAdminSettings.Email,
                UserName = _seedAdminSettings.UserName,
                NormalizedEmail = _seedAdminSettings.Email.ToUpperInvariant(),
                NormalizedUserName = _seedAdminSettings.UserName.ToUpperInvariant(),
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, _seedAdminSettings.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Super admin creation failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, AppRoles.SuperAdmin);
        }
    }
}

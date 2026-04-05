namespace SuperCRM.Application.Interfaces.Security
{
    /// <summary>
    /// Abstracts ASP.NET Identity operations from the Application layer.
    /// This keeps the Application layer independent from direct framework calls.
    /// </summary>
    public interface IApplicationUserAccountService
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new ASP.NET Identity user and assigns the Agent role.
        /// </summary>
        Task<(bool Success, Guid? UserId, string ErrorMessage)> CreateAgentUserAsync(
            string email,
            string userName,
            string password,
            string? phoneNumber,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}

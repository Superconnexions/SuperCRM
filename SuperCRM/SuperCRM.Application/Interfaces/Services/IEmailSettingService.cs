using SuperCRM.Application.DTOs.EmailSettings;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IEmailSettingService
    {
        Task<List<EmailSettingDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<EmailSettingDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateEmailSettingDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateEmailSettingDto request, CancellationToken cancellationToken = default);
    }
}

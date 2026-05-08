using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Services
{
    public class EmailSettingService : IEmailSettingService
    {
        private readonly IEmailSettingRepository _repository;
        private readonly IEmailEncryptionService _encryptionService;

        public EmailSettingService(IEmailSettingRepository repository, IEmailEncryptionService encryptionService)
        {
            _repository = repository;
            _encryptionService = encryptionService;
        }

        public async Task<List<EmailSettingDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        public async Task<EmailSettingDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string ErrorMessage)> CreateAsync(CreateEmailSettingDto request, CancellationToken cancellationToken = default)
        {
            var validation = await ValidateCreateAsync(request, cancellationToken);
            if (!validation.Success) return validation;

            if (request.IsDefault)
                await _repository.SetAllDefaultFalseAsync(cancellationToken);

            var entity = new EmailSetting
            {
                EmailSettingId = Guid.NewGuid(),
                SettingName = request.SettingName.Trim(),
                SmtpServer = request.SmtpServer.Trim(),
                Port = request.Port,
                SenderName = request.SenderName.Trim(),
                SenderEmail = request.SenderEmail.Trim(),
                Username = request.Username.Trim(),
                EncryptedPassword = _encryptionService.Encrypt(request.Password),
                EnableSsl = request.EnableSsl,
                IsDefault = request.IsDefault,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = request.CreatedByUserId
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateEmailSettingDto request, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(request.EmailSettingId, cancellationToken);
            if (entity == null)
                return (false, "Email setting not found.");

            var validation = await ValidateUpdateAsync(request, cancellationToken);
            if (!validation.Success) return validation;

            if (request.IsDefault)
                await _repository.SetAllDefaultFalseAsync(cancellationToken);

            entity.SettingName = request.SettingName.Trim();
            entity.SmtpServer = request.SmtpServer.Trim();
            entity.Port = request.Port;
            entity.SenderName = request.SenderName.Trim();
            entity.SenderEmail = request.SenderEmail.Trim();
            entity.Username = request.Username.Trim();
            if (!string.IsNullOrWhiteSpace(request.Password))
                entity.EncryptedPassword = _encryptionService.Encrypt(request.Password);
            entity.EnableSsl = request.EnableSsl;
            entity.IsDefault = request.IsDefault;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return (true, string.Empty);
        }

        private async Task<(bool Success, string ErrorMessage)> ValidateCreateAsync(CreateEmailSettingDto request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SettingName)) return (false, "Setting name is required.");
            if (string.IsNullOrWhiteSpace(request.SmtpServer)) return (false, "SMTP server is required.");
            if (request.Port <= 0) return (false, "Port is required.");
            if (string.IsNullOrWhiteSpace(request.SenderEmail)) return (false, "Sender email is required.");
            if (string.IsNullOrWhiteSpace(request.Username)) return (false, "Username is required.");
            if (string.IsNullOrWhiteSpace(request.Password)) return (false, "Password is required.");
            if (await _repository.ExistsBySettingNameAsync(request.SettingName.Trim(), null, cancellationToken)) return (false, "Setting name already exists.");
            return (true, string.Empty);
        }

        private async Task<(bool Success, string ErrorMessage)> ValidateUpdateAsync(UpdateEmailSettingDto request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SettingName)) return (false, "Setting name is required.");
            if (string.IsNullOrWhiteSpace(request.SmtpServer)) return (false, "SMTP server is required.");
            if (request.Port <= 0) return (false, "Port is required.");
            if (string.IsNullOrWhiteSpace(request.SenderEmail)) return (false, "Sender email is required.");
            if (string.IsNullOrWhiteSpace(request.Username)) return (false, "Username is required.");
            if (await _repository.ExistsBySettingNameAsync(request.SettingName.Trim(), request.EmailSettingId, cancellationToken)) return (false, "Setting name already exists.");
            return (true, string.Empty);
        }

        private static EmailSettingDto MapToDto(EmailSetting entity)
        {
            return new EmailSettingDto
            {
                EmailSettingId = entity.EmailSettingId,
                SettingName = entity.SettingName,
                SmtpServer = entity.SmtpServer,
                Port = entity.Port,
                SenderName = entity.SenderName,
                SenderEmail = entity.SenderEmail,
                Username = entity.Username,
                EnableSsl = entity.EnableSsl,
                IsDefault = entity.IsDefault,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}

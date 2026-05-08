using MailKit.Security;
using MimeKit;
using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Infrastructure.Email
{
    public class MailKitEmailSenderService : IEmailSenderService
    {
        private readonly IEmailSettingRepository _emailSettingRepository;
        private readonly IEmailLogRepository _emailLogRepository;
        private readonly IEmailEncryptionService _encryptionService;

        public MailKitEmailSenderService(
            IEmailSettingRepository emailSettingRepository,
            IEmailLogRepository emailLogRepository,
            IEmailEncryptionService encryptionService)
        {
            _emailSettingRepository = emailSettingRepository;
            _emailLogRepository = emailLogRepository;
            _encryptionService = encryptionService;
        }

        public Task<SendEmailResultDto> SendTestEmailAsync(string toEmail, Guid? requestedByUserId, CancellationToken cancellationToken = default)
        {
            return SendAsync(new SendEmailRequestDto
            {
                ToEmail = toEmail,
                Subject = "SuperCRM Test Email",
                Body = "<p>This is a test email from SuperCRM email configuration.</p>",
                IsHtml = true,
                SourceModule = "EmailSettings-Test",
                CreatedByUserId = requestedByUserId
            }, cancellationToken);
        }

        public async Task<SendEmailResultDto> SendAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default)
        {
            var setting = await _emailSettingRepository.GetDefaultActiveAsync(cancellationToken);
            if (setting == null)
                return new SendEmailResultDto { Success = false, Message = "No active default email setting found." };

            var log = new EmailLog
            {
                EmailLogId = Guid.NewGuid(),
                EmailSettingId = setting.EmailSettingId,
                ToEmail = request.ToEmail,
                CcEmail = request.CcEmail,
                BccEmail = request.BccEmail,
                Subject = request.Subject,
                Body = request.Body,
                BodyPreview = request.Body.Length > 1000 ? request.Body[..1000] : request.Body,
                IsHtml = request.IsHtml,
                IsSent = false,
                SourceModule = request.SourceModule,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = request.CreatedByUserId
            };

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(setting.SenderName, setting.SenderEmail));
                AddAddresses(message.To, request.ToEmail);
                AddAddresses(message.Cc, request.CcEmail);
                AddAddresses(message.Bcc, request.BccEmail);
                message.Subject = request.Subject;

                var bodyBuilder = new BodyBuilder();
                if (request.IsHtml)
                    bodyBuilder.HtmlBody = request.Body;
                else
                    bodyBuilder.TextBody = request.Body;
                message.Body = bodyBuilder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                var secureSocketOption = setting.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

                await smtp.ConnectAsync(setting.SmtpServer, setting.Port, secureSocketOption, cancellationToken);
                await smtp.AuthenticateAsync(setting.Username, _encryptionService.Decrypt(setting.EncryptedPassword), cancellationToken);
                await smtp.SendAsync(message, cancellationToken);
                await smtp.DisconnectAsync(true, cancellationToken);

                log.IsSent = true;
                log.SentAt = DateTime.UtcNow;

                await _emailLogRepository.AddAsync(log, cancellationToken);
                await _emailLogRepository.SaveChangesAsync(cancellationToken);

                return new SendEmailResultDto { Success = true, Message = "Email sent successfully.", EmailLogId = log.EmailLogId };
            }
            catch (Exception ex)
            {
                log.IsSent = false;
                log.ErrorMessage = ex.Message;

                await _emailLogRepository.AddAsync(log, cancellationToken);
                await _emailLogRepository.SaveChangesAsync(cancellationToken);

                return new SendEmailResultDto { Success = false, Message = ex.Message, EmailLogId = log.EmailLogId };
            }
        }

        private static void AddAddresses(InternetAddressList list, string? addresses)
        {
            if (string.IsNullOrWhiteSpace(addresses)) return;

            foreach (var address in addresses.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                list.Add(MailboxAddress.Parse(address));
            }
        }
    }
}

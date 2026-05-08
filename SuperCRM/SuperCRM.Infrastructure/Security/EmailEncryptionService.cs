using Microsoft.AspNetCore.DataProtection;
using SuperCRM.Application.Interfaces.Services;

namespace SuperCRM.Infrastructure.Security
{
    /// <summary>
    /// Uses ASP.NET Core Data Protection to encrypt/decrypt SMTP passwords.
    /// In production, configure persistent data protection keys, otherwise encrypted
    /// values may become unreadable after app redeploy/server change.
    /// </summary>
    public class EmailEncryptionService : IEmailEncryptionService
    {
        private readonly IDataProtector _protector;

        // IMPORTANT:
        // Do NOT change this string after production deployment.
        // Existing encrypted SMTP passwords will become undecryptable.
        public EmailEncryptionService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("SuperCRM.EmailSettings.Encryption.DoNotChange");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return string.Empty;

            return _protector.Protect(plainText);
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                return string.Empty;

            return _protector.Unprotect(encryptedText);
        }
    }
}

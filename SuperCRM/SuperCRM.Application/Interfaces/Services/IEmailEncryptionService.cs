namespace SuperCRM.Application.Interfaces.Services
{
    public interface IEmailEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}

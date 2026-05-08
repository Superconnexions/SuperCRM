namespace SuperCRM.Application.DTOs.EmailSettings
{
    public class CreateEmailSettingDto
    {
        public string SettingName { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CreatedByUserId { get; set; }
    }
}

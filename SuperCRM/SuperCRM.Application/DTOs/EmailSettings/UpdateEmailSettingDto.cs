namespace SuperCRM.Application.DTOs.EmailSettings
{
    public class UpdateEmailSettingDto
    {
        public Guid EmailSettingId { get; set; }
        public string SettingName { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; }
        public bool EnableSsl { get; set; } = true;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid UpdatedByUserId { get; set; }
    }
}

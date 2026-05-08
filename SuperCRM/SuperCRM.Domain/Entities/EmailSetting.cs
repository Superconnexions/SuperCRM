
namespace SuperCRM.Domain.Entities
{
    public class EmailSetting
    {
        public Guid EmailSettingId { get; set; }
        public string SettingName { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        //public ApplicationUser? CreatedByUser { get; set; }
        //public ApplicationUser? UpdatedByUser { get; set; }
        public ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();
    }
}

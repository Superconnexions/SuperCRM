namespace SuperCRM.Web.ViewModels.EmailSettings
{
    public class EmailSettingListItemViewModel
    {
        public Guid EmailSettingId { get; set; }
        public string SettingName { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}

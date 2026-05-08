namespace SuperCRM.Web.ViewModels.EmailSettings
{
    public class EmailLogListItemViewModel
    {
        public Guid EmailLogId { get; set; }
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SourceModule { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

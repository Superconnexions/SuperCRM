namespace SuperCRM.Application.DTOs.EmailSettings
{
    public class EmailLogDto
    {
        public Guid EmailLogId { get; set; }
        public Guid? EmailSettingId { get; set; }
        public string ToEmail { get; set; } = string.Empty;
        public string? CcEmail { get; set; }
        public string? BccEmail { get; set; }
        public string Subject { get; set; } = string.Empty;
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SourceModule { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

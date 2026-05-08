namespace SuperCRM.Application.DTOs.EmailSettings
{
    public class SendEmailRequestDto
    {
        public string ToEmail { get; set; } = string.Empty;
        public string? CcEmail { get; set; }
        public string? BccEmail { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public string? SourceModule { get; set; }
        public Guid? CreatedByUserId { get; set; }
    }
}

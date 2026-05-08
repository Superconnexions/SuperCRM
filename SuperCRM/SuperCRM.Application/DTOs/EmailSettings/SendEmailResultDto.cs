namespace SuperCRM.Application.DTOs.EmailSettings
{
    public class SendEmailResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? EmailLogId { get; set; }
    }
}

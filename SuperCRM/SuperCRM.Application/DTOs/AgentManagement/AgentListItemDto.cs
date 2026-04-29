namespace SuperCRM.Application.DTOs.AgentManagement
{
    /// <summary>
    /// Agent list read model for SuperAdmin/SuperCRMAdmin management page.
    /// </summary>
    public class AgentListItemDto
    {
        public Guid AgentId { get; set; }
        public Guid UserId { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public bool IsApproved { get; set; }
        public byte RegistrationStatus { get; set; }
        public string RegistrationStatusName { get; set; } = string.Empty;
    }
}

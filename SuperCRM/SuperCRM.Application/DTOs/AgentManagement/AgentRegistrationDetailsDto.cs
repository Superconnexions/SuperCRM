namespace SuperCRM.Application.DTOs.AgentManagement
{
    /// <summary>
    /// Full Agent registration details for admin view/edit page.
    /// </summary>
    public class AgentRegistrationDetailsDto
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
        public bool IsCommissionEligible { get; set; }
        public byte RegistrationStatus { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

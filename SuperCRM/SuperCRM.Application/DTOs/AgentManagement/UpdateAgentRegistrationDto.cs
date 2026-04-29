namespace SuperCRM.Application.DTOs.AgentManagement
{
    /// <summary>
    /// Admin update request for Agent registration approval/status page.
    /// IsApproved is intentionally not accepted from UI because it is controlled by service logic.
    /// </summary>
    public class UpdateAgentRegistrationDto
    {
        public Guid AgentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public bool IsCommissionEligible { get; set; }
        public byte RegistrationStatus { get; set; }
        public Guid UpdatedByUserId { get; set; }
    }
}

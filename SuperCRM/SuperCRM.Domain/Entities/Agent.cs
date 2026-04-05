namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Represents the Agent business table.
    /// This record is created after the ASP.NET Identity user is created successfully.
    /// </summary>
    public class Agent
    {
        public Guid AgentId { get; set; }
        public Guid UserId { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public Guid? ManagerUserId { get; set; }
        public bool IsApproved { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsCommissionEligible { get; set; }
        public byte RegistrationSource { get; set; }
        public byte RegistrationStatus { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

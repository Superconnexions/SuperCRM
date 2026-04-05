namespace SuperCRM.Application.DTOs.Agents
{
    /// <summary>
    /// Read model for the Agent dashboard.
    /// Keeps controller code thin and avoids exposing EF entities directly to the Web layer.
    /// </summary>
    public class AgentDashboardDto
    {
        public Guid AgentId { get; set; }
        public Guid UserId { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsCommissionEligible { get; set; }
        public byte RegistrationStatus { get; set; }
        public string RegistrationStatusText { get; set; } = string.Empty;
        public DateTime? JoinedAt { get; set; }
    }
}

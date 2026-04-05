namespace SuperCRM.Application.DTOs.Agents
{
    /// <summary>
    /// Result of the agent registration process.
    /// Used by the Web layer to decide whether to show success, validation, or error output.
    /// </summary>
    public class AgentRegistrationResultDto
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public Guid? AgentId { get; set; }
        public string? AgentCode { get; set; }
        public byte RegistrationStatus { get; set; }
    }
}

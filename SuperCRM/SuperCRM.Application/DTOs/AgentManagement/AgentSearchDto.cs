namespace SuperCRM.Application.DTOs.AgentManagement
{
    /// <summary>
    /// Search criteria for Agent management list.
    /// </summary>
    public class AgentSearchDto
    {
        public string? SearchText { get; set; }
        public bool? IsApproved { get; set; }
        public byte? RegistrationStatus { get; set; }
    }
}

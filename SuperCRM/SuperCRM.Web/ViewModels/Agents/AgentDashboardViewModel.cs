namespace SuperCRM.Web.ViewModels.Agents
{
    /// <summary>
    /// Web-facing read model for the Agent dashboard.
    /// </summary>
    public class AgentDashboardViewModel
    {
        public string AgentCode { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsCommissionEligible { get; set; }
        public string RegistrationStatusText { get; set; } = string.Empty;
        public DateTime? JoinedAt { get; set; }
    }
}

namespace SuperCRM.Web.ViewModels.AgentManagement
{
    /// <summary>
    /// Row model for Agent list page.
    /// </summary>
    public class AgentListItemViewModel
    {
        public Guid AgentId { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public bool IsApproved { get; set; }
        public string IsApprovedText => IsApproved ? "Yes" : "No";
        public byte RegistrationStatus { get; set; }
        public string RegistrationStatusName { get; set; } = string.Empty;
    }
}

namespace SuperCRM.Domain.Enums
{
    /// <summary>
    /// Agent registration lifecycle values aligned with the Agents table.
    /// </summary>
    public enum AgentRegistrationStatus : byte
    {
        PendingApproval = 1,
        Active = 2,
        Rejected = 3,
        Suspended = 4
    }
}

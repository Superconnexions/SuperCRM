namespace SuperCRM.Domain.Enums
{
    /// <summary>
    /// Registration source values aligned with SCHEMA_V1_BASELINE.
    /// </summary>
    public enum RegistrationSource : byte
    {
        AgentCreated = 1,
        AdminCreated = 2,
        SelfRegistration = 3,
        Imported = 4
    }
}

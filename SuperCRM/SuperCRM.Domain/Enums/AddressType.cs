namespace SuperCRM.Domain.Enums
{
    /// <summary>
    /// Standard address type values aligned with SCHEMA_V1_BASELINE.
    /// </summary>
    public enum AddressType : byte
    {
        Personal = 1,
        Business = 2,
        Mailing = 3,
        Shipping = 4
    }
}

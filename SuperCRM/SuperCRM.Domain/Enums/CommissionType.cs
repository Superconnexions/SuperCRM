namespace SuperCRM.Domain.Enums
{
    /// <summary>
    /// Base commission type for a product.
    /// Adjust enum names/values if your existing project already defines this enum elsewhere.
    /// </summary>
    public enum CommissionType : byte
    {
        FixedAmount = 1,
        Percentage = 2
    }
}

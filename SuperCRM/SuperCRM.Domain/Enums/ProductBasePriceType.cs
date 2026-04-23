namespace SuperCRM.Domain.Enums
{
    /// <summary>
    /// Base price mode used in Products.BasePriceType.
    /// </summary>
    public enum ProductBasePriceType : byte
    {
        SimplePrice = 1,
        OpenPrice = 2,
        VariantPrice = 3
    }
}

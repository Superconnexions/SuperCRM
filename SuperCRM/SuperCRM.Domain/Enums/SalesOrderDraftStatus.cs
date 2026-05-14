namespace SuperCRM.Domain.Enums
{
    public enum SalesOrderDraftStatus : byte
    {
        ProductSelected = 1,
        CustomerSelected = 2,
        ReadyForConfirmation = 3,
        Confirmed = 4,
        Cancelled = 5
    }
}

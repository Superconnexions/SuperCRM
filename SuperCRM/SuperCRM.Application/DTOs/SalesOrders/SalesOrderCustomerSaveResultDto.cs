namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderCustomerSaveResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid SalesOrderDraftId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerBusinessId { get; set; }
        public Guid? CustomerAddressId { get; set; }
        public Guid? CustomerBankAccountId { get; set; }
    }
}

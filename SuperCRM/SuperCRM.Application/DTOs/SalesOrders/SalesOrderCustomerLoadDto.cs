namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderCustomerLoadDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public SalesOrderCustomerDto? Customer { get; set; }
        public SalesOrderCustomerAddressDto? PersonalAddress { get; set; }
        public SalesOrderBusinessDto? Business { get; set; }
        public SalesOrderCustomerAddressDto? BusinessAddress { get; set; }
        public SalesOrderBankAccountDto? BankAccount { get; set; }
    }
}

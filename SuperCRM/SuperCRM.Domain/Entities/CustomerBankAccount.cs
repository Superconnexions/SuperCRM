namespace SuperCRM.Domain.Entities
{
    public class CustomerBankAccount
    {
        public Guid CustomerBankAccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SortCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public Customer? Customer { get; set; }
    }
}

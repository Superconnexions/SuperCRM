namespace SuperCRM.Domain.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public byte RegistrationSource { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsCompanyDirector { get; set; }

        public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
        public ICollection<CustomerBusiness> Businesses { get; set; } = new List<CustomerBusiness>();
        public ICollection<CustomerBankAccount> BankAccounts { get; set; } = new List<CustomerBankAccount>();
    }
}

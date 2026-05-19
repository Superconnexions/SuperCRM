namespace SuperCRM.Domain.Entities
{
    public class CustomerBusiness
    {
        public Guid CustomerBusinessId { get; set; }
        public Guid CustomerId { get; set; }
        public byte BusinessType { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public string? TradingName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }

        public Customer? Customer { get; set; }
        public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    }
}

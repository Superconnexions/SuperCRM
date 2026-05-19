namespace SuperCRM.Domain.Entities
{
    public class CustomerAddress
    {
        public Guid CustomerAddressId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerBusinessId { get; set; }
        public byte AddressType { get; set; }
        public string? AddressLine { get; set; }
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public bool IsBusinessAddressSame { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public Customer? Customer { get; set; }
        public CustomerBusiness? CustomerBusiness { get; set; }
        public Country? Country { get; set; }
        public Region? Region { get; set; }
    }
}

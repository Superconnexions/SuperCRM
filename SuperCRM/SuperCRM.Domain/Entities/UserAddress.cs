namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Stores one or more addresses for an internal system user.
    /// Agent registration creates a default personal address record.
    /// </summary>
    public class UserAddress
    {
        public Guid UserAddressId { get; set; }
        public Guid UserId { get; set; }
        public byte AddressType { get; set; }
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? PostCode { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

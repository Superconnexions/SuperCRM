namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Stores the personal profile details for an internal system user.
    /// Agent registration creates one UserProfile linked to the newly created ApplicationUser.
    /// </summary>
    public class UserProfile
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}


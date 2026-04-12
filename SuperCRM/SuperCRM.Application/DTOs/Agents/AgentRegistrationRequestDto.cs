namespace SuperCRM.Application.DTOs.AgentRegistration
{
    public class AgentRegistrationRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }

        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? PostCode { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? CityId { get; set; }

        /// <summary>
        /// Optional UI/helper field only.
        /// Final stored city text should come from master table lookup by CityId.
        /// </summary>
        public string? City { get; set; }
    }
}
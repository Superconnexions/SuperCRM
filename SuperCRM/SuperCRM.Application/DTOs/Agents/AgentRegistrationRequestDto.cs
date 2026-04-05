namespace SuperCRM.Application.DTOs.Agents
{
    /// <summary>
    /// Request payload used by the agent registration service.
    /// Web ViewModel is mapped to this DTO before entering application logic.
    /// </summary>
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
        public string? City { get; set; }
        public string? PostCode { get; set; }
        public int? CountryId { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

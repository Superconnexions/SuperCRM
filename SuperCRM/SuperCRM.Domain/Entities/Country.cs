namespace SuperCRM.Domain.Entities
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string? IsoCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
namespace SuperCRM.Domain.Entities
{
    public class Region
    {
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
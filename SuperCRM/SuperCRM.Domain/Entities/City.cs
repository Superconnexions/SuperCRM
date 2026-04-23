namespace SuperCRM.Domain.Entities
{
    public class City
    {
        public int CityId { get; set; }
        public int RegionId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Sales unit master entity.
    /// Used to define how products are priced and displayed in order lines.
    /// Example UnitCode values: EACH, PER-MONTH, PER-UNIT.
    /// </summary>
    public class SalesUnit
    {
        public int SalesUnitId { get; set; }
        public string UnitCode { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

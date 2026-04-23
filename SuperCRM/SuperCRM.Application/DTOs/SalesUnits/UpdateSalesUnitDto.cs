namespace SuperCRM.Application.DTOs.SalesUnits
{
    /// <summary>
    /// Update DTO for Sales Unit.
    /// SalesUnitId identifies the target row.
    /// </summary>
    public class UpdateSalesUnitDto
    {
        public int SalesUnitId { get; set; }
        public string UnitCode { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

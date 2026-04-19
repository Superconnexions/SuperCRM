namespace SuperCRM.Application.DTOs.SalesUnits
{
    /// <summary>
    /// Create DTO for Sales Unit.
    /// </summary>
    public class CreateSalesUnitDto
    {
        public string UnitCode { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

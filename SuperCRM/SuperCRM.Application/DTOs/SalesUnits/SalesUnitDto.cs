namespace SuperCRM.Application.DTOs.SalesUnits
{
    /// <summary>
    /// Read DTO for Sales Unit list/detail display.
    /// </summary>
    public class SalesUnitDto
    {
        public int SalesUnitId { get; set; }
        public string UnitCode { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

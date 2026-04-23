namespace SuperCRM.Web.ViewModels.SalesUnits
{
    /// <summary>
    /// Lightweight ViewModel for Sales Unit list page.
    /// </summary>
    public class SalesUnitListItemViewModel
    {
        public int SalesUnitId { get; set; }
        public string UnitCode { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

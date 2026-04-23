using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Web.ViewModels.SalesUnits
{
    /// <summary>
    /// UI ViewModel for Create/Edit Sales Unit page.
    /// </summary>
    public class SalesUnitCreateEditViewModel
    {
        public int? SalesUnitId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Unit Code")]
        public string UnitCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Unit Name")]
        public string UnitName { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public bool IsEditMode { get; set; }
    }
}

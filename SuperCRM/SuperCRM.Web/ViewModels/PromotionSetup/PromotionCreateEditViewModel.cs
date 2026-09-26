using System.ComponentModel.DataAnnotations;
using SuperCRM.Application.DTOs.PromotionSetup;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Web.ViewModels.PromotionSetup
{
    public class PromotionCreateEditViewModel
    {
        public Guid? PromotionId { get; set; }

        public string? PromotionCode { get; set; }

        [Required]
        [StringLength(200)]
        public string PromotionName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? PromotionSummary { get; set; }

        [StringLength(2000)]
        public string? NotificationMessage { get; set; }

        [DataType(DataType.Date)]
        public DateTime PromotionStartDate { get; set; }
            = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime PromotionEndDate { get; set; }
            = DateTime.Today;

        [StringLength(500)]
        public string? Remarks { get; set; }

        public bool Submitted { get; set; }

        public bool Published { get; set; }

        public bool Cancelled { get; set; }

        public DateTime? CancelledAt { get; set; }

        public Guid? CancelledByUserId { get; set; }

        // Default Yes.
        // Visible/editable on Edit page after PromotionId exists.
        public bool IsActive { get; set; } = true;

        public bool EmailNotificationToAgents { get; set; }

        public byte NoOfEmailNotificationSend { get; set; }

        public List<PromotionItemViewModel> Items { get; set; }
            = new();

        public List<PromotionProductLookupDto> ProductOptions
            { get; set; }
            = new();
    }

    public class PromotionItemViewModel
    {
        public Guid? PromotionItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductBaseCommissionId { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public decimal StandardCommission { get; set; }

        public PromotionType PromotionType { get; set; }
            = PromotionType.FixedAmount;

        public decimal? PromotionPercentage { get; set; }

        public decimal? PromotionAmount { get; set; }

        public decimal FinalCommissionAmount { get; set; }

        // New item defaults to Yes.
        public bool IsActive { get; set; } = true;
    }
}

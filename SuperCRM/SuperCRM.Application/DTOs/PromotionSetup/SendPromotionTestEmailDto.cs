using System.ComponentModel.DataAnnotations;

namespace SuperCRM.Application.DTOs.PromotionSetup
{
    public class SendPromotionTestEmailDto
    {
        public Guid PromotionId { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string TestEmail { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
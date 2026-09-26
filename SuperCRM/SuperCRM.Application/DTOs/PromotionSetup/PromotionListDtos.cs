namespace SuperCRM.Application.DTOs.PromotionSetup
{
    public class PromotionListFilterDto
    {
        public string? PromotionName { get; set; }

        // Exact PromotionStartDate search.
        public DateTime? PromotionStartDate { get; set; }

        // Exact PromotionEndDate search.
        public DateTime? PromotionEndDate { get; set; }

        public bool? Published { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class PromotionListItemDto
    {
        public Guid PromotionId { get; set; }

        public string PromotionCode { get; set; } = string.Empty;

        public string PromotionName { get; set; } = string.Empty;

        public DateTime PromotionStartDate { get; set; }

        public DateTime PromotionEndDate { get; set; }

        public bool Published { get; set; }

        public bool IsActive { get; set; }

        public bool Cancelled { get; set; }

        public int NoOfEmailNotificationSend { get; set; }

        public int TotalProducts { get; set; }

        public int ActiveProducts { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class PromotionListResultDto
    {
        public List<PromotionListItemDto> Items { get; set; } = new();

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }
    }
}

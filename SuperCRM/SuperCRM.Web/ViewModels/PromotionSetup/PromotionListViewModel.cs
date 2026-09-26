namespace SuperCRM.Web.ViewModels.PromotionSetup
{
    public class PromotionListViewModel
    {
        public string? PromotionName { get; set; }

        public DateTime? PromotionStartDate { get; set; }

        public DateTime? PromotionEndDate { get; set; }

        public bool? Published { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public List<PromotionListRowViewModel> Promotions { get; set; } = new();
    }

    public class PromotionListRowViewModel
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
}

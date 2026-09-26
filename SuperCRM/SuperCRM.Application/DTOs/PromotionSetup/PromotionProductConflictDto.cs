public class PromotionProductConflictDto
{
    public Guid ProductId { get; set; }

    public string ProductCode { get; set; }
        = string.Empty;

    public string ProductName { get; set; }
        = string.Empty;

    public Guid PromotionId { get; set; }

    public string PromotionCode { get; set; }
        = string.Empty;

    public string PromotionName { get; set; }
        = string.Empty;

    public DateTime PromotionEndDate { get; set; }
}
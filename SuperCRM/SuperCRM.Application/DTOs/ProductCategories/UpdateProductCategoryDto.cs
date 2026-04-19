namespace SuperCRM.Application.DTOs.ProductCategories
{
    /// <summary>
    /// Update DTO for Product Category.
    /// CategoryId identifies the target row.
    /// </summary>
    public class UpdateProductCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string? CategoryCode { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryImageUrl { get; set; }
        public string? DisplayNotes { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

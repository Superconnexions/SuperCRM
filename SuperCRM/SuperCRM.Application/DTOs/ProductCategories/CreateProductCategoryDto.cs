namespace SuperCRM.Application.DTOs.ProductCategories
{
    /// <summary>
    /// Create DTO for Product Category.
    /// CategoryId is generated in service layer.
    /// </summary>
    public class CreateProductCategoryDto
    {
        public string? CategoryCode { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryImageUrl { get; set; }
        public string? DisplayNotes { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

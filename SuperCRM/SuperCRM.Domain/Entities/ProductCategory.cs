namespace SuperCRM.Domain.Entities
{
    /// <summary>
    /// Product category master entity.
    /// Used to group products for display, filtering, and order-entry navigation.
    /// </summary>
    public class ProductCategory
    {
        public Guid CategoryId { get; set; }
        public string? CategoryCode { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryImageUrl { get; set; }
        public string? DisplayNotes { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

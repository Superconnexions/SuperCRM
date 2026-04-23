
namespace SuperCRM.Application.DTOs.Products
{
    public class CreateProductImageDto
    {
        public Guid ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

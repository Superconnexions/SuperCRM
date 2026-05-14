namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderProductCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public List<SalesOrderProductItemDto> Products { get; set; } = new();
    }
}

namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderProductListDto
    {
        public List<SalesOrderProductCategoryDto> BusinessCategories { get; set; } = new();
        public List<SalesOrderProductCategoryDto> ResidentialCategories { get; set; } = new();
    }
}

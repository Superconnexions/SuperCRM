namespace SuperCRM.Application.DTOs.ProductBaseCommissions
{
    public class ProductLookupDto
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
    }
}

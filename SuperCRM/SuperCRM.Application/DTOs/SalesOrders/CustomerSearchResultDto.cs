namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class CustomerSearchResultDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public bool IsCompanyDirector { get; set; }
    }
}

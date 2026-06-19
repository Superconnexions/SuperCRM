using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class CustomerManagementViewModel
    {
        public bool IsAgentView { get; set; }

        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public string? CustomerCode { get; set; }

        public string? EmailOrMobile { get; set; }
        public List<CustomerManagementListDto> Customers { get; set; } = new();

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalRecords { get; set; }

        public int TotalPages =>
            PageSize <= 0 ? 1 : (int)Math.Ceiling((double)TotalRecords / PageSize);
    }

    public class CustomerSalesOrdersViewModel
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public List<CustomerSalesOrderListDto> Orders { get; set; } = new();
    }
}
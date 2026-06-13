using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class AgentsKpiViewModel
    {
        public DateTime OrderDateFrom { get; set; }
        public DateTime OrderDateTo { get; set; }

        public Guid? AgentId { get; set; }
        public byte? SalesOrderStatus { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalRecords { get; set; }

        public int TotalPages =>
            PageSize <= 0 ? 1 : (int)Math.Ceiling((double)TotalRecords / PageSize);

        public List<SelectListItem> AgentOptions { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new();

        public List<AgentsKpiRowViewModel> Items { get; set; } = new();
    }

    public class AgentsKpiRowViewModel
    {
        public Guid AgentId { get; set; }
        public string AgentCode { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Mobile { get; set; }

        public int TotalCustomer { get; set; }
        public int TotalSalesOrder { get; set; }

        public decimal CommissionUnsettled { get; set; }
        public decimal CommissionSettled { get; set; }
        public decimal CommissionDistributed { get; set; }

        public List<AgentsKpiStatusCountViewModel> StatusCounts { get; set; } = new();
    }

    public class AgentsKpiStatusCountViewModel
    {
        public byte Status { get; set; }
        public string StatusText { get; set; } = "";
        public int Count { get; set; }
    }
}
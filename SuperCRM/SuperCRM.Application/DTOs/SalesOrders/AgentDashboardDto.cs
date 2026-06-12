namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class AgentDashboardDto
    {
        public int MyCustomerCount { get; set; }
        public int MySalesOrderCount { get; set; }
        public int OrdersThisMonthCount { get; set; }
        public int PendingOrderCount { get; set; }

        public decimal TotalCommissionUnsettled { get; set; }
        public decimal TotalCommissionSettled { get; set; }
        public decimal ReceivedCommission { get; set; }

        public List<AgentDashboardStatusDto> StatusSummary { get; set; } = new();
        public List<AgentDashboardCustomerDto> RecentCustomers { get; set; } = new();
        public List<AgentDashboardOrderDto> RecentOrders { get; set; } = new();
    }

    public class AgentDashboardStatusDto
    {
        public byte Status { get; set; }
        public string StatusText { get; set; } = "";
        public int Count { get; set; }
    }

    public class AgentDashboardCustomerDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string RegistrationSourceText { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class AgentDashboardOrderDto
    {
        public Guid SaleId { get; set; }
        public string OrderNo { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = "";
        public string ProviderName { get; set; } = "";
        public string StatusText { get; set; } = "";
        public decimal OrderTotal { get; set; }
    }
}
namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class AdminDashboardDto
    {
        public int TotalCustomerCount { get; set; }
        public int TotalSalesOrderCount { get; set; }
        public int OrdersThisMonthCount { get; set; }
        public int PendingOrderCount { get; set; }

        public decimal TotalCommissionUnsettled { get; set; }
        public decimal TotalCommissionSettled { get; set; }
        public decimal ReceivedCommission { get; set; }

        public decimal ReceivedCommissionBySuperCRM { get; set; }

        public Guid? SelectedAgentUserId { get; set; }
        public Guid? SelectedAdminUserId { get; set; }

        public List<DashboardUserOptionDto> AgentOptions { get; set; } = new();
        public List<DashboardUserOptionDto> AdminOptions { get; set; } = new();

        public List<AgentDashboardStatusDto> StatusSummary { get; set; } = new();
        public List<AgentDashboardCustomerDto> RecentCustomers { get; set; } = new();
        public List<AgentDashboardOrderDto> RecentOrders { get; set; } = new();
    }

    public class DashboardUserOptionDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public string? AgentCode { get; set; }

        public string DisplayText { get; set; } = "";
    }
}
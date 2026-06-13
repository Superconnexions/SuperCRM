namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class AgentsKpiDto
    {
        public DateTime OrderDateFrom { get; set; }
        public DateTime OrderDateTo { get; set; }
        public Guid? AgentId { get; set; }
        public byte? SalesOrderStatus { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public List<AgentsKpiAgentOptionDto> AgentOptions { get; set; } = new();
        public List<AgentsKpiRowDto> Items { get; set; } = new();
    }

    public class AgentsKpiAgentOptionDto
    {
        public Guid AgentId { get; set; }
        public string AgentCode { get; set; } = "";
        public string FullName { get; set; } = "";
    }

    public class AgentsKpiRowDto
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

        public List<AgentsKpiStatusCountDto> StatusCounts { get; set; } = new();
    }

    public class AgentsKpiStatusCountDto
    {
        public byte Status { get; set; }
        public string StatusText { get; set; } = "";
        public int Count { get; set; }
    }
}
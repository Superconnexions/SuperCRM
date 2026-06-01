using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderHistoryViewModel
    {
        public bool IsAgentView { get; set; }
        public List<SalesOrderHistoryDto> Orders { get; set; } = new();
    }
}
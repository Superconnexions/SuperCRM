using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderHistoryViewModel
    {
        public bool IsAgentView { get; set; }

        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public byte? SalesOrderStatus { get; set; }

        public List<SelectListItem> StatusOptions { get; set; } = new();
        public List<SalesOrderHistoryDto> Orders { get; set; } = new();

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalRecords { get; set; }

        public int TotalPages =>
            PageSize <= 0 ? 1 : (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}
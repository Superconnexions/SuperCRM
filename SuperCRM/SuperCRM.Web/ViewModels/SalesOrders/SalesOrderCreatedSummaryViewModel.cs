using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Web.ViewModels.SalesOrders
{
    public class SalesOrderCreatedSummaryViewModel
    {
        public SalesOrderCustomerSummaryDto Customer { get; set; } = new();
        public SalesOrderBusinessSummaryDto? Business { get; set; }
        public SalesOrderAddressSummaryDto? HomeAddress { get; set; }
        public SalesOrderAddressSummaryDto? BusinessAddress { get; set; }
        public SalesOrderBankAccountSummaryDto? BankAccount { get; set; }
        public List<SalesOrderProviderSummaryDto> Orders { get; set; } = new();
    }
}

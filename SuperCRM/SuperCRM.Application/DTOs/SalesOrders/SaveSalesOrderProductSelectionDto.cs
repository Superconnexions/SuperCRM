namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SaveSalesOrderProductSelectionDto
    {
        public Guid? SalesOrderDraftId { get; set; }
        public Guid CurrentUserId { get; set; }
        public List<SaveSalesOrderDraftLineDto> Lines { get; set; } = new();
    }
}

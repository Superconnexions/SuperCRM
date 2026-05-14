namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderDraftSaveResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? SalesOrderDraftId { get; set; }
        public string DraftNo { get; set; } = string.Empty;
    }
}

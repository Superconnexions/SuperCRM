using SuperCRM.Domain.Enums;

namespace SuperCRM.Domain.Entities
{
    public class SaleLine
    {
        public Guid SaleLineId { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string? VariantCode { get; set; }
        public string? VariantName { get; set; }
        public Guid? ProviderProductId { get; set; }
        public int Quantity { get; set; }
        public int SettledQty { get; set; }
        public int PaidQty { get; set; }
        public byte BasePriceType { get; set; }
        public decimal BasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsSalePriceEdited { get; set; }
        public DateTime? PriceFinalizedAt { get; set; }
        public Guid? PriceFinalizedByUserId { get; set; }
        public Guid? ProductBaseCommissionId { get; set; }
        public CommissionType? CommissionType { get; set; }
        public decimal? CommissionValue { get; set; }
        public decimal CalculatedAgentCommission { get; set; }
        public decimal FinalAgentCommission { get; set; }
        public decimal SuperCRMCommissionEarned { get; set; }
        public bool IsCommissionFinalized { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public int SalesUnitId { get; set; }
        public string SalesUnitCode { get; set; } = string.Empty;
        public decimal LineTotalAmount { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
        public int? NoOfInstallment { get; set; }
        public DateTime? FirstInstallmentDate { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool CancelledOrRejected { get; set; }
        public DateTime? CancelledOrRejectedDate { get; set; }
        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public ProviderProduct? ProviderProduct { get; set; }
        public ProductBaseCommission? ProductBaseCommission { get; set; }
        public ICollection<InstallmentSchedule> InstallmentSchedules { get; set; } = new List<InstallmentSchedule>();
    }
}

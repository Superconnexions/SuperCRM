using SuperCRM.Domain.Enums;

namespace SuperCRM.Domain.Entities
{
    public class ProductBaseCommission
    {
        public Guid ProductBaseCommissionId { get; set; }
        public Guid ProductId { get; set; }
        public CommissionType CommissionType { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? Percentage { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public Product? Product { get; set; }
        //public ApplicationUser? CreatedByUser { get; set; }
        //public ApplicationUser? UpdatedByUser { get; set; }
        public ICollection<ProductBaseCommissionHistory> Histories { get; set; } = new List<ProductBaseCommissionHistory>();
    }
}

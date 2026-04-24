using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.DTOs.ProductBaseCommissions
{
    public class ProductBaseCommissionFormLookupDto
    {
        public List<ProductBaseCommissionLookupItemDto> Products { get; set; } = new();
        public List<ProductBaseCommissionLookupItemDto> CommissionTypes { get; set; } = new();

        public static List<ProductBaseCommissionLookupItemDto> BuildCommissionTypeOptions()
        {
            return Enum.GetValues(typeof(CommissionType))
                .Cast<CommissionType>()
                .Select(x => new ProductBaseCommissionLookupItemDto
                {
                    Value = ((byte)x).ToString(),
                    Text = x.ToString()
                })
                .ToList();
        }
    }
}

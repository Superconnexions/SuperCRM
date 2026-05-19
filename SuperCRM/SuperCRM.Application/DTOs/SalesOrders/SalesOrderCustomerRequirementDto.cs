namespace SuperCRM.Application.DTOs.SalesOrders
{
    public class SalesOrderCustomerRequirementDto
    {
        public bool HasResidentialProduct { get; set; }
        public bool HasBusinessProduct { get; set; }
        public bool HasMixedBusinessResidential { get; set; }
        public bool IsResidentialOnly => HasResidentialProduct && !HasBusinessProduct;
        public bool IsBusinessFlow => HasBusinessProduct || HasMixedBusinessResidential;
        public bool RequiresBankInformation { get; set; }
        public string ScenarioName { get; set; } = string.Empty;
    }
}

namespace SuperCRM.Application.DTOs.ProviderProducts
{
    public class UpdateProviderProductDto
    {
        public Guid ProviderProductId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

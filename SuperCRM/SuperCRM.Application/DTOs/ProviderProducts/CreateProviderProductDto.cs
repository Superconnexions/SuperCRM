namespace SuperCRM.Application.DTOs.ProviderProducts
{
    public class CreateProviderProductDto
    {
        public Guid ProviderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}

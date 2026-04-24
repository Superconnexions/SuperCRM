namespace SuperCRM.Application.DTOs.ProviderProducts
{
    public class ProviderProductFormLookupDto
    {
        public List<ProviderProductLookupItemDto> Providers { get; set; } = new();
        public List<ProviderProductLookupItemDto> Products { get; set; } = new();
    }
}

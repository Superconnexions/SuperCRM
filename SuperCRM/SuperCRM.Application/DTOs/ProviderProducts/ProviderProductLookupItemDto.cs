namespace SuperCRM.Application.DTOs.ProviderProducts
{
    public class ProviderProductLookupItemDto
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
    }
}

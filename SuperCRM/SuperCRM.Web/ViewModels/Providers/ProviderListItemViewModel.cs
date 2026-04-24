namespace SuperCRM.Web.ViewModels.Providers;

public class ProviderListItemViewModel
{
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; }

    public string? ProviderUrl { get; set; }
    public string? ProviderAddress { get; set; }
}
namespace SuperCRM.Web.Helpers
{
    public interface IAppSettingsHelper
    {
        string GetSalesOrderCCEmail();
        string GetSalesOrderBCCEmail();
    }

    public class AppSettingsHelper : IAppSettingsHelper
    {
        private readonly IConfiguration _configuration;

        public AppSettingsHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetSalesOrderCCEmail()
        {
            return NormalizeEmailList(_configuration["SalesOrderEmail:SalesOrderCCEmail"]) ?? string.Empty;

        }

        public string GetSalesOrderBCCEmail()
        {
            return NormalizeEmailList(_configuration["SalesOrderEmail:SalesOrderBCCEmail"] ) ?? string.Empty;
        }

        private static string NormalizeEmailList(string? emails)
        {
            if (string.IsNullOrWhiteSpace(emails))
                return string.Empty;

            return string.Join(",",
                emails.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(x => x.Trim())
                      .Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }
}
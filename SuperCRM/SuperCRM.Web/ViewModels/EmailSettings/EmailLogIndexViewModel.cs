using SuperCRM.Application.DTOs.Common;
using SuperCRM.Application.DTOs.EmailSettings;

namespace SuperCRM.Web.ViewModels.EmailSettings
{
    public class EmailLogIndexViewModel
    {
        public bool? IsSent { get; set; }
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public PagedResultDto<EmailLogListDto> Logs { get; set; } = new();
    }
}

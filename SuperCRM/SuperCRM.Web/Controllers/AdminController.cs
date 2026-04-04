using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Shared;

namespace SuperCRM.Web.Controllers
{
    
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SuperCRM.Web.Models;
using System.Diagnostics;

namespace SuperCRM.Web.Controllers
{
    public class AgentController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public AgentController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult MyAccount()
        {
            return View("MyAccount","");
        }

        public IActionResult MyCustomer()
        {
            return View("MyCustomer","");
        }

        public IActionResult CreateCustomer()
        {
            return View("CreateCustomer","");
        }


        public IActionResult AgentCommission()
        {
            return View("AgentCommission","");
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RentalApp.MvcApp.Models;
using RentalApp.MvcApp.Models.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace RentalApp.MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger)
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
    }
}

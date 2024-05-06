using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pratice1.MvcApp.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Pratice1.MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Signin(LoginRequestModel requestModel)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("Dbconnection"));
                conn.Open();
                string query = @"SELECT [UserID]
      ,[Email]
      ,[Password]
      ,[IsActive]
  FROM [dbo].[Users] WHERE Email = @Email AND Password = @Password AND IsActive = @IsActive";
                SqlCommand cmd = new(query,conn);
                cmd.Parameters.AddWithValue("@Email", requestModel.Email);
                cmd.Parameters.AddWithValue("@Password", requestModel.Password);
                cmd.Parameters.AddWithValue("@IsActive", true);
                SqlDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                conn.Close();

                if(dt.Rows.Count > 0)
                {
                    TempData["successMessage"] = "Login Successful";
                }
                else
                {
                    TempData["fail"] = "Login fail";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

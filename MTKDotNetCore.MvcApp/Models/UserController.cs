using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace MTKDotNetCore.MvcApp.Models
{
    public class UserController : Controller
    {
        private readonly IConfiguration? _configuration;

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginRequestModel requestModel)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"SELECT [UserID]
      ,[Email]
      ,[Password]
      ,[IsActive]
  FROM [dbo].[Users] WHERE Email = @Email AND Password = @Password AND Active = @Active";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@Email", requestModel.Email);
                cmd.Parameters.AddWithValue("@Password", requestModel.Password);
                cmd.Parameters.AddWithValue("@Active", true);
                SqlDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                conn.Close();

                if (dt.Rows.Count > 0)
                {
                    TempData["successMessage"] = "Login Successful!";
                }
                else
                {
                    TempData["fail"] = "Login Fail!";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}

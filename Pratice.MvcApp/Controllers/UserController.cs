using Microsoft.AspNetCore.Mvc;

namespace Pratice.MvcApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Login()
        {
            try
            {
                SqlConnection conn = new(configuration.ConnectionString("DbConnection"));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

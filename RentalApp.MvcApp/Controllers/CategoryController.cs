using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RentalApp.MvcApp.Models.Entity;
using RentalApp.MvcApp.Models.Request;
using System.Data;
using System.Data.SqlClient;

namespace RentalApp.MvcApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IConfiguration _configuration;

        public CategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult CategoryManagement()
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"SELECT [CategoryId]
      ,[CategoryName]
      ,[IsActive]
  FROM [dbo].[Category] WHERE IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@IsActive", true);
                SqlDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                conn.Close();
                string jsonStr = JsonConvert.SerializeObject(dt);
                List<CategoryDataModel> lst = JsonConvert.DeserializeObject<List<CategoryDataModel>>(jsonStr)!;

                return View(lst);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public IActionResult Delete(long id)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"Update Category SET IsActive = @IsActive WHERE CategoryId = @CategoryId";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@CategoryId", id);
                cmd.Parameters.AddWithValue("@IsActive", false);
                int result = cmd.ExecuteNonQuery();
                conn.Close();

                if (result > 0)
                {
                    TempData["success"] = "Deleting Successful";
                }
                else
                {
                    TempData["error"] = "Deleting Fail!";
                }

                return RedirectToAction("CategoryManagement");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IActionResult CreateCategory()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Create(CreateCategoryRequestModel requestModel)
        {
            try
            {
                if (IsCategoryNameDuplicate(requestModel.CategoryName))
                {
                    TempData["error"] = "Category Name alreay exits!";
                    return RedirectToAction("CategoryManagement");
                }

                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"INSERT INTO Category(CategoryName,IsActive) VALUES(@CategoryName,@IsActive)";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@CategoryName", requestModel.CategoryName);
                cmd.Parameters.AddWithValue("@IsActive", true);
                int result = cmd.ExecuteNonQuery();
                conn.Close();

                if (result > 0)
                {
                    TempData["success"] = "Creating Successful";
                }
                else
                {
                    TempData["error"] = "Creating Fail!";
                }
                return RedirectToAction("CategoryManagement");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool IsCategoryNameDuplicate(string categoryName)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"SELECT [CategoryId]
      ,[CategoryName]
      ,[IsActive]
  FROM [dbo].[Category] WHERE CategoryName = @CategoryName AND IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                cmd.Parameters.AddWithValue("@IsActive", true);
                SqlDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                conn.Close();

                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IActionResult EditCategory(long id)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"SELECT [CategoryId]
      ,[CategoryName]
      ,[IsActive]
  FROM [dbo].[Category] WHERE CategoryId = @CategoryId AND IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@CategoryId", id);
                cmd.Parameters.AddWithValue("@IsActive", true);
                SqlDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                conn.Close();
                string jsonStr = JsonConvert.SerializeObject(dt);
                List<CategoryDataModel> lst = JsonConvert.DeserializeObject<List<CategoryDataModel>>(jsonStr)!;

                return View(lst);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Update(UpdateCategoryRequestModel requestModel)
        {
            try
            {
                if (IsCategoryNameDuplicate(requestModel.CategoryName))
                {
                    TempData["error"] = "Category Name alreay exits!";
                    return RedirectToAction("CategoryManagement");
                }

                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"UPDATE Category SET CategoryName = @CategoryName 
WHERE CategoryId = @CategoryId AND IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@CategoryId", requestModel.CategoryId);
                cmd.Parameters.AddWithValue("@CategoryName", requestModel.CategoryName);
                cmd.Parameters.AddWithValue("@IsActive", true);
                int result = cmd.ExecuteNonQuery();
                conn.Close();
                if (result > 0)
                {
                    TempData["success"] = "Updating Successful";
                }
                else
                {
                    TempData["error"] = "Updating fail";
                }
                return RedirectToAction("CategoryManagement");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

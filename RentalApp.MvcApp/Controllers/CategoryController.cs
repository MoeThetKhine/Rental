using Microsoft.AspNetCore.Mvc;
using RentalApp.MvcApp.Models.Entity;
using RentalApp.MvcApp.Models.Request;
using RentalApp.MvcApp.Services;
using System.Data;
using System.Data.SqlClient;

namespace RentalApp.MvcApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AdoDotNetService _adoDotNetService;

        public CategoryController(AdoDotNetService adoDotNetService)
        {
            _adoDotNetService = adoDotNetService;
        }

        public IActionResult CategoryManagement()
        {
            try
            {
                string query = @"SELECT [CategoryId]
      ,[CategoryName]
      ,[IsActive]
  FROM [dbo].[Category] WHERE IsActive = @IsActive";
                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@IsActive",true)
                };
                List<CategoryDataModel> lst = _adoDotNetService.Query<CategoryDataModel>(query, parameters.ToArray());

                return View(lst);
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
                    TempData["error"] = "Category Name already exits!";
                    return RedirectToAction("CategoryManagement");
                }

                string query = @"INSERT INTO Category (CategoryName,IsActive) VALUES (@CategoryName,@IsActive)";
                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@CategoryName", requestModel.CategoryName),
                    new SqlParameter("@IsActive", true)
                };
                int result = _adoDotNetService.Execute(query, parameters.ToArray());

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

        public IActionResult EditCategory(long id)
        {
            try
            {
                string query = @"SELECT [CategoryId]
      ,[CategoryName]
      ,[IsActive]
  FROM [dbo].[Category] WHERE CategoryId = @CategoryId AND IsActive = @IsActive";
                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@CategoryId",id),
                    new SqlParameter("@IsActive",true)
                };
                List<CategoryDataModel> lst = _adoDotNetService.Query<CategoryDataModel>(query, parameters.ToArray());

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

                string query = @"UPDATE Category SET CategoryName = @CategoryName 
WHERE CategoryId = @CategoryId AND IsActive = @IsActive";
                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@CategoryId", requestModel.CategoryId),
                    new SqlParameter("@CategoryName", requestModel.CategoryName),
                    new SqlParameter("@IsActive", true),
                };
                int result = _adoDotNetService.Execute(query, parameters.ToArray());

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

        public IActionResult Delete(long id)
        {
            try
            {
                string query = @"Update Category SET IsActive = @IsActive WHERE CategoryId = @CategoryId";
                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@CategoryId", id),
                    new SqlParameter("@IsActive", false)
                };
                int result = _adoDotNetService.Execute(query, parameters.ToArray());

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

        private bool IsCategoryNameDuplicate(string categoryName)
        {
            try
            {
                string query = @"SELECT [CategoryId]
      ,[CategoryName]
      ,[IsActive]
  FROM [dbo].[Category] WHERE CategoryName = @CategoryName AND IsActive = @IsActive";
                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@CategoryName",categoryName),
                    new SqlParameter("@IsActive",true)
                };
                DataTable dt = _adoDotNetService.QueryFirstOrDefault(query, parameters.ToArray());

                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
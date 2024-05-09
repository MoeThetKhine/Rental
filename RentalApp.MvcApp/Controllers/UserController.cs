using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RentalApp.MvcApp.Models.Entity;
using RentalApp.MvcApp.Models.Request;
using RentalApp.MvcApp.Models.ResponseModel;
using RentalApp.MvcApp.Services;
using System.Data;
using System.Data.SqlClient;

namespace RentalApp.MvcApp.Controllers
{
    public class UserController : Controller

    {
        private readonly IConfiguration _configuration;
        private readonly AdoDotNetService _adoDotNetService;

        public UserController(IConfiguration configuration, AdoDotNetService adoDotNetService)
        {
            _configuration = configuration;
            _adoDotNetService = adoDotNetService;
        }

        [ActionName("LoginPage")]
        public IActionResult GoToLogin()
        {
            return View();
        }
        public IActionResult UserManagement()
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"SELECT [UserId]
      ,[UserName]
      ,[Address]
      ,[PhoneNumber]
      ,[UserRole]
      ,[IsActive]
  FROM [dbo].[Customer] WHERE IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@IsActive", true);
                SqlDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                conn.Close();

                string jsonStr = JsonConvert.SerializeObject(dt);
                List<UserResponseModel> lst = JsonConvert.DeserializeObject<List<UserResponseModel>>(jsonStr)!;
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
                string query = @"Update Customer SET IsActive = @IsActive WHERE UserId = @UserId";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@UserId", id);
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
                return RedirectToAction("UserManagement");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Update(UpdateUserRequestModel requestModel)
        {
            string cleanedPhoneNumber = new(requestModel.PhoneNumber.Where(char.IsDigit).ToArray());

            try
            {
                if (string.IsNullOrEmpty(requestModel.PhoneNumber))
                {
                    TempData["error"] = "Please fill your Phone Number";
                    return RedirectToAction("UserManagement");
                }
                else if ((requestModel.PhoneNumber.Length < 11 || requestModel.PhoneNumber.Length > 11))
                {
                    TempData["error"] = "Phone Number should be 11 numbers";
                    return RedirectToAction("UserManagement");
                }
                else if (cleanedPhoneNumber.Length != 11)
                {
                    TempData["error"] = "Phone Number should be 11 numbers without any space";
                    return RedirectToAction("UserManagement");
                }

                if (IsPhoneNumberDuplicate(requestModel.UserId, requestModel.PhoneNumber))
                {
                    TempData["error"] = "User with this Phone Number already exits";
                    return RedirectToAction("UserManagement");
                }

                string noChangesCaseQuery = @"SELECT * FROM Customer WHERE UserName = @UserName
AND Address = @Address AND PhoneNumber = @PhoneNumber WHERE UserId = @UserId AND IsActive = @IsActive";
                List<SqlParameter> parameters = new()
                {
                    new SqlParameter("@UserName", requestModel.UserName),
                    new SqlParameter("@Address", requestModel.Address),
                    new SqlParameter("@PhoneNumber", requestModel.PhoneNumber),
                    new SqlParameter("@UserId", requestModel.UserId),
                    new SqlParameter("@IsActive", true)
                };
                DataTable dt = _adoDotNetService.QueryFirstOrDefault(noChangesCaseQuery, parameters.ToArray());
                if (dt.Rows.Count > 0)
                {
                    TempData["warning"] = "No Changes...";
                    return RedirectToAction("UserManagement");
                }

                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"UPDATE Customer SET UserName = @UserName, Address = @Address, PhoneNumber = @PhoneNumber 
WHERE UserId = @UserId AND IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@UserId", requestModel.UserId);
                cmd.Parameters.AddWithValue("@UserName", requestModel.UserName);
                cmd.Parameters.AddWithValue("@Address", requestModel.Address);
                cmd.Parameters.AddWithValue("@PhoneNumber", requestModel.PhoneNumber);
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
                return RedirectToAction("UserManagement");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Login(UserDataModel dataModel)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"
    SELECT [UserId]
          ,[UserName]
          ,[Address]
          ,[PhoneNumber]
          ,[UserRole]
          ,[IsActive]
    FROM [dbo].[Customer] 
    WHERE LTRIM(RTRIM(UserName)) = LTRIM(RTRIM(@UserName)) 
          AND Password = @Password 
          AND IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@UserName", dataModel.UserName);
                cmd.Parameters.AddWithValue("@Password", dataModel.Password);
                cmd.Parameters.AddWithValue("@IsActive", true);
                SqlDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                conn.Close();

                //if (dt.Rows.Count > 0)
                //{
                //    TempData["success"] = "Login Successful";
                //}
                //else
                //{
                //    TempData["error"] = "Login fail";
                //}
                //return RedirectToAction("Login");

                if (dt.Rows.Count == 0)
                {
                    TempData["error"] = "Login fail";
                    return RedirectToAction("LoginPage");
                }

                return RedirectToAction("UserManagement");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IActionResult EditUser(long id)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"SELECT [UserId]
      ,[UserName]
      ,[Address]
      ,[PhoneNumber]
      ,[UserRole]
      ,[IsActive]
  FROM [dbo].[Customer] WHERE UserId = @UserId AND IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@UserId", id);
                cmd.Parameters.AddWithValue("@IsActive", true);
                SqlDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                conn.Close();

                string jsonStr = JsonConvert.SerializeObject(dt);
                List<UserDataModel> User = JsonConvert.DeserializeObject<List<UserDataModel>>(jsonStr)!;

                return View(User);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [ActionName("CreateUser")]
        public IActionResult GoToCreateUserPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserDataModel dataModel)
        {
            string cleanedPhoneNumber = new string(dataModel.PhoneNumber.Where(char.IsDigit).ToArray());

            try
            {
                if (string.IsNullOrEmpty(dataModel.PhoneNumber))
                {
                    TempData["error"] = "Please fill your Phone Number";
                    return RedirectToAction("UserManagement");
                }
                else if ((dataModel.PhoneNumber.Length < 11 || dataModel.PhoneNumber.Length > 11))
                {
                    TempData["error"] = "Phone Number should be 11 numbers";
                    return RedirectToAction("UserManagement");
                }
                else if (cleanedPhoneNumber.Length != 11)
                {
                    TempData["error"] = "Phone Number should be 11 numbers without any space";
                    return RedirectToAction("UserManagement");
                }
                if (IsPhoneNumberDuplicate(dataModel.PhoneNumber))
                {
                    TempData["error"] = "User with this Phone Number already exits";
                    return RedirectToAction("UserManagement");
                }

                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"INSERT INTO Customer (UserName,Address,PhoneNumber,UserRole,IsActive)
VALUES(@UserName,@Address,@PhoneNumber,@UserRole,@IsActive)";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@UserName", dataModel.UserName);
                cmd.Parameters.AddWithValue("@Address", dataModel.Address);
                cmd.Parameters.AddWithValue("@PhoneNumber", dataModel.PhoneNumber);
                cmd.Parameters.AddWithValue("@UserRole", "User");
                cmd.Parameters.AddWithValue("@IsActive", true);
                int result = cmd.ExecuteNonQuery();
                conn.Close();

                if (result > 0)
                {
                    TempData["success"] = "Create Successfully";
                }
                else
                {
                    TempData["error"] = "Create Fail!";
                }
                return RedirectToAction("UserManagement");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private bool IsPhoneNumberDuplicate(long userID, string phoneNumber)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"SELECT [UserId]
      ,[UserName]
      ,[Password] 
      ,[Address]
      ,[PhoneNumber]
      ,[UserRole]
      ,[IsActive]
  FROM [dbo].[Customer] WHERE PhoneNumber = @PhoneNumber AND IsActive = @IsActive AND UserId != @UserId"; // 09773871112
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@UserId", userID);
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
        private bool IsPhoneNumberDuplicate(string phoneNumber)
        {
            try
            {
                SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
                conn.Open();
                string query = @"SELECT [UserId]
      ,[UserName]
      ,[Password] 
      ,[Address]
      ,[PhoneNumber]
      ,[UserRole]
      ,[IsActive]
  FROM [dbo].[Customer] WHERE PhoneNumber = @PhoneNumber AND IsActive = @IsActive";
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
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
    }
}

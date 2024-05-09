using System.ComponentModel.DataAnnotations;

namespace RentalApp.MvcApp.Models.ResponseModel
{
    public class UserResponseModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        [Phone]
        [MaxLength(11)]
        [MinLength(11)]
        public string PhoneNumber { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
    }
}

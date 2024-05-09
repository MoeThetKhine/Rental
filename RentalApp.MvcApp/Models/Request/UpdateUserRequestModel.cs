using System.ComponentModel.DataAnnotations;

namespace RentalApp.MvcApp.Models.Request
{
    public class UpdateUserRequestModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
       
        
    }
}

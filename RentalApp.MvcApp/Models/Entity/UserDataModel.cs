using System.ComponentModel.DataAnnotations;

namespace RentalApp.MvcApp.Models.Entity
{
    public class UserDataModel
    {
        public long UserId { get; set; }   
        public string UserName { get; set; }   
        public string Password {  get; set; }
        public string Address { get; set; }
        
        public string PhoneNumber {  get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
    }
}

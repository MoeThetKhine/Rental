namespace RentalApp.MvcApp.Models.Request;

public class UpdateUserRequestModel
{
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}

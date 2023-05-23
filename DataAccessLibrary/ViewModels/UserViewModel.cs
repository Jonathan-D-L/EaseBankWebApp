using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.ViewModels;

public class UserViewModel
{
    private const string MsgEmail = "Please enter a valid email.";
    private const string MsgPhoneNumber = "Please enter a valid phone number.";
    private const string MsgRole = "Please select Role.";
    public string Id { get; set; }
    public string UserName { get; set; }

    [Required(ErrorMessage = MsgEmail)]
    [EmailAddress(ErrorMessage = MsgEmail)]
    public string? Email { get; set; }

    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{2,4})\)?[-. ]?([0-9]{2,4})[-. ]?([0-9]{2,4})[-. ]?([0-9]{2,4})$", ErrorMessage = MsgPhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = MsgRole)]
    public IList<string>? Roles { get; set; }
}

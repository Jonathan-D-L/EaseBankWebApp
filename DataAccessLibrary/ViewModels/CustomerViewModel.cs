using System.ComponentModel.DataAnnotations;
using DataAccessLibrary.Infrastructure.Validation;

namespace DataAccessLibrary.ViewModels;

public class CustomerViewModel
{
    private const string MsgFirstName = "Please enter first name.";
    private const string MsgLastName = "Please enter last name.";
    private const string MsgGender = "Please select gender.";
    private const string MsgStreetAddress = "Please enter a valid street address.";
    private const string MsgCity = "Please enter a valid city.";
    private const string MsgZip = "Please enter a valid zip.";
    private const string MsgCountry = "Please select a country.";
    private const string MsgPhoneNumber = "Please enter a valid phone number.";
    private const string MsgEmail = "Please enter a valid email.";
    public int CustomerId { get; set; }


    [MaxLength(20, ErrorMessage = MsgFirstName)]
    [MinLength(2, ErrorMessage = MsgFirstName)]
    [Required(ErrorMessage = MsgFirstName)]
    public string GivenName { get; set; }


    [MaxLength(20, ErrorMessage = MsgLastName)]
    [MinLength(2, ErrorMessage = MsgLastName)]
    [Required(ErrorMessage = MsgLastName)]
    public string SurName { get; set; } 


    [Required(ErrorMessage = MsgGender)]
    public string Gender { get; set; }


    [MaxLength(50, ErrorMessage = MsgStreetAddress)]
    [MinLength(1, ErrorMessage = MsgStreetAddress)]
    [Required(ErrorMessage = MsgStreetAddress)]
    public string StreetAddress { get; set; }


    [MaxLength(50, ErrorMessage = MsgCity)]
    [MinLength(1, ErrorMessage = MsgCity)]
    [Required(ErrorMessage = MsgCity)]
    public string City { get; set; }


    [RegularExpression(@"^[a-z0-9][a-z0-9\- ]{0,10}[a-z0-9]$", ErrorMessage = MsgZip)]
    public string Zipcode { get; set; }


    [MaxLength(50, ErrorMessage = MsgCountry)]
    [MinLength(1, ErrorMessage = MsgCountry)]
    [Required(ErrorMessage = MsgCountry)]
    public string Country { get; set; }
    public string CountryCode { get; set; }

    [Date]
    public DateTime? Birthday { get; set; }
    public string? NationalId { get; set; }
    public string? Telephonecountrycode { get; set; }

    [DataType(DataType.PhoneNumber)]
    [Required(ErrorMessage = MsgPhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{2,4})\)?[-. ]?([0-9]{2,4})[-. ]?([0-9]{2,4})[-. ]?([0-9]{2,4})$", ErrorMessage = MsgPhoneNumber)]
    public string? Telephonenumber { get; set; }

    [Required(ErrorMessage = MsgEmail)]
    public string? Emailaddress { get; set; }
}

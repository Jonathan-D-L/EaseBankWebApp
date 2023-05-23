using ServicesLibrary;

namespace BankWebAppApi.User;

public class UserCredentials
{
    private readonly ICustomerService _customerService;

    public UserCredentials(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public List<UserModel> GetUsers()
    {
        var users = new List<UserModel>()
        {
            new UserModel() // Full read/write access
            {
                UserName = "richard_admin",
                UserId = "",
                EmailAddress = "richard_admin@email.se",
                Password = "passwordAdmin",
                GivenName = "Richard",
                SurName = "chalk",
                Role = "Admin",
            },
        };
        var getUsers = _customerService.GetCustomers();
        foreach (var user in getUsers)
        {
            users.Add(new UserModel
            {
                UserName = user.Givenname.ToLower(),
                UserId = user.CustomerId.ToString(),
                EmailAddress = user.Emailaddress,
                Password = user.CustomerId.ToString(),
                GivenName = user.Givenname,
                SurName = user.Surname,
                Role = "User",
            });
        }
        return users;
    }
}
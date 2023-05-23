namespace DataAccessLibrary.ViewModels;


public class Top10CustomerViewModel
{
    public int CustomerId { get; set; }
    public string GivenName { get; set; }
    public string SurName { get; set; }
    public string CountryCode { get; set; }
    public decimal Balance { get; set; }
}

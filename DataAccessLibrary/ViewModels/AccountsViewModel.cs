namespace DataAccessLibrary.ViewModels;

public class AccountsViewModel
{
    public int AccountId { get; set; }
    public string Frequency { get; set; }
    public DateTime Created { get; set; }
    public decimal Balance { get; set; }
    public string Type { get; set; }
}
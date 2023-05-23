namespace DataAccessLibrary.ViewModels;

public class PermenentOrderViewModel
{
    public int OrderId { get; set; }
    public int AccountId { get; set; }
    public string BankTo { get; set; }
    public string AccountTo { get; set; }
    public decimal? Amount { get; set; }
    public string Symbol { get; set; }
}
namespace DataAccessLibrary.ViewModels;

public class LoansViewModel
{
    public int LoanId { get; set; }
    public int AccountId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int Duration { get; set; }
    public decimal Payments { get; set; }
    public string Status { get; set; }
}
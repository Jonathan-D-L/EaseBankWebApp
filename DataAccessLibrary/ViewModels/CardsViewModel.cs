namespace DataAccessLibrary.ViewModels;

public class CardsViewModel
{
    public int CardId { get; set; }
    public int DispositionId { get; set; }
    public string Type { get; set; }
    public DateTime Issued { get; set; }
    public string Cctype { get; set; }
    public string Ccnumber { get; set; }
    public string Cvv2 { get; set; }
    public string ExpM { get; set; }
    public int ExpY { get; set; }

}
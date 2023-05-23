using DataAccessLibrary.Models;

namespace ServicesLibrary
{
    public interface IDispositionService
    {
       IQueryable<Disposition> GetDispositions(string countryCode);
      
       List<Disposition> GetAccountAndHolderName(string q);
    }
}

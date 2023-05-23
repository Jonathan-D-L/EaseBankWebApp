using DataAccessLibrary.Infrastructure.Paging;
using DataAccessLibrary.Models;

namespace ServicesLibrary
{
    public interface ICardService
    {
        int GetCardsCount(int customerId);

        PagedResult<Card> GetCards(int customerId, string sortColumn, string sortOrder, int page, string q);
    }
}

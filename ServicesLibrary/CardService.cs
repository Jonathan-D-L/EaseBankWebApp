using DataAccessLibrary.Infrastructure.Paging;
using DataAccessLibrary.Models;
using UtilityLibrary;

namespace ServicesLibrary
{
    public class CardService : ICardService
    {
        private readonly BankAppDataContext _dbContext;
        public CardService(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int GetCardsCount(int customerId)
        {
            var cards = _dbContext.Cards
                .Where(c => c.DispositionId == customerId)
                .AsQueryable();
            return cards.Count();
        }

        public PagedResult<Card> GetCards(int customerId, string sortColumn, string sortOrder, int page, string q)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                sortColumn = "OrderId";
                sortOrder = "CardId";
            }
            var cards = _dbContext.Cards
                .Where(c => c.DispositionId == customerId)
                .SortColumn(sortColumn, sortOrder);
            if (!string.IsNullOrWhiteSpace(q))
            {
                cards = Search.ScopedDataSearch(cards, q).AsQueryable();
            }
            return cards.GetPaged(page, 10);
        }
    }
}

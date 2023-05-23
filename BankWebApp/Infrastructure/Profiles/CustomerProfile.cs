using AutoMapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;

namespace BankWebApp.Infrastructure.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerViewModel>().ReverseMap();
            CreateMap<Account, AccountsViewModel>()
                .ForMember(dest => dest.Type, opt => opt
                    .MapFrom(src => src.Dispositions.First().Type))
                .ReverseMap();
            CreateMap<Card, CardsViewModel>().ReverseMap();
            CreateMap<Transaction, TransactionsViewModel>().ReverseMap();
            CreateMap<PermenentOrder, PermenentOrderViewModel>().ReverseMap();
            CreateMap<Loan, LoansViewModel>().ReverseMap();
        }
    }
}

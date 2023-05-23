using AutoMapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;

namespace BankWebApp.Infrastructure.Profiles
{
    public class Top10Profile : Profile
    {
        public Top10Profile()
        {
            CreateMap<Account, Top10CustomerViewModel>().ReverseMap();
            CreateMap<Disposition, Top10CustomerViewModel>()
                .IncludeMembers(src => src.Customer)
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.CustomerId))
                .IncludeMembers(src => src.Customer)
                .ForMember(dest => dest.GivenName, opt => opt.MapFrom(src => src.Customer.Givenname))
                .IncludeMembers(src => src.Customer)
                .ForMember(dest => dest.SurName, opt => opt.MapFrom(src => src.Customer.Surname))
                .IncludeMembers(src => src.Customer)
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.Customer.CountryCode))
                .IncludeMembers(src => src.Account)
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Account.Balance))
                .ReverseMap();
        }
    }
}

using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class OverdraftDIProfile : Profile
    {
        public OverdraftDIProfile()
        {
            CreateMap<Overdraft, OverdraftDI>().ForMember(dest => dest.EmployeeDI, opt => opt.MapFrom(src => src.Employee))
               .ForMember(dest => dest.PeriodDetailDI, opt => opt.MapFrom(src => src.PeriodDetail))
               .ForMember(dest => dest.OverdraftDetailDIs, opt => opt.MapFrom(src => src.OverdraftDetails))
               .ReverseMap().ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.EmployeeDI))
               .ForMember(dest => dest.PeriodDetail, opt => opt.MapFrom(src => src.PeriodDetailDI))
               .ForMember(dest => dest.OverdraftDetails, opt => opt.MapFrom(src => src.OverdraftDetailDIs));
            
            //CreateMap<Overdraft, OverdraftDI>().ReverseMap();
        }
    }
}

using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class EmployeeDIProfile : Profile
    {
        public EmployeeDIProfile()
        {
            CreateMap<Employee, EmployeeDI>().ForMember(dest => dest.WorkshiftDI, opt => opt.MapFrom(src => src.Workshift))
               .ReverseMap()
               .ForMember(dest => dest.Workshift, opt => opt.MapFrom(src => src.WorkshiftDI));
        }
    }
}

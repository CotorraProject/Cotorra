using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
             CreateMap<Department, DepartmentDTO>().ForMember(dest=> dest.AreaDTO, opt=> opt.MapFrom(src=> src.Area)).
                ReverseMap().ForMember(dest => dest.Area, opt=> opt.MapFrom(src => src.AreaDTO)); 
        }
    }
}

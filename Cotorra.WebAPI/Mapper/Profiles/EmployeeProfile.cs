using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO.Catalogs;

namespace Cotorra.WebAPI
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        { 
            CreateMap<Employee, EmployeeDTO>().ForMember(dest => dest.DepartmentDTO, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest=> dest.JobPositionDTO, opt => opt.MapFrom(src=> src.JobPosition))
                .ReverseMap().ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.DepartmentDTO))
                .ForMember(dest => dest.JobPosition, opt => opt.MapFrom(src => src.JobPositionDTO));
        }
    }
}

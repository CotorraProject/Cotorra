using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class MinimumSalaryProfile : Profile
    {
        public MinimumSalaryProfile()
        {
             CreateMap<MinimunSalary, MinimumSalaryDTO>().ReverseMap();
        }
    }
}

using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class MonthlyIncomeTaxProfile : Profile
    {
        public MonthlyIncomeTaxProfile()
        {
             CreateMap<MonthlyIncomeTax, MonthlyIncomeTaxDTO>().ReverseMap();
        }
    }
}

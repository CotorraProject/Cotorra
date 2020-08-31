using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class AnualIncomeTaxProfile : Profile
    {
        public AnualIncomeTaxProfile()
        {
             CreateMap<AnualIncomeTax, AnualIncomeTaxDTO>().ReverseMap();
        }
    }
}

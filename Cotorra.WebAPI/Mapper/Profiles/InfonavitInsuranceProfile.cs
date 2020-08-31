using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class InfonavitInsuranceProfile : Profile
    {
        public InfonavitInsuranceProfile()
        {
             CreateMap<InfonavitInsurance, InfonavitInsuranceDTO>().ReverseMap();
        }
    }
}

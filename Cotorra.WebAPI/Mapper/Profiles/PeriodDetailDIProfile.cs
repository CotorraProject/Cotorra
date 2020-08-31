using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class PeriodDetailDIProfile : Profile
    {
        public PeriodDetailDIProfile()
        {
             CreateMap<PeriodDetail, PeriodDetailDI>().ReverseMap(); 
        }
    }
}

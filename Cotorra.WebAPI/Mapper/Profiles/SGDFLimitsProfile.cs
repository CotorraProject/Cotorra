using AutoMapper;
using Cotorra.Schema; 
namespace Cotorra.WebAPI
{
    public class SGDFLimitsProfile : Profile
    {
        public SGDFLimitsProfile()
        {
             CreateMap<SGDFLimits, SGDFLimitsDTO>().ReverseMap();
        }
    }
}

using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class OverdraftDetailDIProfile : Profile
    {
        public OverdraftDetailDIProfile()
        {
            CreateMap<OverdraftDetail, OverdraftDetailDI>().ForMember(dest => dest.ConceptPaymentDI, opt => opt.MapFrom(src => src.ConceptPayment))
               .ReverseMap().ForMember(dest => dest.ConceptPayment, opt => opt.MapFrom(src => src.ConceptPaymentDI));
           
        }
    }
}

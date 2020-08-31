using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class ConceptPaymentDIProfile : Profile
    {
        public ConceptPaymentDIProfile()
        {
             CreateMap<ConceptPayment, ConceptPaymentDI>().ReverseMap();
        }
    }
}

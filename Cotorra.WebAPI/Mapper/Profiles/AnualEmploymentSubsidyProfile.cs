using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class AnualEmploymentSubsidyProfile : Profile
    {
        public AnualEmploymentSubsidyProfile()
        {
             CreateMap<AnualEmploymentSubsidy, AnualEmploymentSubsidyDTO>().ReverseMap();
        }
    }
}

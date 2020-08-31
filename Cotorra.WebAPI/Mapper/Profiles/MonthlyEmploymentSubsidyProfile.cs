using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class MonthlyEmploymentSubsidyProfile : Profile
    {
        public MonthlyEmploymentSubsidyProfile()
        {
             CreateMap<MonthlyEmploymentSubsidy, MonthlyEmploymentSubsidyDTO>().ReverseMap();
        }
    }
}

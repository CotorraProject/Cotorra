using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class JobPositionProfile : Profile
    {
        public JobPositionProfile()
        {
             CreateMap<JobPosition, JobPositionDTO>().ReverseMap();
        }
    }
}

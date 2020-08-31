using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class WorksfhiftDIProfile : Profile
    {
        public WorksfhiftDIProfile()
        {
             CreateMap<Workshift, WorkshiftDI>().ReverseMap();
        }
    }
}

using AutoMapper;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
namespace Cotorra.WebAPI
{
    public class AreaProfile : Profile
    {
        public AreaProfile()
        {
             CreateMap<Area, AreaDTO>().ReverseMap();
        }
    }
}

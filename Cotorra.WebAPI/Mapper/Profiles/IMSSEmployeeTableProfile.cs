using AutoMapper;
using Cotorra.Schema;
namespace Cotorra.WebAPI
{
    public class IMSSEmployerTableProfile : Profile
    {
        public IMSSEmployerTableProfile()
        {
             CreateMap<IMSSEmployerTable, IMSSEmployerTableDTO>().ReverseMap();
        }
    }
}

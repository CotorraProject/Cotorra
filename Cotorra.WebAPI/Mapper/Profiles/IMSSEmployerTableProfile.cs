using AutoMapper;
using Cotorra.Schema;
namespace Cotorra.WebAPI
{
    public class IMSSEmployeeTableProfile : Profile
    {
        public IMSSEmployeeTableProfile()
        {
             CreateMap<IMSSEmployeeTable, IMSSEmployeeTableDTO>().ReverseMap();
        }
    }
}

using AutoMapper; 
using Cotorra.Core.Validator; 
using Cotorra.Schema.DTO;
using System; 
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{
    
    public interface IAreasService
    {
        Task<AreaDTO> Post(Guid instanceID, Guid userID, Guid identityWorkID, AreaValidator validator, IMapper mapper,
             AreaDTO AreaDTO);

        Task<AreaDTO> Put(Guid instanceID, Guid userID, Guid identityWorkID, AreaValidator validator, IMapper mapper,
          AreaDTO AreaDTO);

        Task Delete(Guid instanceID, Guid userID, Guid identityWorkID, AreaValidator validator, IMapper mapper,
          Guid AreaID);
    }
}

using AutoMapper;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{
    public class AreasService : IAreasService
    {
        public async Task Delete(Guid instanceID, Guid userID, Guid identityWorkID, AreaValidator validator, IMapper mapper, Guid AreaID)
        {
            var mgr = new MiddlewareManager<Area>(new BaseRecordManager<Area>(), validator);
            await mgr.DeleteAsync(new List<Guid>() { AreaID }, identityWorkID);
        }

        public async Task<AreaDTO> Post(Guid instanceID, Guid userID, Guid identityWorkID, AreaValidator validator, IMapper mapper,
            AreaDTO AreaDTO)
        {
            Area dep = new Area();
            mapper.Map(AreaDTO, dep);
            dep.InstanceID = instanceID;
            dep.CompanyID = identityWorkID;

            var mgr = new MiddlewareManager<Area>(new BaseRecordManager<Area>(), validator);
            await mgr.CreateAsync(new List<Area>() { dep }, identityWorkID);
            return AreaDTO;
        }


        public async Task<AreaDTO> Put(Guid instanceID, Guid userID, Guid identityWorkID, AreaValidator validator, IMapper mapper,
          AreaDTO AreaDTO)
        {
            var mgr = new MiddlewareManager<Area>(new BaseRecordManager<Area>(), validator);
            var found =  (await mgr.FindByExpressionAsync(x=> x.ID == AreaDTO.ID, identityWorkID)).FirstOrDefault();

            if (found != null)
            {
                found.Name = AreaDTO.Name; 
                await mgr.UpdateAsync(new List<Area>() { found }, identityWorkID);
            }
            else
            {
                return await Post(instanceID, userID, identityWorkID, validator, mapper, AreaDTO);    
            }

            return AreaDTO;
        }

        
    }

}

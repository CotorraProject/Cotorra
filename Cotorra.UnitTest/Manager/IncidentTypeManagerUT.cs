using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.UnitTest.Manager
{
    public class IncidentTypeManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID) where T : BaseEntity
        {
            //Act Dependencies          
            
            var accumulatedTypes = new MemoryStorageContext().GetDefaultAccumulatedType(identityWorkId, instanceID, Guid.NewGuid());
            List <IncidentType> incidentTypes = new MemoryStorageContext().GetDefaultIncidentType(identityWorkId, instanceID, Guid.NewGuid(), accumulatedTypes);

            //AcumulatedTypes
            var middlewareManagerAccumulatedType = new MiddlewareManager<AccumulatedType>(new BaseRecordManager<AccumulatedType>(), new AccumulatedTypeValidator());
            await middlewareManagerAccumulatedType.CreateAsync(accumulatedTypes, identityWorkId);

            //IncidentTypes
            var middlewareManagerIncidentType = new MiddlewareManager<IncidentType>(new BaseRecordManager<IncidentType>(), new IncidentTypeValidator());
            await middlewareManagerIncidentType.CreateAsync(incidentTypes, identityWorkId);

            return incidentTypes as List<T>;
        }

    }
}

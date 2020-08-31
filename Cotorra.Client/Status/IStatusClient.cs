using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{

    public interface IStatusClient<T> where T : IStatusFull
    {
        Task SetActive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId);
        Task SetUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, params object[] parameters);
        Task SetInactive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId);
        Task SetStatus(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, CotorriaStatus status);
        Task UpdateAsync(IEnumerable<Guid> idsToUpdate, CotorriaStatus status, Guid identityWorkID, params object[] parameters);
    }
}

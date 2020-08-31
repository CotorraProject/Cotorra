using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Schema
{
    public interface IStatusFullValidator<T> where T : IStatusFull
    {       

        Task BeforeActivate(IEnumerable<Guid> idsToUpdate, Guid identityWorkId);

        Task AfterActivate(IEnumerable<Guid> idsToUpdate, Guid identityWorkId);    

        Task BeforeUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId);

        Task AfterUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, params object[] parameters);

        Task BeforeInactivate(IEnumerable<Guid> idsToUpdate, Guid identityWorkId);

        Task AfterInactivate(IEnumerable<Guid> idsToUpdate, Guid identityWorkId);

        string GetPersonalizedQuery(CotorriaStatus status, string keySplitted, params object[] parameters);
    }
}

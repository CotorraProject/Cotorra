using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace Cotorra.Core.Storage
{
    interface IRepositroyCotorria<T> where T : BaseEntity
    {
        Task CreateorUpdateAsync(List<T> objectsToCreate, Guid identityWorkID = default);
    }
}

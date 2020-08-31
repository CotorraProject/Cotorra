using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public interface IBaseRecordManager<T> where T : BaseEntity
    {
        void Create(List<T> lstObjects, Guid identityWorkId);
        Task CreateAsync(List<T> lstObjects, Guid identityWorkId);

        void Update(List<T> lstObjects, Guid identityWorkId);

        Task UpdateAsync(List<T> lstObjects, Guid identityWorkId);


        void Delete(List<Guid> lstObjects, Guid identityWorkId);
        Task DeleteAsync(List<Guid> lstObjects, Guid identityWorkId);


        List<T> GetByIds(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude);

        Task<List<T>> GetByIdsAsync(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude);

        List<T> Find(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude);

        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude);

        List<T> Find(string predicate, Guid identityWorkId, string[] objectsToInclude);

        Task<List<T>> FindAsync(string predicate, Guid identityWorkId, string[] objectsToInclude);

        List<T> GetAll(Guid identityWorkId, string[] objectsToInclude);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId);

        int Count(Expression<Func<T, bool>> predicate, Guid identityWorkId);

        Task<int> CountAsync(string predicate, Guid identityWorkId);

        int Count(string predicate, Guid identityWorkId);

        Task CreateorUpdateAsync(List<T> objectsToCreate, Guid identityWorkID);
    }
}

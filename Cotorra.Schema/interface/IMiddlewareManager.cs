using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cotorra.Schema
{
    public interface IMiddlewareManager<T> where T : BaseEntity
    {
        void Create(List<T> lstObjects, Guid identityWorkId);
        void Update(List<T> lstObjects, Guid identityWorkId);
        List<T> GetByIds(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude = null);
        Task<List<T>> GetByIdsAsync(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude = null);
        List<T> FindByExpression(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null);
        Task<List<T>> FindByExpressionAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null);
        List<T> Find(string predicate, Guid identityWorkId, string[] objectsToInclude = null);
        int Count(Expression<Func<T, bool>> predicate, Guid identityWorkId);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId);
        int CountAll(Guid identityWorkId, Guid instanceId);
        Task<int> CountAllAsync(Guid identityWorkId, Guid instanceId);
        List<T> GetAll(Guid identityWorkId, Guid instanceId, string[] objectsToInclude = null);
        Task<List<T>> GetAllAsync(Guid identityWorkId, Guid instanceId, string[] objectsToInclude = null);
        void Delete(List<Guid> lstGuids, Guid identityWorkId);
        Task CreateAsync(List<T> lstObjects, Guid identityWorkId);
        Task DeleteAsync(List<Guid> lstObjects, Guid identityWorkId);
        Task UpdateAsync(List<T> lstObjects, Guid identityWorkId);
        Task DeleteAllAsync(Guid identityWorkId, Guid instanceId);
        Task CreateorUpdateAsync(List<T> objectsToCreate, Guid identityWorkID);

    }
}

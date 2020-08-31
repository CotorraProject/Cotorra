using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IClient<T> where T : BaseEntity
    {
        IValidator<T> Validator { get; set; }
        Task CreateAsync(List<T> lstObjects, Guid identityWorkId);
        Task CreateAsync(List<T> lstObjects, LicenseParams licenseParams);
        Task DeleteAsync(List<Guid> lstGuids, Guid identityWorkId);
        Task DeleteAsync(List<Guid> lstGuids, LicenseParams licenseParams);

        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null);

        Task<List<T>> GetAllAsync(Guid identityWorkId, Guid instanceId, string[] objectsToInclude = null);

        Task<List<T>> GetAllAsync(string[] objectsToInclude = null);

        Task<List<T>> GetByIdsAsync(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude = null);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, Guid instanceId);

        Task<int> CountAllAsync(Guid identityWorkId, Guid instanceId);

        Task UpdateAsync(List<T> lstObjects, Guid identityWorkId);
        Task DeleteAllAsync(Guid identityWorkId, Guid instanceId);

        Task DeleteByExpresssionAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId);

    }
}

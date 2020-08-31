using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Storage;
using Cotorra.General.Core.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cotorra.General.Core
{
    public class BaseRecordGeneralManager<T> : IBaseRecordManager<T> where T : BaseEntity
    {
        #region "Attributes"
        /// <summary>
        /// IRepository
        /// </summary>
       
        #endregion

        #region "Constructor"
        /// <summary>
        /// Constructor
        /// </summary>
        public BaseRecordGeneralManager()
        {

        }
        #endregion

        /// <summary>
        /// Create new things
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public void Create(List<T> lstObjects, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            iRepository.Create(lstObjects, identityWorkId);
        }

        /// <summary>
        /// Update things
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public void Update(List<T> lstObjects, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            iRepository.Update(lstObjects, identityWorkId);
        }

        public int Count(Expression<Func<T, bool>> predicate, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return iRepository.GetCount(predicate, identityWorkID: identityWorkId);
        }
            

        public int Count(string predicate, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return iRepository.GetCount(predicate, identityWorkID: identityWorkId);
        }

        public async Task<int> CountAsync(string predicate, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return await iRepository.GetCountAsync(predicate, identityWorkID: identityWorkId);
        }
        
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return await iRepository.GetCountAsync(predicate, identityWorkID: identityWorkId);
        }

        /// <summary>
        /// Delete things
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public void Delete(List<Guid> lstObjects, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            iRepository.Delete(lstObjects, identityWorkId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstGuids"></param>
        /// <param name="identityWorkId"></param>
        /// <returns></returns>
        public List<T> GetByIds(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return iRepository.GetByIds(lstGuids, identityWorkID: identityWorkId, objectsToInclude: objectsToInclude).Select(p => p.Value).ToList();
        }

        public async Task<List<T>> GetByIdsAsync(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return (await iRepository.GetByIdsAsync(lstGuids, identityWorkID: identityWorkId, objectsToInclude: objectsToInclude)).Select(p => p.Value).ToList();
        }

        /// <summary>
        /// To find a predicate indicated in the method
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="identityWorkId"></param>
        /// <returns></returns>
        public List<T> Find(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return iRepository.Find(predicate, identityWorkID: identityWorkId, objectsToInclude: objectsToInclude).ToList();
        }      

        public List<T> Find(string predicate, Guid identityWorkId, string[] objectsToInclude)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return iRepository.Find(predicate, identityWorkID: identityWorkId, objectsToInclude: objectsToInclude).ToList();
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return await iRepository.FindAsync(predicate, identityWorkID: identityWorkId, objectsToInclude: objectsToInclude);
        }

        public async Task<List<T>> FindAsync(string predicate, Guid identityWorkId, string[] objectsToInclude)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return await iRepository.FindAsync(predicate, identityWorkID: identityWorkId, objectsToInclude: objectsToInclude);
        }

        public List<T> GetAll(Guid identityWorkId, string[] objectsToInclude)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            return iRepository.FindAll(identityWorkID: identityWorkId).ToList();
        }

        #region async 


        public async Task CreateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            await iRepository.CreateAsync(lstObjects, identityWorkId);
        }

        public async Task DeleteAsync(List<Guid> lstObjects, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            await iRepository.DeleteAsync(lstObjects, identityWorkId);
        }

        public async Task UpdateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            await iRepository.UpdateAsync(lstObjects, identityWorkId);
        }

        public async Task CreateorUpdateAsync(List<T> objectsToCreate, Guid identityWorkID)
        {
            using var context = new CotorraGeneralContext(ConnectionManager.ConfigConnectionStringGeneral);
            using var iRepository = new RepositoryStorage<T, CotorraGeneralContext>(context, true);
            await iRepository.CreateorUpdateAsync(objectsToCreate, identityWorkID);
        }

        #endregion
    }
}
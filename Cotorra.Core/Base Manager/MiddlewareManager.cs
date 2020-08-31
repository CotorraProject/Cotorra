using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace Cotorra.Core
{
    /// <summary>
    /// CRUD
    /// </summary>
    public class MiddlewareManager<T> : IMiddlewareManager<T> where T : BaseEntity
    {
        #region "Attributes"
        private readonly IBaseRecordManager<T> baseRecordManager;
        private readonly IValidator<T> validator;
        #endregion

        #region "Constructor"
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_baseRecordManager"></param>
        /// <param name="_validator"></param>
        public MiddlewareManager(IBaseRecordManager<T> _baseRecordManager, IValidator<T> _validator)
        {
            baseRecordManager = _baseRecordManager;
            validator = _validator;
            validator.MiddlewareManager = this;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_baseRecordManager"></param>
        public MiddlewareManager(IBaseRecordManager<T> _baseRecordManager)
        {
            baseRecordManager = _baseRecordManager;
        }
        #endregion

        /// <summary>
        /// Create new things
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public void Create(List<T> lstObjects, Guid identityWorkId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                validator.BeforeCreate(lstObjects);
                baseRecordManager.Create(lstObjects, identityWorkId);
                validator.AfterCreate(lstObjects);

                scope.Complete();
            }
        }

        /// <summary>
        /// Create new things
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public void Update(List<T> lstObjects, Guid identityWorkId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                validator.BeforeUpdate(lstObjects);
                baseRecordManager.Update(lstObjects, identityWorkId);
                validator.AfterUpdate(lstObjects);

                scope.Complete();
            }
        }

        /// <summary>
        /// Create new things
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public List<T> GetByIds(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude = null)
        {
            return baseRecordManager.GetByIds(lstGuids, identityWorkId, objectsToInclude);
        }

        public async Task<List<T>> GetByIdsAsync(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude = null)
        {
            return await baseRecordManager.GetByIdsAsync(lstGuids, identityWorkId, objectsToInclude);
        }

        /// <summary>
        /// Create new things
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public List<T> FindByExpression(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null)
        {
            return baseRecordManager.Find(predicate, identityWorkId, objectsToInclude);
        }

        public async Task<List<T>> FindByExpressionAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null)
        {
            return await baseRecordManager.FindAsync(predicate, identityWorkId, objectsToInclude);
        }

        public List<T> Find(string predicate, Guid identityWorkId, string[] objectsToInclude = null)
        {
            return baseRecordManager.Find(predicate, identityWorkId, objectsToInclude);
        }

        public int Count(Expression<Func<T, bool>> predicate, Guid identityWorkId)
        {
            return baseRecordManager.Count(predicate, identityWorkId);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId)
        {
            return await baseRecordManager.CountAsync(predicate, identityWorkId);
        }

        public int CountAll(Guid identityWorkId, Guid instanceId)
        {
            return baseRecordManager.Count($"InstanceID == Guid.Parse(\"{instanceId}\")", identityWorkId);
        }

        public async Task<int> CountAllAsync(Guid identityWorkId, Guid instanceId)
        {
            return await baseRecordManager.CountAsync($"InstanceID == Guid.Parse(\"{instanceId}\")", identityWorkId);
        }

        /// <summary>
        /// Get all items
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public List<T> GetAll(Guid identityWorkId, Guid instanceId, string[] objectsToInclude = null)
        {
            return baseRecordManager.Find($"InstanceID == Guid.Parse(\"{instanceId}\")", identityWorkId, objectsToInclude);
        }

        /// <summary>
        /// Get all items async
        /// </summary>
        /// <param name="identityWorkId"></param>
        /// <param name="instanceId"></param>
        /// <param name="objectsToInclude"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync(Guid identityWorkId, Guid instanceId, string[] objectsToInclude = null)
        {
            return await baseRecordManager.FindAsync($"InstanceID == Guid.Parse(\"{instanceId}\")", identityWorkId, objectsToInclude);
        }

        /// <summary>
        /// Get all items async
        /// </summary>
        /// <param name="identityWorkId"></param>
        /// <param name="instanceId"></param>
        /// <param name="objectsToInclude"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync(string[] objectsToInclude = null)
        {
            Type type = typeof(T);
            if (type.BaseType.Name != "BaseEntity")
            {
                throw new Exception("Try sending identityworkid or extend correctly");
            }
            return await baseRecordManager.FindAsync($"Active == true", Guid.Empty, objectsToInclude);
        }


        /// <summary>
        /// Create new things
        /// </summary>
        /// <param name="lstObjects"></param>
        /// <param name="identityWorkId"></param>
        public void Delete(List<Guid> lstGuids, Guid identityWorkId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                validator.BeforeDelete(lstGuids);
                baseRecordManager.Delete(lstGuids, identityWorkId);
                validator.AfterDelete(lstGuids);

                scope.Complete();
            }
        }

        #region async

        public async Task CreateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                validator.BeforeCreate(lstObjects);
                await baseRecordManager.CreateAsync(lstObjects, identityWorkId).ContinueWith(_ =>
                {
                    if (_.Exception != null)
                    {
                        throw new Exception($"Ocurrió un error al crear el registro {typeof(T).Name}", _.Exception);
                    }

                    validator.AfterCreate(lstObjects);
                });

                scope.Complete();
            }
        }

        public async Task CreateAsync(List<T> lstObjects, IParams parameters)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                validator.BeforeCreate(lstObjects);
                await baseRecordManager.CreateAsync(lstObjects, parameters.IdentityWorkID).ContinueWith(_ =>
                {
                    if (_.Exception != null)
                    {
                        throw new Exception($"Ocurrió un error al crear el registro {typeof(T).Name}", _.Exception);
                    }

                });
                await validator.AfterCreate(lstObjects, parameters);
                scope.Complete();

            }
        }

        public async Task DeleteAsync(List<Guid> lstObjects, Guid identityWorkId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var parametersToAfter = validator.BeforeDelete(lstObjects, identityWorkId);
                await baseRecordManager.DeleteAsync(lstObjects, identityWorkId).ContinueWith(_ =>
                {
                    if (_.Exception != null)
                    {
                        throw new Exception($"Ocurrió un error al borrar el registro {typeof(T).Name}", _.Exception);
                    }

                    validator.AfterDelete(lstObjects, parametersToAfter);
                });

                scope.Complete();
            }
        }

        public async Task DeleteAsync(List<Guid> lstObjects, IParams parameters)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var parametersToAfter = validator.BeforeDelete(lstObjects, parameters.IdentityWorkID);
                await baseRecordManager.DeleteAsync(lstObjects, parameters.IdentityWorkID).ContinueWith(  _ =>
                {
                    if (_.Exception != null)
                    {
                        throw new Exception($"Ocurrió un error al borrar el registro {typeof(T).Name}", _.Exception);
                    }

                });
                await validator.AfterDeleteAsync(lstObjects, parameters);

                scope.Complete();
            }
        }

        public async Task UpdateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                validator.BeforeUpdate(lstObjects);
                await baseRecordManager.UpdateAsync(lstObjects, identityWorkId).ContinueWith(_ =>
                {
                    if (_.Exception != null)
                    {
                        throw new Exception($"Ocurrió un error al actualizar el registro {typeof(T).Name}", _.Exception);
                    }

                    validator.AfterUpdate(lstObjects);
                });

                scope.Complete();
            }
        }

        public async Task DeleteAllAsync(Guid identityWorkId, Guid instanceId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var toDelete = (await GetAllAsync(identityWorkId, instanceId)).Select(x => x.ID).ToList();
                if (toDelete.Any())
                {
                    await DeleteAsync(toDelete, identityWorkId);
                }

                scope.Complete();
            }
        }

        public async Task DeleteByExpresssionAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var toDelete = (await baseRecordManager.FindAsync(predicate, identityWorkId, objectsToInclude)).Select(x => x.ID).ToList();
                if (toDelete.Any())
                {
                    await DeleteAsync(toDelete, identityWorkId);
                }

                scope.Complete();
            }
        }

        public async Task CreateorUpdateAsync(List<T> objectsToCreate, Guid identityWorkID)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //validator.BeforeUpdate(lstObjects);
                await baseRecordManager.CreateorUpdateAsync(objectsToCreate, identityWorkID).ContinueWith(_ =>
                {
                    if (_.Exception != null)
                    {
                        throw new Exception($"Ocurrió un error al actualizar el registro {typeof(T).Name}", _.Exception);
                    }

                    // validator.AfterUpdate(lstObjects);
                });

                scope.Complete();
            }
        }

        #endregion
    }
}

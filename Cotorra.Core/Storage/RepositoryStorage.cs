using CotorraNode.Common.Base.Schema;
using CotorraNode.Common.Base.Schema.Parameters.Base;
using CotorraNode.LocalStorage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Linq;
using Cotorra.Core.Extensions;
using MoreLinq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq.Dynamic;

namespace Cotorra.Core.Storage
{
    public class RepositoryStorage<T, V> : IRepository3<T>, IDisposable where T : BaseEntity
    {
        private readonly DbContext Context;

        #region "Constructor"
        public RepositoryStorage(V context, bool persistent = false)
        {
            Context = context as DbContext;
            Context.Database.SetCommandTimeout(1200);
        }
        #endregion

        public string PluginID => Guid.Parse("DAC0E1DC-B74B-4AA4-A429-3D74A1FD4DB6").ToString();

        public SqlTransaction BeginTransaction(Guid identityWorkID)
        {
            throw new NotImplementedException();
        }

        public SqlTransaction BeginTransaction(Guid identityWorkID, string transactionName)
        {
            throw new NotImplementedException();
        }

        public SqlTransaction BeginTransaction(Guid identityWorkID, IsolationLevel iso, string transactionName)
        {
            throw new NotImplementedException();
        }

        public SqlTransaction BeginTransaction(Guid identityWorkID, IsolationLevel iso)
        {
            throw new NotImplementedException();
        }

        public void BulkCreate(List<T> objectsToCreate, Guid identityWorkID = default)
        {
            throw new NotImplementedException();
        }

        public void BulkCreate<U>(List<U> objectsToCreate, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }

        public void BulkDelete(List<Guid> objectsToDelete, Guid identityWorkID = default)
        {
            throw new NotImplementedException();
        }

        public void BulkDelete<U>(List<Guid> objectsToDelete, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }

        public void BulkUpdate(List<T> objectToUpdate, Guid identityWorkID = default)
        {
            throw new NotImplementedException();
        }

        public void BulkUpdate<U>(List<U> objectsToUpdate, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }

        public void Create(List<T> objectsToCreate, Guid identityWorkID = default)
        {
            Context.AddRange(objectsToCreate);
            Context.SaveChanges();
        }

        public void Create<U>(List<U> objectsToCreate, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(List<T> objectsToCreate, Guid identityWorkID = default)
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            await Context.AddRangeAsync(objectsToCreate);
            await Context.SaveChangesAsync();
        }

        public async Task CreateorUpdateAsync(List<T> objectsToCreate, Guid identityWorkID = default)
        {             
            var found = await FindAsync(x => objectsToCreate.Select(p => p.ID).Contains(x.ID), identityWorkID: identityWorkID);
            objectsToCreate.ForEach(item =>
            {
                Context.Entry(item).State = !found.Any(f => f.ID == item.ID) ? EntityState.Added : EntityState.Modified;
            });             
            Context.SaveChanges();
        }

        public void Delete(List<Guid> objectsToDelete, Guid identityWorkID = default)
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            var keys = objectsToDelete.Select(p => "'" + p + "'").ToArray();
            var keySplitted = String.Join(",", keys);
            var sql = $"delete from {typeof(T).Name} where ID in ({keySplitted})";

            var row = Context.Database.ExecuteSqlRaw(sql);
            Context.SaveChanges();
        }

        public void Delete<U>(List<Guid> objectsToDelete, Guid identityWorkID = default) where U : BaseEntity
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            var keys = objectsToDelete.Select(p => "'" + p + "'").ToArray();
            var keySplitted = String.Join(",", keys);
            var sql = $"delete from {typeof(T).Name} where ID in ({keySplitted})";

            var row = Context.Database.ExecuteSqlRaw(sql);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Database.CloseConnection();
            Context.Dispose();
        }

        public async Task DeleteAsync(List<Guid> objectsToDelete, Guid identityWorkID = default)
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            var keys = objectsToDelete.Select(p => "'" + p + "'").ToArray();
            var keySplitted = String.Join(",", keys);
            var sql = $"delete from {typeof(T).Name} where ID in ({keySplitted})";

            var row = await Context.Database.ExecuteSqlRawAsync(sql);
            await Context.SaveChangesAsync();
        }

        public Task DeleteAsync<U>(List<Guid> objectsToDelete, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }

        public List<X> ExecuteSPReturn<U, X>(List<U> procedureParameters, Guid identityWorkID = default)
            where U : class
            where X : class
        {
            throw new NotImplementedException();
        }

        public List<T> Find(Expression<Func<T, bool>> predicate, string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default)
        {
            var query = Context
                .Set<T>()
                .AsNoTracking()
                .IncludeMultiple(objectsToInclude)
                .Where(p => p.Active)
                .Where(predicate);

            return query.ToList();
        }

        public List<T> Find(string predicate, string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default)
        {
            var query = Context
                .Set<T>()
                .AsNoTracking()
                .IncludeMultiple(objectsToInclude)
                .Where(p => p.Active)
                .Where(predicate);

            return query.ToList();
        }

        public async Task<List<T>> FindAsync(string predicate, string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default)
        {
            var query = await Context
                .Set<T>()
                .AsNoTracking()
                .IncludeMultiple(objectsToInclude)
                .Where(p => p.Active)
                .Where(predicate).ToListAsync();

            return query;
        }


        public List<U> Find<U>(Expression<Func<U, bool>> predicate, string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }

        public List<T> FindAll(string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default)
        {
            var query = Context
               .Set<T>()
               .AsNoTracking()
               .IncludeMultiple(objectsToInclude)
               .Where(p => p.Active)
               .Select(p => p);

            return query?.ToList();
        }

        public List<U> FindAll<U>(string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> FindAllAsync(string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default)
        {
            var query = await Context
              .Set<T>()
              .AsNoTracking()
              .IncludeMultiple(objectsToInclude)
              .Where(p => p.Active)
              .Select(p => p).ToListAsync();

            return query;
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default)
        {
            var query = await Context
              .Set<T>()
              .AsNoTracking()
              .IncludeMultiple(objectsToInclude)
              .Where(predicate).ToListAsync();

            return query;
        }

        public async Task<List<U>> FindAsync<U>(Expression<Func<U, bool>> predicate, string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default) where U : BaseEntity
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        public T Get(Guid Id, string[] objectsToInclude = null, Guid identityWorkID = default)
        {
            var t = Context
                .Set<T>()
                .AsNoTracking()
                .IncludeMultiple(objectsToInclude)
                .FirstOrDefault(p => p.Active && p.ID == Id);
            return t;
        }

        public Dictionary<Guid, T> GetByIds(List<Guid> Ids, string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default)
        {
            var t = Context
                .Set<T>()
                .AsNoTracking()
                .IncludeMultiple(objectsToInclude)
                .Where(p => p.Active && Ids.Contains(p.ID))
                .ToList();

            var dictionary = new ConcurrentDictionary<Guid, T>();
            Parallel.ForEach(t, t1 =>
            {
                dictionary.TryAdd(t1.ID, t1);
            });

            return dictionary.ToDictionary();
        }

        public async Task<Dictionary<Guid, T>> GetByIdsAsync(List<Guid> Ids, string[] objectsToInclude = null, int pageNumber = 0, int pageSize = 0, List<OrderParams> orderParamsList = null, Guid identityWorkID = default)
        {
            var t = await Context
                .Set<T>()
                .AsNoTracking()
                .IncludeMultiple(objectsToInclude)
                .Where(p => p.Active && Ids.Contains(p.ID))
                .ToListAsync();

            var dictionary = new ConcurrentDictionary<Guid, T>();
            Parallel.ForEach(t, t1 =>
            {
                dictionary.TryAdd(t1.ID, t1);
            });

            return dictionary.ToDictionary();
        }

        public SqlConnection GetConnection(Guid identityWorkID)
        {
            return Context.Database.GetDbConnection() as SqlConnection;
        }

        public string GetConnectionString(Guid identityWorkID)
        {
            return Context.Database.GetDbConnection().ConnectionString;
        }

        public int GetCount(Expression<Func<T, bool>> predicate, Guid identityWorkID = default)
        {
            var t = Context
                 .Set<T>()
                 //.Where(p => p.Active)
                 .Count(predicate);
            return t;
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkID = default)
        {
            var t = await Context
                 .Set<T>()
                 //.Where(p => p.Active)
                 .CountAsync(predicate);
            return t;
        }

        public int GetCount(string predicate, Guid identityWorkID = default)
        {
            var t = Context
                 .Set<T>()
                 //.Where(p => p.Active)
                 .Where(predicate)
                 .Count();
            return t;
        }

        public async Task<int> GetCountAsync(string predicate, Guid identityWorkID = default)
        {
            var t = await Context
                 .Set<T>()
                 .Where(predicate)
                 .CountAsync();
            return t;
        }

        public decimal GetUsedSpace(Guid identityWorkID)
        {
            throw new NotImplementedException();
        }

        public void Update(List<T> objectToUpdate, Guid identityWorkID = default)
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            Context.UpdateRange(objectToUpdate);
            Context.SaveChanges();
        }

        public void Update<U>(List<U> objectsToUpdate, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(List<T> objectsToUpdate, Guid identityWorkID = default)
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            Context.UpdateRange(objectsToUpdate);
            await Context.SaveChangesAsync();
        }

        public Task UpdateAsync<U>(List<U> objectsToUpdate, Guid identityWorkID = default) where U : BaseEntity
        {
            throw new NotImplementedException();
        }
    }
}

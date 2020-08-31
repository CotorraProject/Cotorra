using CotorraNode.Common.Base.Schema;
using Microsoft.EntityFrameworkCore;
using Cotorra.Core.Context;
using Cotorra.Schema;
using NPOI.OpenXmlFormats.Dml.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Cotorra.Core
{
    /// <summary>
    /// CRUD
    /// </summary>
    public class StatusManager<T> where T : IEntityId<Guid>, IStatusFull
    {
        #region "Attributes"

        public IStatusFullValidator<T> Validator { get; set; }

        #endregion

        #region "Constructor"
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_baseRecordManager"></param>
        /// <param name="_validator"></param>
        public StatusManager(IStatusFullValidator<T> validator)
        {
            this.Validator = validator;
        }

        #endregion

        public async Task SetActive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            await Validator.BeforeActivate(idsToUpdate, identityWorkId);
            await UpdateAsync(idsToUpdate, CotorriaStatus.Active, identityWorkId);
            await Validator.AfterActivate(idsToUpdate, identityWorkId);
        }

        public async Task SetUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, params object[] parameters)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await Validator.BeforeUnregistered(idsToUpdate, identityWorkId);
                await UpdateAsync(idsToUpdate, CotorriaStatus.Unregistered, identityWorkId, parameters);
                await Validator.AfterUnregistered(idsToUpdate, identityWorkId, parameters);
                scope.Complete();
            }          
        }

        public async Task SetInactive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            await Validator.BeforeInactivate(idsToUpdate, identityWorkId);
            await UpdateAsync(idsToUpdate, CotorriaStatus.Inactive, identityWorkId);
            await Validator.AfterInactivate(idsToUpdate, identityWorkId);

        }

        public async Task SetStatus(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, CotorriaStatus status)
        {
            await UpdateAsync(idsToUpdate, status, identityWorkId);
        }

        public async Task UpdateAsync(IEnumerable<Guid> idsToUpdate, CotorriaStatus status, Guid identityWorkID, params object[] parameters)
        {
            using var context = new CotorraContext(ConnectionManager.ConfigConnectionString);
            DateTime now = DateTime.UtcNow;
            string nowStrng = now.ToString("yyyy-mm-dd HH:mm:ss.fffffff");

            context.ChangeTracker.AutoDetectChangesEnabled = false;
            var keys = idsToUpdate.Select(p => "'" + p + "'").ToArray();
            var keySplitted = String.Join(",", keys);
            var sql = Validator.GetPersonalizedQuery(status, keySplitted, parameters);
            if (string.IsNullOrEmpty(sql))
            {
                sql = $"update   {typeof(T).Name} set LocalStatus = {(int)status}, LastStatusChange = getdate()  where ID in ({keySplitted}) ";
            }
            var row = await context.Database.ExecuteSqlRawAsync(sql);
            await context.SaveChangesAsync();
        }


    }


}

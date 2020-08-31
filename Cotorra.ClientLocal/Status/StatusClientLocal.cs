using Cotorra.Client;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.ClientLocal
{
    public class StatusClientLocal<T> : IStatusClient<T> where T : StatusIdentityCatalogEntityExt
    {
        private readonly string _authorizationHeader;

        public IStatusFullValidator<T> Validator { get; set; }

        public StatusClientLocal(string authorizationHeader)
        {
            Validator = new StatusfullValidatorFactory().CreateInstance<T>();
            _authorizationHeader = authorizationHeader;
        }

        public StatusClientLocal(string authorizationHeader, IStatusFullValidator<T> validator)
        {
            Validator = validator;
            _authorizationHeader = authorizationHeader;
        }

        public Task SetActive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            throw new NotImplementedException();
        }

        public async Task SetUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, params object[] parameters)
        {
            var statusManager = new StatusManager<T>(Validator);
            await statusManager.SetUnregistered(idsToUpdate, identityWorkId, parameters);
        }

        public Task SetInactive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            throw new NotImplementedException();
        }

        public Task SetStatus(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, CotorriaStatus status)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IEnumerable<Guid> idsToUpdate, CotorriaStatus status, Guid identityWorkID, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}

using CotorraNode.Common.Base.Schema;
using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Cotorra.Client;
using Cotorra.Schema;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using Serialize.Linq.Interfaces;
using System.Linq.Expressions;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Cotorra.ClientProxy
{
    public class StatusClientProxy<T> : IStatusClient<T> where   T : StatusIdentityCatalogEntityExt
    {
        private IStatusFullValidator<T> _validator;
        private string _authorizationHeader;
        private string _cotorraUri;
        private string _cotorraExtensionsUri;
        private readonly IConfigProvider _configProvider;

       
        public IStatusFullValidator<T> Validator
        {
            get => _validator; set => _validator = value;
        }

        public StatusClientProxy(string authorizationHeader, IStatusFullValidator<T> validator)
        {
           
        }

        public StatusClientProxy(string authorizationHeader)
        {
          
        }

        public StatusClientProxy(string authorizationHeader, IConfigProvider configProvider)
        {
           
        }

        public Task SetActive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            throw new NotImplementedException();
        }

        public Task SetUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, params object[] parameters)
        {
            throw new NotImplementedException();
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

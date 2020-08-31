using AutoMapper;
using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{
    public class CatalogServiceSearch<T, U> : ICatalogServiceSearch<T,U> where T : BaseEntity 
    {
        public async Task<IEnumerable<U>> Get(Expression<Func<T, bool>> predicate, Guid identityWorkID, string[] includes, IValidator<T> validator, IMapper mapper)
        {
            var mgr = new MiddlewareManager<T>(new BaseRecordManager<T>(), validator);
            var registers = await mgr.FindByExpressionAsync(predicate, identityWorkID, includes);
            return mapper.Map<List<T>, List<U>>(registers);  
        }
    }

}

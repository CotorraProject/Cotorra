using AutoMapper;
using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{
    
    public interface ICatalogServiceSearch<T, U> where T : BaseEntity
    {
        Task<IEnumerable<U>> Get(Expression<Func<T, bool>> predicate, Guid identityWorkID, string[] includes, IValidator<T> validator, IMapper mapper);
    }
}

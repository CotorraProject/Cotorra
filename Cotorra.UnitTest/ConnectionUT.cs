using Cotorra.Schema;
using System.Collections.Generic;
using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Dapper.Extensions.Linq.Builder;
using Cotorra.Core.Extensions;
using System.Threading.Tasks;
using Xunit;
using Dapper.Extensions.Linq.Extensions;
using CotorraNode.Common.Config;
using Cotorra.Core.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using Dapper.Extensions.Linq.Core.Configuration;
using Dapper.Extensions.Linq.Mapper;
using Dapper.Extensions.Linq.Sql;

namespace Cotorra.UnitTest
{
    public class ConnectionUT
    {
        [Fact]
        public async Task GetAllEF_Dapper()
        {
            //var connection = ConfigManager.GetValue("ConfigConnectionString");
            //var time = new Stopwatch();

            //time.Start();
            //DapperConfiguration.Use()
            //       .UseSqlDialect(new SqlServerDialect())
            //       .FromAssembly("Cotorra.Schema")
            //       .Build();

            //using (var cn = new SqlConnection(connection))
            //{
            //    Expression<Func<Area, bool>> filter = p => p.Active;
            //    var queryFilter = QueryBuilder<Area>.FromExpression(filter);

            //    var list = cn.GetList<Area>(
            //        queryFilter
            //    );
            //}
            //Trace.WriteLine(time.Elapsed.TotalSeconds);

            //using (var context = new CotorraContext(connection))
            //{
            //    string[] objectsToInclude = null;
            //    Expression<Func<Area, bool>> filter = p => p.Active;
            //    var query = await context
            //      .Set<Area>()
            //      .AsNoTracking()
            //      .IncludeMultiple(objectsToInclude)
            //      .Where(p => p.Active)
            //      .Where(filter).ToListAsync();
            //}
            //Trace.WriteLine(time.Elapsed.TotalSeconds);

            //time.Stop();
        }
    }
}

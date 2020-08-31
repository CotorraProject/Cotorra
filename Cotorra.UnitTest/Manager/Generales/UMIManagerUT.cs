using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Cotorra.Core.Extensions;
using Cotorra.Core.Validator;
using CotorraNode.Common.Base.Schema;
using System.Transactions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class UMIManagerUT
    {


        public class Get
        {
            //[Fact]
            [Theory]
            [InlineData("2017-01-01", 75.49)] 
            [InlineData("2018-01-01", 78.43)] 
            [InlineData("2019-01-01", 82.22)] 
            [InlineData("2020-01-01", 84.55)] 
            public async Task Should_Get_All(string date, decimal value)
            {
                var ValidityDate = DateTime.Parse(date);
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var middlewareManager = new MiddlewareManager<UMI>(new BaseRecordManager<UMI>(), new UMIValidator());
                //Asserts
                //Get
                var result = await middlewareManager.GetAllAsync();

                Assert.True(result.Any());
                Assert.NotNull(result.FirstOrDefault(x => x.ValidityDate == ValidityDate));
                Assert.Equal(value, result.FirstOrDefault(x => x.ValidityDate == ValidityDate).Value);
            }

            //[Fact]
            [Theory]
            [InlineData("2017-01-01", 75.49)]
            [InlineData("2018-01-01", 78.43)]
            [InlineData("2019-01-01", 82.22)]
            [InlineData("2020-01-01", 84.55)]
            public void Should_Get_All_From_Memory(string date, decimal value)
            {
                var ValidityDate = DateTime.Parse(date);
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var middlewareManager = new MemoryStorageContext();
                //Asserts
                //Get
                var result =   middlewareManager.GetdDefaultUMI();

                Assert.True(result.Any());
                Assert.NotNull(result.FirstOrDefault(x => x.ValidityDate == ValidityDate));
                Assert.Equal(value, result.FirstOrDefault(x => x.ValidityDate == ValidityDate).Value);
            }


        }

        public class GetEquality
        {
             [Fact] 
            public async Task Should_Get_All_In_Memory_Should_Be_Equal_To_Get_All_DB( )
            {
              
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var middlewareManager = new MiddlewareManager<UMI>(new BaseRecordManager<UMI>(), new UMIValidator());
                var memoryMgr = new MemoryStorageContext();

                //Asserts
                //Get
                var result = await middlewareManager.GetAllAsync();
                Assert.True(result.Any());                
                var resultMemory = memoryMgr.GetdDefaultUMI(); 
                Assert.True(resultMemory.Any());

                Assert.Equal(result.Count(), resultMemory.Count());

                resultMemory.ForEach(memory =>
                {
                    var db = result.FirstOrDefault(x => x.ID == memory.ID);
                    Assert.NotNull(db);

                }); 
            }
 

        }
    }
}

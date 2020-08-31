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
    public class InfonavitInsuranceUT
    {


        public class Get
        {
            //[Fact]
            [Theory]
            [InlineData("2009-01-01", 13.00)] 
            [InlineData("2010-01-01", 15.00)]  
            public async Task Should_Get_All(string date, decimal value)
            {
                var ValidityDate = DateTime.Parse(date);
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var middlewareManager = new MiddlewareManager<InfonavitInsurance>(new BaseRecordManager<InfonavitInsurance>(), new InfonavitInsuranceValidator());
                //Asserts
                //Get
                var result = await middlewareManager.GetAllAsync();

                Assert.True(result.Any());
                Assert.NotNull(result.FirstOrDefault(x => x.ValidityDate == ValidityDate));
                Assert.Equal(value, result.FirstOrDefault(x => x.ValidityDate == ValidityDate).Value);
            }

            //[Fact]
            [Theory]
            [InlineData("2009-01-01", 13.00)]
            [InlineData("2010-01-01", 15.00)]
            public void Should_Get_All_From_Memory(string date, decimal value)
            {
                var ValidityDate = DateTime.Parse(date);
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var middlewareManager = new MemoryStorageContext();
                //Asserts
                //Get
                var result =   middlewareManager.GetdDefaultInfonavitInsurance();

                Assert.True(result.Any());
                Assert.NotNull(result.FirstOrDefault(x => x.ValidityDate == ValidityDate));
                Assert.Equal(value, result.FirstOrDefault(x => x.ValidityDate == ValidityDate).Value);
            }


        }

        public class GetEquality
        {
            [Fact]
            public async Task Should_Get_All_In_Memory_Should_Be_Equal_To_Get_All_DB()
            {

                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var middlewareManager = new MiddlewareManager<InfonavitInsurance>(new BaseRecordManager<InfonavitInsurance>(), new InfonavitInsuranceValidator());
                var memoryMgr = new MemoryStorageContext();

                //Asserts
                //Get
                var result = await middlewareManager.GetAllAsync();
                Assert.True(result.Any());
                var resultMemory = memoryMgr.GetdDefaultInfonavitInsurance();
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

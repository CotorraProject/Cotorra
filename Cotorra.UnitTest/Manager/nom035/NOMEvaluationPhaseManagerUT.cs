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
using Cotorra.Schema.nom035;
using System.Threading.Tasks;

namespace Cotorra.UnitTest.Manager.nom035
{
    public class NOMEvaluationPhaseManagerUT
    {  
        public class GetAll
        {
            [Fact]
            public async Task Should_Get_All()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                //Act
                var middlewareManager = new MiddlewareManager<NOMEvaluationPhase>(new BaseRecordManager<NOMEvaluationPhase>(), new NOMEvaluationPhaseValidator());
                var all = await middlewareManager.FindByExpressionAsync(x => x.Active == true, Guid.Empty);

                Assert.True(all.Any());
            }

            [Fact]
            public async Task Should_Get_ATSPhases()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                //Act
                var middlewareManager = new MiddlewareManager<NOMEvaluationPhase>(new BaseRecordManager<NOMEvaluationPhase>(), new NOMEvaluationPhaseValidator());
                var ATS = await middlewareManager.FindByExpressionAsync(x => x.NOMEvaluationSurveyID == 
                Guid.Parse("6EC14905-F634-418F-B0D2-EF2F315835E8") && x.Active  == true, Guid.Empty);

                Assert.True(ATS.Any());
            }


        }

       
    }
}

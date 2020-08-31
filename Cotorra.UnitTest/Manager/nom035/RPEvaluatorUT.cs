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
using Cotorra.Core.Managers.nom035;

namespace Cotorra.UnitTest.Manager.nom035
{
    public class RPEvaluatorUT
    {  
        public class GetPoints
        {

            [Fact]
            public async Task When_answer_question_18_With_Never_should_get_4()
            {
                RPEvaluator evaluator = new RPEvaluator();

                var points = await Task.FromResult(evaluator.GetPoints(18, 4));
                 
                 
                Assert.Equal(4, points);
            }


            [Fact]
            public async Task When_answer_question_1_With_Never_should_get_0()
            {
                RPEvaluator evaluator = new RPEvaluator();

                var points = await Task.FromResult( evaluator.GetPoints(1, 4));


                Assert.Equal(0, points);
            }





        }


    }
}

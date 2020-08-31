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
using Cotorra.Schema.domain.nom035;
using Cotorra.Schema.nom035;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class NOMEvaluationQuestionManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {

            var NOMEvaluationGuide = new NOMEvaluationGuide()
            {
                ID = Guid.NewGuid(),
                Active = true,
                Timestamp = DateTime.UtcNow,
                Description = "Soy una guia",
                CreationDate = DateTime.Now,
                Name = "g1",
                StatusID = 1,
            };
            var middlewareManagerGuide = new MiddlewareManager<NOMEvaluationGuide>(
                new BaseRecordManager<NOMEvaluationGuide>(), new NOMEvaluationGuideValidator());
            await middlewareManagerGuide.CreateAsync(new List<NOMEvaluationGuide>
            {
                NOMEvaluationGuide
            }, Guid.Empty);

            var NOMEvaluationSurvey = new NOMEvaluationSurvey()
            {
                ID = Guid.NewGuid(),
                Active = true,
                Timestamp = DateTime.UtcNow,
                Description = "Soy una encuesta",
                CreationDate = DateTime.Now,
                Name = "s1",
                StatusID = 1,
                //NOMEvaluationGuide = NOMEvaluationGuide,
                NOMEvaluationGuideID = NOMEvaluationGuide.ID,
            };
            var middlewareManagerSurvey = new MiddlewareManager<NOMEvaluationSurvey>(
               new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            await middlewareManagerSurvey.CreateAsync(new List<NOMEvaluationSurvey>
            {
                NOMEvaluationSurvey
            }, identityWorkId);

            var NOMEvaluationPhase = new NOMEvaluationPhase()
            {
                ID = Guid.NewGuid(),
                Active = true,
                Timestamp = DateTime.UtcNow,
                Description = "Soy una Phase",
                CreationDate = DateTime.Now,
                Name = "P1",
                StatusID = 1,
                //NOMEvaluationSurvey = NOMEvaluationSurvey,
                NOMEvaluationSurveyID = NOMEvaluationSurvey.ID
            };
            var middlewareManagerPhase = new MiddlewareManager<NOMEvaluationPhase>(
               new BaseRecordManager<NOMEvaluationPhase>(), new NOMEvaluationPhaseValidator());
            await middlewareManagerPhase.CreateAsync(new List<NOMEvaluationPhase>
            {
                NOMEvaluationPhase
            }, identityWorkId);

            var NOMEvaluationQuestions = new List<NOMEvaluationQuestion>();
            NOMEvaluationQuestions.Add(new NOMEvaluationQuestion()
            {
                ID = Guid.NewGuid(),
                Active = true,
                Timestamp = DateTime.UtcNow,
                Description = "Soy una pregunta",
                CreationDate = DateTime.Now,
                Name = "Q1",
                Number = 1,
                StatusID = 1,
                //NOMEvaluationPhase = NOMEvaluationPhase,
                NOMEvaluationPhaseID = NOMEvaluationPhase.ID,
            });

            var middlewareManager = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());
            await middlewareManager.CreateAsync(NOMEvaluationQuestions, identityWorkId);

            return NOMEvaluationQuestions as List<T>;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_NOMEvaluationQuestion_And_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var NOMEvaluationQuestions = await new NOMEvaluationQuestionManagerUT().CreateDefaultAsync<NOMEvaluationQuestion>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                   .FindByExpressionAsync(p => p.ID == NOMEvaluationQuestions.FirstOrDefault().ID, Guid.Empty);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAsync(NOMEvaluationQuestions.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == NOMEvaluationQuestions.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(NOMEvaluationQuestions.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }

        }
    }
}

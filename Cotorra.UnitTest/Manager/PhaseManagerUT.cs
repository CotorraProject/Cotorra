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
using Cotorra.Schema.domain.nom035;

namespace Cotorra.UnitTest
{
    public class PhaseManagerUT
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

            return new List<NOMEvaluationPhase>
            {
                NOMEvaluationPhase
            } as List<T>;
        }
             
        public class Create
        {
            [Fact]
            public async Task Should_Create_NOMEvaluationPhase_And_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var NOMEvaluationPhases = await new PhaseManagerUT().CreateDefaultAsync<NOMEvaluationPhase>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<NOMEvaluationPhase>(new BaseRecordManager<NOMEvaluationPhase>(), new NOMEvaluationPhaseValidator());
                var middlewareManagerQuestion = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());

                //Asserts
                //Get
                var ids = NOMEvaluationPhases.Select(p => p.ID).ToList();
                var id = NOMEvaluationPhases.Select(p => p.ID).ToList().First();
                var result = middlewareManager
                    .GetByIds(ids, identityWorkId);
                Assert.True(result.Count() > 0);

                //Act again

                var NOMEvaluationPhases2 = new List<NOMEvaluationQuestion>();
                NOMEvaluationPhases2.Add(new NOMEvaluationQuestion()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    Timestamp = DateTime.UtcNow,
                    Description = "Mi trabajo me exige hacer mucho esfuerzo físico.",
                    CreationDate = DateTime.Now,
                    Name = "Mi trabajo me exige hacer mucho esfuerzo físico..",
                    StatusID = 1,
                    NOMEvaluationPhaseID = id,

                });

                var middlewareManager3 = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());
                await middlewareManager3.CreateAsync(NOMEvaluationPhases2, identityWorkId);

                var NoEvalQuestion = NOMEvaluationPhases2;
                var ids2 = NoEvalQuestion.Select(p => p.ID).ToList();

                var resultQuestions = await middlewareManagerQuestion.GetByIdsAsync(ids2, identityWorkId);
                Assert.True(result.Any());
                Assert.True(result.FirstOrDefault().ID == NOMEvaluationPhases.FirstOrDefault().ID);
                Assert.True(resultQuestions.Any());

                //Delete
                await middlewareManagerQuestion.DeleteAsync(ids2, identityWorkId);
                await middlewareManager.DeleteAsync(ids, identityWorkId);

                //Get it again to verify if the registry was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(ids, identityWorkId);
                Assert.True(!result2.Any());

                var result3 = await middlewareManager
                    .GetByIdsAsync(ids2, identityWorkId);
                Assert.True(!result3.Any());
            }
        }
    }
}

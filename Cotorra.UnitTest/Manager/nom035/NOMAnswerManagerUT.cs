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
    public class NOMAnswerManagerUT
    {


        public async  Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var nOMSurveyReplyResult = (await new NOMSurveyReplyManagerUT().CreateDefaultAsync<NOMSurveyReplyResult>(identityWorkId, instanceId)).FirstOrDefault();

            var NOMAnswers = BuildAnswers(identityWorkId, instanceId, nOMSurveyReplyResult.NOMSurveyReply.ID);
            var middlewareManager = new MiddlewareManager<NOMAnswer>(new BaseRecordManager<NOMAnswer>(), new NOMAnswerValidator());
            middlewareManager.Create(NOMAnswers, identityWorkId);

            return NOMAnswers as List<T>;
        }

        public List<NOMAnswer> BuildAnswers(Guid identityWorkId, Guid instanceId, Guid nomSurveyReplyID)
        {
            var NOMAnswers = new List<NOMAnswer>();

            NOMAnswers.Add(new NOMAnswer()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Answer de NOM",
                CreationDate = DateTime.Now,
                Name = "Nominas",
                StatusID = 1,
                Value = 1,
                NOMSurveyReplyID = nomSurveyReplyID,
                NOMEvaluationQuestionID = Guid.Parse("BB06A330-EE7B-453F-B9C1-2A17E7FDEC3D")
            });
            return NOMAnswers;
        }

        public NOMAnswer BuildAnswer(Guid identityWorkId, Guid instanceId, Guid nomSurveyReplyID)
        {
            return new NOMAnswer()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Answer de NOM",
                CreationDate = DateTime.Now,
                Name = "Nominas",
                StatusID = 1,
                Value = 1,
                NOMSurveyReplyID = nomSurveyReplyID,
                NOMEvaluationQuestionID = Guid.Parse("BB06A330-EE7B-453F-B9C1-2A17E7FDEC3D")
            };
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_NOMAnswer_And_Get_ToValidate_Finally_do_DeleteAsync()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var NOMAnswers = await new NOMAnswerManagerUT().CreateDefaultAsync<NOMAnswer>(identityWorkId, instanceId);                
               
                //Act
                var middlewareManager = new MiddlewareManager<NOMAnswer>(new BaseRecordManager<NOMAnswer>(), new NOMAnswerValidator());

                //Asserts
                //Get
                var result = middlewareManager
                    .GetByIds(NOMAnswers.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Count() > 0);

                //Delete
                middlewareManager.Delete(NOMAnswers.Select(p => p.ID).ToList(), identityWorkId);

                //Get it again to verify if the registry it was deleted
                var result2 = middlewareManager
                    .GetByIds(NOMAnswers.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result2.Count() == 0);
            }

           
        }

        public class Update
        {
            [Fact]
            public async Task Should_Create_NOMAnswer_And_Get_Update_Validate_Finally_do_DeleteAsync()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var NOMAnswers = await new NOMAnswerManagerUT().CreateDefaultAsync<NOMAnswer>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<NOMAnswer>(new BaseRecordManager<NOMAnswer>(), new NOMAnswerValidator());
                 
                //Get
                var result = middlewareManager
                    .GetByIds(NOMAnswers.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Count() > 0);

                result.FirstOrDefault().Value = 2;

                await middlewareManager.UpdateAsync(result, identityWorkId);

                result = middlewareManager.GetByIds(NOMAnswers.Select(p => p.ID).ToList(), identityWorkId);
                
                //Assert
                Assert.True(result.Count() > 0);
                Assert.Equal(2, result.FirstOrDefault().Value);

                //Delete
                middlewareManager.Delete(NOMAnswers.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == NOMAnswers.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = middlewareManager
                    .GetByIds(NOMAnswers.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result2.Count() == 0);
            }


        }
    }
}

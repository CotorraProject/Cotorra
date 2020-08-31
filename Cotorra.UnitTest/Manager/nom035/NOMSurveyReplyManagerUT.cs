using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Managers.nom035;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Xunit;

namespace Cotorra.UnitTest.Manager.nom035
{
    public class NOMSurveyReplyManagerUT
    {
         

        public NOMSurveyReply BuildNomSurveyReply(Guid identityWorkId, Guid instanceId, Guid employeeId,
       Guid periodId, Guid evaluationSurveyID)
        {
            var nomSurveyReply = new NOMSurveyReply()
            {
                ID = Guid.NewGuid(),
                Active = true,
                Timestamp = DateTime.UtcNow,
                Description = String.Empty,
                CreationDate = DateTime.Now,
                EmployeeID = employeeId,
                NOMEvaluationPeriodID = periodId,
                NOMEvaluationSurveyID = evaluationSurveyID,
                company = identityWorkId,
                user = Guid.NewGuid(),
                InstanceID = instanceId,
                Result = 0,
                ResultType = NOMSurveyReplyResultType.Numeric,
                EvaluationState = EvaluationStateType.New,
                Name = "",
                StatusID = 1,
            };

            return nomSurveyReply;
        }

        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : class
        {
            //EvaluationPeriod
            var nomEvaluationPeriod = new NOMEvaluationPeriod()
            {
                ID = Guid.NewGuid(),
                Active = true,
                Timestamp = DateTime.UtcNow,
                Description = "Soy una surveyreply",
                CreationDate = DateTime.Now,
                company = identityWorkId,
                user = Guid.NewGuid(),
                InstanceID = instanceId,
                Name = "g1",
                StatusID = 1,
                InitialDate = DateTime.Now,
                FinalDate = DateTime.Now.AddDays(1),
                Period = "2019",
                State = true,
            };
            var lstNomEvaluationPeriods = new List<NOMEvaluationPeriod>();
            lstNomEvaluationPeriods.Add(nomEvaluationPeriod);

            var middlewareManagerEvaluationPeriod = new MiddlewareManager<NOMEvaluationPeriod>(new BaseRecordManager<NOMEvaluationPeriod>(), new NOMEvaluationPeriodValidator());
            middlewareManagerEvaluationPeriod.Create(lstNomEvaluationPeriods, identityWorkId);

            //Employee
            var lstEmployees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, randomValues: true);

            var nomSurveyReply = new NOMSurveyReplyParams()
            {
                EmployeeId = lstEmployees.FirstOrDefault().ID,
                NOMEvaluationPeriodId = nomEvaluationPeriod.ID,
                NOMEvaluationSurveyId = Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"),
                IdentityWorkId = identityWorkId,
                InstanceId = instanceId
            };

            var manager = new NOMSurveyManager();
            var res = await manager.CreateAsync(nomSurveyReply);

            return new List<T>() { res as T };
        }

        [Fact]
        public async Task EncryptDecryptAsync()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var g = Guid.NewGuid();
            var token = StringCipher.Encrypt(g.ToString());
            var des = StringCipher.Decrypt(token);
        }

        [Fact]
        public async Task CreateSurveyAsync()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();

            var NOMSurveyReplies = (await CreateDefaultAsync<NOMSurveyReplyResult>(identityWorkId, instanceId)).FirstOrDefault();
            //Act
            var middlewareManager = new NOMSurveyManager();

            //Get
            var resultPrevious = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.NotNull(resultPrevious);
            Assert.True(resultPrevious.NOMAnswer.Any());

            //Delete

            await middlewareManager.DeleteAsync(new List<Guid>() { resultPrevious.ID }, identityWorkId);

            //Get it again to verify if the registry it was deleted
            var mustNotExists = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.Null(mustNotExists);

        }


        [Fact]
        public async Task UpdateSurveyAsync()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();

            var NOMSurveyReplies = (await CreateDefaultAsync<NOMSurveyReplyResult>(identityWorkId, instanceId)).FirstOrDefault();
            //Act
            var middlewareManager = new NOMSurveyManager();

            //Get
            var resultPrevious = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.NotNull(resultPrevious);

            resultPrevious.Result = 1;
            resultPrevious.EvaluationState = EvaluationStateType.Sent;

            //Delete
            await middlewareManager.UpdateSurveyAsync(resultPrevious, identityWorkId);

            //Get
            resultPrevious = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.NotNull(resultPrevious);
            Assert.Equal(1, resultPrevious.Result);
            Assert.Equal(EvaluationStateType.Sent, resultPrevious.EvaluationState);


            await middlewareManager.DeleteAsync(new List<Guid>() { resultPrevious.ID }, identityWorkId);

            //Get it again to verify if the registry it was deleted
            resultPrevious = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.Null(resultPrevious);

        }


        [Fact]
        public async Task UpdateSurveyAnswersAsync()
        {
            var transactionScopeoption = TransactionScopeOption.RequiresNew;
            using var scope = new TransactionScope(transactionScopeoption, TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();

            var NOMSurveyReplies = (await CreateDefaultAsync<NOMSurveyReplyResult>(identityWorkId, instanceId)).FirstOrDefault();
            //Act
            var middlewareManager = new NOMSurveyManager();

            //Get
            var resultPrevious = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.NotNull(resultPrevious);

            resultPrevious.Result = 1;
            resultPrevious.EvaluationState = EvaluationStateType.Sent;
            List<NOMAnswer> nOMAnswer = new List<NOMAnswer>()
            {
                new NOMAnswer()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    Timestamp = DateTime.UtcNow,
                    Description = "Soy una surveyreply",
                    CreationDate = DateTime.Now,
                    company = identityWorkId,
                    user = Guid.NewGuid(),
                    InstanceID = instanceId,
                    Name = "g1",
                    StatusID = 1,
                    Value = 1

                }
            };
            //resultPrevious.NOMAnswer = nOMAnswer;

            //Delete
            await middlewareManager.UpdateSurveyAsync(resultPrevious, identityWorkId);

            //Get
            resultPrevious = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.NotNull(resultPrevious);
            Assert.Equal(1, resultPrevious.Result);
            Assert.Equal(EvaluationStateType.Sent, resultPrevious.EvaluationState);

            await middlewareManager.DeleteAsync(new List<Guid>() { resultPrevious.ID }, identityWorkId);

            //Get it again to verify if the registry it was deleted
            resultPrevious = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.Null(resultPrevious);

        }

        [Fact]
        public async Task ProcessFinalization()
        {
            var transactionScopeoption = TransactionScopeOption.RequiresNew;
            using var scope = new TransactionScope(transactionScopeoption, TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var NOMSurveyReplies = (await CreateDefaultAsync<NOMSurveyReplyResult>(identityWorkId, instanceId)).FirstOrDefault();
            //Act
            var middlewareManager = new NOMSurveyManager();

            //Get
            var resultPrevious = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.NotNull(resultPrevious);
            Assert.True(resultPrevious.NOMAnswer.Any());


            //TODO answers and process


            //Delete

            await middlewareManager.DeleteAsync(new List<Guid>() { resultPrevious.ID }, identityWorkId);

            //Get it again to verify if the registry it was deleted
            var mustNotExists = await middlewareManager.GetAsync(NOMSurveyReplies.Token, identityWorkId);

            //Assert
            Assert.Null(mustNotExists);

        }

        //Si contesta no en todas las respuestas de la fase 1 
        [Fact]
        public async Task ATS_Result_should_be_zero_when_answer_phase_1_is_0()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var periodId = Guid.NewGuid();
            var evaluationSurveyId = Guid.Parse("6EC14905-F634-418F-B0D2-EF2F315835E8");//ATS
            var middlewareManager = new NOMSurveyManager();

            var nomSurveyReply = BuildNomSurveyReply(identityWorkId, instanceId, employeeId, periodId,
                        evaluationSurveyId);
            var answers = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var nomEvaluationPhaseManager = new MiddlewareManager<NOMEvaluationPhase>(new BaseRecordManager<NOMEvaluationPhase>(), new NOMEvaluationPhaseValidator());
            
            var aTSPhases = await nomEvaluationPhaseManager.FindByExpressionAsync(x => x.NOMEvaluationSurveyID ==
             evaluationSurveyId && x.Active == true, Guid.Empty);

            var listATSPhasesIDs = aTSPhases.Select(x => x.ID);
            var PhaseI = aTSPhases.FirstOrDefault(x => x.Number == 1);
            var questionMiddleWare = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());
            var aTSQuestions = questionMiddleWare.FindByExpression(e => listATSPhasesIDs.Contains(e.NOMEvaluationPhaseID),
                Guid.Empty);
            var phaseIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseI.ID,  Guid.Empty);

            answers.NOMEvaluationQuestionID = phaseIATSQuestions.FirstOrDefault().ID;
            answers.Value = 0;

            //Act
            var processed = middlewareManager.ProcessFinalizationATS(nomSurveyReply, new List<NOMAnswer>() { answers }, aTSQuestions, aTSPhases);

            //Arrange
            Assert.NotNull(processed);
            Assert.Equal(0, processed.Result);
            Assert.Equal(EvaluationStateType.Answered, processed.EvaluationState);

        }

        //Si contesta si en alguna de las respuestas de la fase 1 y si en alguna de la  fase 2
        [Fact]
        public async Task ATS_Result_should_be_one_when_answer_phase_1_is_1_and_answer_1_phase_2_is_1()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var periodId = Guid.NewGuid();
            var evaluationSurveyId = Guid.Parse("6EC14905-F634-418F-B0D2-EF2F315835E8");//ATS
            var middlewareManager = new NOMSurveyManager();

            var nomSurveyReply = BuildNomSurveyReply(identityWorkId, instanceId, employeeId, periodId,
                        evaluationSurveyId);
            var answerPhase1 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase2 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var nomEvaluationPhaseManager = new MiddlewareManager<NOMEvaluationPhase>(new BaseRecordManager<NOMEvaluationPhase>(), new NOMEvaluationPhaseValidator());

            var aTSPhases = await nomEvaluationPhaseManager.FindByExpressionAsync(x => x.NOMEvaluationSurveyID ==
             evaluationSurveyId && x.Active == true, Guid.Empty);

            var listATSPhasesIDs = aTSPhases.Select(x => x.ID);
            var PhaseI = aTSPhases.FirstOrDefault(x => x.Number == 1);
            var PhaseII = aTSPhases.FirstOrDefault(x => x.Number == 2);
            var questionMiddleWare = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());
            var aTSQuestions = questionMiddleWare.FindByExpression(e => listATSPhasesIDs.Contains(e.NOMEvaluationPhaseID),
                Guid.Empty);
            var phaseIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseI.ID, Guid.Empty);
            var phaseIIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseII.ID, Guid.Empty);


            answerPhase1.NOMEvaluationQuestionID = phaseIATSQuestions.FirstOrDefault().ID;
            answerPhase1.Value = 1;
           
            answerPhase2.NOMEvaluationQuestionID = phaseIIATSQuestions.FirstOrDefault().ID;
            answerPhase2.Value = 1;

            //Act
            var processed = middlewareManager.ProcessFinalizationATS(nomSurveyReply, new List<NOMAnswer>() { answerPhase1, answerPhase2 }, aTSQuestions, aTSPhases);


            //Arrange
            Assert.NotNull(processed);
            Assert.Equal(1, processed.Result);
            Assert.Equal(EvaluationStateType.Answered, processed.EvaluationState);

        }

        //Si contesta si en alguna de las respuestas de la fase 1   no en las de la  fase 2
        // y si en tres o mas de la fase 3
        [Fact]
        public async Task ATS_Result_should_be_one_when_answer_phase_1_is_1_and_answer_phase_2_is_0_and_phase_3_has_more_than_2()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var periodId = Guid.NewGuid();
            var evaluationSurveyId = Guid.Parse("6EC14905-F634-418F-B0D2-EF2F315835E8");//ATS
            var middlewareManager = new NOMSurveyManager();

            var nomSurveyReply = BuildNomSurveyReply(identityWorkId, instanceId, employeeId, periodId,
                        evaluationSurveyId);
            var answerPhase1 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase2 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase31 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase32 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase33 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var nomEvaluationPhaseManager = new MiddlewareManager<NOMEvaluationPhase>(new BaseRecordManager<NOMEvaluationPhase>(), new NOMEvaluationPhaseValidator());

            var aTSPhases = await nomEvaluationPhaseManager.FindByExpressionAsync(x => x.NOMEvaluationSurveyID ==
             evaluationSurveyId && x.Active == true, Guid.Empty);

            var listATSPhasesIDs = aTSPhases.Select(x => x.ID);
            var PhaseI = aTSPhases.FirstOrDefault(x => x.Number == 1);
            var PhaseII = aTSPhases.FirstOrDefault(x => x.Number == 2);
            var PhaseIII = aTSPhases.FirstOrDefault(x => x.Number == 3);
            var questionMiddleWare = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());
            var aTSQuestions = questionMiddleWare.FindByExpression(e => listATSPhasesIDs.Contains(e.NOMEvaluationPhaseID),
                Guid.Empty);
            var phaseIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseI.ID, Guid.Empty);
            var phaseIIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseII.ID, Guid.Empty);
            var phaseIIIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseIII.ID, Guid.Empty);

            answerPhase1.NOMEvaluationQuestionID = phaseIATSQuestions.FirstOrDefault().ID;
            answerPhase1.Value = 1;

            answerPhase2.NOMEvaluationQuestionID = phaseIIATSQuestions.FirstOrDefault().ID;
            answerPhase2.Value = 0;

            answerPhase31.NOMEvaluationQuestionID = phaseIIIATSQuestions.FirstOrDefault().ID;
            answerPhase31.Value = 1;
            answerPhase32.NOMEvaluationQuestionID = phaseIIIATSQuestions.FirstOrDefault().ID;
            answerPhase32.Value = 1;
            answerPhase33.NOMEvaluationQuestionID = phaseIIIATSQuestions.FirstOrDefault().ID;
            answerPhase33.Value = 1;

            //Act
            var processed = middlewareManager.ProcessFinalizationATS(nomSurveyReply, new List<NOMAnswer>() { answerPhase1, answerPhase2, answerPhase31, answerPhase32, answerPhase33 }, aTSQuestions, aTSPhases);


            //Arrange
            Assert.NotNull(processed);
            Assert.Equal(1, processed.Result);
            Assert.Equal(EvaluationStateType.Answered, processed.EvaluationState);

        }

        //Si contesta si en alguna de las respuestas de la fase 1   no en las de la  fase 2
        // y si en tres o mas de la fase 3
        [Fact]
        public async Task ATS_Result_should_be_one_when_answer_phase_1_is_1_and_answer_phase_2_is_0_and_phase_3_is_zero_and_phase4_has_more_than_1()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var periodId = Guid.NewGuid();
            var evaluationSurveyId = Guid.Parse("6EC14905-F634-418F-B0D2-EF2F315835E8");//ATS
            var middlewareManager = new NOMSurveyManager();

            var nomSurveyReply = BuildNomSurveyReply(identityWorkId, instanceId, employeeId, periodId,
                        evaluationSurveyId);
            var answerPhase1 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase2 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase31 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase32 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase33 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase41 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var answerPhase42 = new NOMAnswerManagerUT().BuildAnswer(identityWorkId, instanceId, nomSurveyReply.ID);
            var nomEvaluationPhaseManager = new MiddlewareManager<NOMEvaluationPhase>(new BaseRecordManager<NOMEvaluationPhase>(), new NOMEvaluationPhaseValidator());

            var aTSPhases = await nomEvaluationPhaseManager.FindByExpressionAsync(x => x.NOMEvaluationSurveyID ==
             evaluationSurveyId && x.Active == true, Guid.Empty);

            var listATSPhasesIDs = aTSPhases.Select(x => x.ID);
            var PhaseI = aTSPhases.FirstOrDefault(x => x.Number == 1);
            var PhaseII = aTSPhases.FirstOrDefault(x => x.Number == 2);
            var PhaseIII = aTSPhases.FirstOrDefault(x => x.Number == 3);
            var PhaseIV = aTSPhases.FirstOrDefault(x => x.Number == 4);
            var questionMiddleWare = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());
            var aTSQuestions = questionMiddleWare.FindByExpression(e => listATSPhasesIDs.Contains(e.NOMEvaluationPhaseID),
                Guid.Empty);
            var phaseIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseI.ID, Guid.Empty);
            var phaseIIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseII.ID, Guid.Empty);
            var phaseIIIATSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseIII.ID, Guid.Empty);
            var phaseIVTSQuestions = questionMiddleWare.FindByExpression(e => e.Active && e.NOMEvaluationPhaseID == PhaseIV.ID, Guid.Empty);

            answerPhase1.NOMEvaluationQuestionID = phaseIATSQuestions.FirstOrDefault().ID;
            answerPhase1.Value = 1;

            answerPhase2.NOMEvaluationQuestionID = phaseIIATSQuestions.FirstOrDefault().ID;
            answerPhase2.Value = 0;

            answerPhase31.NOMEvaluationQuestionID = phaseIIIATSQuestions.FirstOrDefault().ID;
            answerPhase31.Value = 0;
            answerPhase32.NOMEvaluationQuestionID = phaseIIIATSQuestions.FirstOrDefault().ID;
            answerPhase32.Value = 0;
            answerPhase33.NOMEvaluationQuestionID = phaseIIIATSQuestions.FirstOrDefault().ID;
            answerPhase33.Value = 0;

            answerPhase41.NOMEvaluationQuestionID = phaseIVTSQuestions.FirstOrDefault().ID;
            answerPhase41.Value = 1;
            answerPhase42.NOMEvaluationQuestionID = phaseIVTSQuestions.FirstOrDefault().ID;
            answerPhase42.Value = 1;

            //Act
            var processed = middlewareManager.ProcessFinalizationATS(nomSurveyReply, new List<NOMAnswer>() { answerPhase1, answerPhase2, answerPhase31, answerPhase32, answerPhase33, answerPhase41, answerPhase42 }, aTSQuestions, aTSPhases);


            //Arrange
            Assert.NotNull(processed);
            Assert.Equal(1, processed.Result);
            Assert.Equal(EvaluationStateType.Answered, processed.EvaluationState);

        }
    
    
    
    }
}

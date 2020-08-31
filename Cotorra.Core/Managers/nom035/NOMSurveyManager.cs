using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace Cotorra.Core.Managers.nom035
{
    public class NOMSurveyManager
    {
        private const string PH_SEED = "Nom1P@q3.!2020";

      
        public async Task<NOMSurveyReplyResult> CreateAsync(NOMSurveyReplyParams nOMSurveyReplyParams)
        {
            BeforeCreate(nOMSurveyReplyParams);

            var result = new NOMSurveyReplyResult();
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var NOMSurveyReply = CreateDefault(
                nOMSurveyReplyParams.IdentityWorkId,
                nOMSurveyReplyParams.InstanceId,
                nOMSurveyReplyParams.EmployeeId,
                nOMSurveyReplyParams.NOMEvaluationPeriodId,
                nOMSurveyReplyParams.NOMEvaluationSurveyId);

            await SetAnswers(NOMSurveyReply);

            //middleware instance
            var middlewareManager = new MiddlewareManager<NOMSurveyReply>(new BaseRecordManager<NOMSurveyReply>(), new NOMSurveyReplyValidator());

            //Get
            var resultPrevious = await middlewareManager.FindByExpressionAsync(p =>
               p.EmployeeID == nOMSurveyReplyParams.EmployeeId &&
               p.EvaluationState != EvaluationStateType.Answered &&
               p.NOMEvaluationPeriod.ID == nOMSurveyReplyParams.NOMEvaluationPeriodId, nOMSurveyReplyParams.IdentityWorkId);

            //verify if exists pending surverreply for employee
            if (!resultPrevious.Any())
            {
                //Create
                var lstToCreate = new List<NOMSurveyReply>();
                lstToCreate.Add(NOMSurveyReply);
                await middlewareManager.CreateAsync(lstToCreate, nOMSurveyReplyParams.IdentityWorkId);
            }

            //Encrypt and encode de token
            var encrypted = StringCipher.Encrypt(NOMSurveyReply.ID.ToString());
            var encryptedEncoded = HttpUtility.UrlEncode(encrypted);

            result.NOMSurveyReply = NOMSurveyReply;
            result.Token = encryptedEncoded;

            scope.Complete();

            return result;
        }

        public async Task<NOMSurveyReplyResult> UpdateSurveyAsync(NOMSurveyReply reply, Guid identityWorkId)
        {
            var middlewareManager = new MiddlewareManager<NOMSurveyReply>(new BaseRecordManager<NOMSurveyReply>(), new NOMSurveyReplyValidator());
            await middlewareManager.UpdateAsync(new List<NOMSurveyReply>() { reply }, identityWorkId);
            return new NOMSurveyReplyResult();
        }

        public async Task<NOMSurveyReply> GetAsync(string token, Guid identityWorkID)
        {
            var decode = HttpUtility.UrlDecode(token);
            Guid id = Guid.Parse(StringCipher.Decrypt(decode));

            var middlewareManager = new MiddlewareManager<NOMSurveyReply>(new BaseRecordManager<NOMSurveyReply>(), new NOMSurveyReplyValidator());

            var resultPrevious = await middlewareManager.FindByExpressionAsync(p => p.ID == id, identityWorkID, new string[] { "NOMAnswer" });
            return resultPrevious.FirstOrDefault();

        }

        public async Task DeleteAsync(List<Guid> id, Guid identityWorkID)
        {
            var nmAnswerMiddlewareManager = new MiddlewareManager<NOMAnswer>(new BaseRecordManager<NOMAnswer>(), new NOMAnswerValidator());

            var answers = await nmAnswerMiddlewareManager.FindByExpressionAsync(p => id.Contains(p.NOMSurveyReplyID), identityWorkID);

            var middlewareManager = new MiddlewareManager<NOMSurveyReply>(new BaseRecordManager<NOMSurveyReply>(), new NOMSurveyReplyValidator());
            await nmAnswerMiddlewareManager.DeleteAsync(answers.Select(p => p.ID).ToList(), identityWorkID);
            await middlewareManager.DeleteAsync(id, identityWorkID);

        }

        private NOMSurveyReply CreateDefault(Guid identityWorkId, Guid instanceId, Guid employeeId, Guid periodId, Guid evaluationSurveyID)
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


        public NOMSurveyReply ProcessFinalizationATS(NOMSurveyReply answeredSurvey, List<NOMAnswer> answers,
            List<NOMEvaluationQuestion> aTSQuestions,
            List<NOMEvaluationPhase> aTSPhases)
        {
            ATSEvaluator evaluator = new ATSEvaluator();
            return evaluator.ProcessFinalizationATS(answeredSurvey, answers, aTSQuestions, aTSPhases);
        }


        public NOMSurveyReply ProcessFinalizationRP(NOMSurveyReply answeredSurvey, List<NOMAnswer> answers,
             List<NOMEvaluationQuestion> aTSQuestions, List<NOMEvaluationCategory> categories, List<NOMEvaluationDomain> domains)
        {
            RPEvaluator evaluator = new RPEvaluator();
            return evaluator.ProcessFinalization(answeredSurvey, answers, aTSQuestions, categories, domains);
        }


        private void BeforeCreate(NOMSurveyReplyParams nOMSurveyReplyParams)
        {
            //validations
        }

        private async Task SetAnswers(NOMSurveyReply nOMSurveyReply)
        {
            List<NOMAnswer> answers = await GetAnswers(nOMSurveyReply.company, nOMSurveyReply.InstanceID, nOMSurveyReply.ID);
            nOMSurveyReply.NOMAnswer = answers;
        }

        private async Task<List<NOMAnswer>> GetAnswers(Guid identityWorkId, Guid instanceId, Guid nomSurveyReplyId)
        {
            ConcurrentBag<NOMAnswer> concurrentCollection = new ConcurrentBag<NOMAnswer>();
            var middlewareManager = new MiddlewareManager<NOMEvaluationQuestion>(new BaseRecordManager<NOMEvaluationQuestion>(), new NOMEvaluationQuestionValidator());
            var nomQuestions = await middlewareManager.FindByExpressionAsync(x => x.Active == true, Guid.Empty);

            Parallel.ForEach(nomQuestions, question =>
            {
                concurrentCollection.Add(new NOMAnswer()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceId,
                    Description = question.Description + " Answer",
                    CreationDate = DateTime.Now,
                    Name = question.Name + " Answer",
                    StatusID = 1,
                    Value = 1,
                    NOMSurveyReplyID = nomSurveyReplyId,
                    NOMEvaluationQuestionID = question.ID,
                });
            });
            return concurrentCollection.ToList();
        }



    }
}

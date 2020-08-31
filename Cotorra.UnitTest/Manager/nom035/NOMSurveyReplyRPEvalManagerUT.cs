using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Managers.nom035;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Core.Validator.nom035;
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
    public class NOMSurveyReplyRPEvalManagerUT
    {

        readonly int[] ItemsBlock1 = new int[] { 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33 };
        readonly int[] ItemsBlock2 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46 };


        [Fact]
        public async Task Should_return_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new NOMEvaluationDomainValidator());
            List<NOMAnswer> answers = new List<NOMAnswer>();

            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});

            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();


            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(), rPCategoreis,
                rPDomains);
            Assert.Equal(0, evaluatedSurvey.Result);

        }

        [Fact]
        public async Task Should_return_184_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());

            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();


            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            Assert.Equal(184, evaluatedSurvey.Result);

        }

        [Fact]
        public async Task Should_return_Category1_12_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var firstCategoryId = rPCategoreis.FirstOrDefault(x => x.Number == 1).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstCategoryResult = evaluatedSurvey.NOMSurveyCategoryResult.FirstOrDefault(x => x.NOMEvaluationCategoryID == firstCategoryId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(12, firstCategoryResult.Result);
        }

        [Fact]
        public async Task Should_return_Category1_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new 
                NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstCategoryId = rPCategoreis.FirstOrDefault(x => x.Number == 1).ID;
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);


            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();


            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstCategoryResult = evaluatedSurvey.NOMSurveyCategoryResult.FirstOrDefault(x => x.NOMEvaluationCategoryID == firstCategoryId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstCategoryResult.Result);
        }


        [Fact]
        public async Task Should_return_Category2_80_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var secondCategoryId = rPCategoreis.FirstOrDefault(x => x.Number == 2).ID;
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);


            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();


            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstCategoryResult = evaluatedSurvey.NOMSurveyCategoryResult.FirstOrDefault(x => x.NOMEvaluationCategoryID == secondCategoryId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(80, firstCategoryResult.Result);
        }

        [Fact]
        public async Task Should_return_Category2_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var secondCategoryId = rPCategoreis.FirstOrDefault(x => x.Number == 2).ID;
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);


            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();


            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstCategoryResult = evaluatedSurvey.NOMSurveyCategoryResult.FirstOrDefault(x => x.NOMEvaluationCategoryID == secondCategoryId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstCategoryResult.Result);
        }

        [Fact]
        public async Task Should_return_Category3_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());


            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var thirdCategoryId = rPCategoreis.FirstOrDefault(x => x.Number == 3).ID;
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);


            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstCategoryResult = evaluatedSurvey.NOMSurveyCategoryResult.FirstOrDefault(x => x.NOMEvaluationCategoryID == thirdCategoryId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstCategoryResult.Result);
        }

        [Fact]
        public async Task Should_return_Category4_76_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
             NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var thirdCategoryId = rPCategoreis.FirstOrDefault(x => x.Number == 4).ID;
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);


            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstCategoryResult = evaluatedSurvey.NOMSurveyCategoryResult.FirstOrDefault(x => x.NOMEvaluationCategoryID == thirdCategoryId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(76, firstCategoryResult.Result);
        }

        [Fact]
        public async Task Should_return_Category4_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
             NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var thirdCategoryId = rPCategoreis.FirstOrDefault(x => x.Number == 4).ID;


            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstCategoryResult = evaluatedSurvey.NOMSurveyCategoryResult.FirstOrDefault(x => x.NOMEvaluationCategoryID == thirdCategoryId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstCategoryResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain1_12_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
      
            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 1).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(12, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain1_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 1).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain2_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 2).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain2_52_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 2).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(52, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain3_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 3).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain3_28_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 3).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(28, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain4_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 4).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain4_8_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 4).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(8, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain5_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 5).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain5_8_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 5).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(8, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain6_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 6).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain6_20_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 6).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(20, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain7_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 7).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain7_24_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 7).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(24, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain8_32_when_block1_are_4_and_block2_are_0()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 8).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(184, evaluatedSurvey.Result);
            Assert.Equal(32, firstDomainResult.Result);
        }

        [Fact]
        public async Task Should_return_Domain8_0_when_block1_are_0_and_block2_are_4()
        {
            var surveyMiddleWare = new MiddlewareManager<NOMEvaluationSurvey>(new BaseRecordManager<NOMEvaluationSurvey>(), new NOMEvaluationSurveyValidator());
            var categoryMiddleware = new MiddlewareManager<NOMEvaluationCategory>(new BaseRecordManager<NOMEvaluationCategory>(), new NOMEvaluationCategoryValidator());
            var domainMiddleware = new MiddlewareManager<NOMEvaluationDomain>(new BaseRecordManager<NOMEvaluationDomain>(), new
              NOMEvaluationDomainValidator());

            List<NOMAnswer> answers = new List<NOMAnswer>();
            var anserUT = new NOMAnswerManagerUT();
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var nomSurveyReplyId = Guid.NewGuid();
            var nOMSurveyManager = new NOMSurveyManager();
            var survey = new NOMSurveyReply();
            var rPSurvey = await surveyMiddleWare.FindByExpressionAsync(e => e.ID == Guid.Parse("612A7938-1D11-4400-8BB7-AD29191AC33C"), Guid.Empty, new string[]{ "NOMEvaluationPhases",
            "NOMEvaluationPhases.NOMEvaluationQuestions"});
            var rPCategoreis = await categoryMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);

            var rPDomains = await domainMiddleware.FindByExpressionAsync(e => e.Active == true, Guid.Empty);
            var firstDomainId = rPDomains.FirstOrDefault(x => x.Number == 8).ID;

            var questions = rPSurvey.SelectMany(x => x.NOMEvaluationPhases).SelectMany(y => y.NOMEvaluationQuestions).ToList();

            ItemsBlock1.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 0;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            ItemsBlock2.ToList().ForEach(item =>
            {
                var answer = anserUT.BuildAnswer(identityWorkId, instanceId, nomSurveyReplyId);
                var questionId = questions.FirstOrDefault(x => x.Number == item).ID;
                answer.Value = 4;
                answer.NOMEvaluationQuestionID = questionId;
                answers.Add(answer);
            });

            var evaluatedSurvey = nOMSurveyManager.ProcessFinalizationRP(survey, answers, questions.ToList(),
                rPCategoreis, rPDomains);
            var firstDomainResult = evaluatedSurvey.NOMSurveyDomainResult.FirstOrDefault(x => x.NOMEvaluationDomainID == firstDomainId);
            Assert.Equal(0, evaluatedSurvey.Result);
            Assert.Equal(0, firstDomainResult.Result);
        }
    }

}

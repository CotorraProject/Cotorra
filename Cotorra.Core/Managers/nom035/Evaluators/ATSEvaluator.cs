using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.nom035
{
    public class ATSEvaluator
    {

        public NOMSurveyReply ProcessFinalizationATS(NOMSurveyReply answeredSurvey, List<NOMAnswer> answers,
            List<NOMEvaluationQuestion> aTSQuestions,
            List<NOMEvaluationPhase> aTSPhases)
        {

            var evaluationFirstAsemptionRes = FirstPhaseEvaluation(answeredSurvey, answers,
             aTSQuestions, aTSPhases);
            var needMoreValidation = evaluationFirstAsemptionRes.need;
            answeredSurvey = evaluationFirstAsemptionRes.survey;

            if(needMoreValidation)
            {
                var evaluationSecondAsemptionRes = FirstToFourthPhaseEvaluation(answeredSurvey, answers,
                    aTSQuestions, aTSPhases, 2, 0);

                needMoreValidation = evaluationSecondAsemptionRes.need;
                answeredSurvey = evaluationSecondAsemptionRes.survey;

                if(needMoreValidation)
                {
                    var (survey, need) = FirstToFourthPhaseEvaluation(answeredSurvey, answers,
                    aTSQuestions, aTSPhases, 3, 2);

                    needMoreValidation = need;
                    answeredSurvey = survey;

                    if(needMoreValidation)
                    {
                        var evaluationFourthAsemptionRes = FirstToFourthPhaseEvaluation(answeredSurvey, answers,
                        aTSQuestions, aTSPhases,4,1);
                        needMoreValidation = evaluationFourthAsemptionRes.need;
                        answeredSurvey = evaluationFourthAsemptionRes.survey;
                    }
                }
                 
            }

            return answeredSurvey;
        }


        private (NOMSurveyReply survey, bool need) FirstPhaseEvaluation(NOMSurveyReply answeredSurvey, List<NOMAnswer> answers,
            List<NOMEvaluationQuestion> aTSQuestions,
            List<NOMEvaluationPhase> aTSPhases)
        {
            var firstPhaseATS = aTSPhases.FirstOrDefault(x => x.Number == 1);
            var firstPhaseATSQuestions = aTSQuestions.Where(x => x.NOMEvaluationPhaseID == firstPhaseATS.ID).ToList();
            var firstPhaseATSQuestionsIds = firstPhaseATSQuestions.Select(_ => _.ID);
            var firstPhaseATSAnswers = answers.Where(x => firstPhaseATSQuestionsIds.Contains(x.NOMEvaluationQuestionID));
            bool needMoreValidation = true;
            if (!firstPhaseATSAnswers.Any())
            {
                throw new CotorraException(0111, "Eval", "La evaluacion no contiene preguntas de Fase I", null);
            }
            var sumVal = firstPhaseATSAnswers.Sum(x => x.Value);
            if (sumVal == 0)
            {
                answeredSurvey.EvaluationState = EvaluationStateType.Answered;
                answeredSurvey.Result = 0;
                needMoreValidation = false;
            }
            return (survey: answeredSurvey, need: needMoreValidation);
        }

        private (NOMSurveyReply survey, bool need) FirstToFourthPhaseEvaluation(NOMSurveyReply answeredSurvey, List<NOMAnswer> answers,
       List<NOMEvaluationQuestion> aTSQuestions,
       List<NOMEvaluationPhase> aTSPhases, int phaseNumber, int validBoundary)
        {
            var phaseATS = aTSPhases.FirstOrDefault(x => x.Number == phaseNumber);
            var phaseATSQuestions = aTSQuestions.Where(x => x.NOMEvaluationPhaseID == phaseATS.ID).ToList();
            var phaseATSQuestionsIds = phaseATSQuestions.Select(_ => _.ID);
            var phaseATSAnswers = answers.Where(x => phaseATSQuestionsIds.Contains(x.NOMEvaluationQuestionID));
            bool needMoreValidation = phaseNumber == 4 ? false:true;

            if (!phaseATSAnswers.Any())
            {
                throw new CotorraException(0111, "Eval", "La evaluacion no contiene preguntas de Fase "+ phaseNumber, null);
            }
            var sumVal = phaseATSAnswers.Sum(x => x.Value);
            if (sumVal > validBoundary)
            {
                answeredSurvey.EvaluationState = EvaluationStateType.Answered;
                answeredSurvey.Result = 1;
                needMoreValidation = false;
            }
            return (survey: answeredSurvey, need: needMoreValidation);
        }

    }
}

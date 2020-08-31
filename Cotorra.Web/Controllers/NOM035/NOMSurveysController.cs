using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using Cotorra.Schema.nom035;
using System.IO;

namespace Cotorra.Web.Controllers
{
    /// <summary>
    /// NomSurvey Controller
    /// </summary>
    public class NOMSurveysController : Controller
    {
        public NOMSurveysController()
        {
        }

        [HttpGet]
        [TelemetryUI]
        [ResponseCache(Duration = 2600)]
        public async Task<JsonResult> Get()
        {
            return Json("OK");
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetAllActive(String token)
        {
            var client = new Client<NOMEvaluationSurvey>(String.Empty, ClientConfiguration.GetAdapterFromConfig());
            var surveys = await client.FindAsync(e => e.Active, Guid.Empty, new String[] { "NOMEvaluationGuide" });

            var result = from s in surveys
                         orderby s.NOMEvaluationGuide.Number
                         select new
                         {
                             s.ID,
                             s.Name,
                             s.Description,
                             Guide = s.NOMEvaluationGuide.Name,
                         };

            return Json(result);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetDetail(Guid surveyID)
        {
            var client = new Client<NOMEvaluationQuestion>(String.Empty, ClientConfiguration.GetAdapterFromConfig());
            var questions = await client.FindAsync(e => e.Active && e.NOMEvaluationPhase.NOMEvaluationSurveyID == surveyID, Guid.Empty, new string[] { "NOMEvaluationPhase" });

            var groupedQuestions = (from q in questions
                                    orderby q.NOMEvaluationPhase.Number
                                    group q by q.NOMEvaluationPhaseID into gq
                                    select gq).ToList();

            var phases = (from gq in groupedQuestions
                          select new
                          {
                              name = questions.FirstOrDefault(x => x.NOMEvaluationPhase.ID == gq.Key).NOMEvaluationPhase.Name,
                              number = questions.FirstOrDefault(x => x.NOMEvaluationPhase.ID == gq.Key).NOMEvaluationPhase.Number,
                              questions = from gq2 in gq
                                          select new
                                          {
                                              qid = gq2.ID,
                                              qnumber = gq2.Number,
                                              qname = gq2.Name,
                                              qvalue = ""
                                          }
                          }).ToList();

            return Json(phases);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> Save()
        {
            return Json("OK");
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<ContentResult> Respond(String id)
        {
            String html = "";
            using (StreamReader streamReader = new StreamReader("wwwroot/views/nom035/surveys.html"))
            {
                html = await streamReader.ReadToEndAsync();
            }

            return new ContentResult
            {
                ContentType = "text/html",
                Content = html
            };
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> VerifyUser(LoginUser loginUser)
        {
            return await  Task.FromResult( Json("OK"));
        }

        public class LoginUser
        {
            public String Token { get; set; }
            public String LastName { get; set; }
            public Guid Department { get; set; }
        }
    }
}

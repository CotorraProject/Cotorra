using CotorraNode.Common.Config;
using CotorraNode.Common.Library.Public;
using Cotorra.Core.Managers.Calculation.Functions.Catalogs;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Cotorra.UnitTest
{
    public class CotorriaBotLUISUT
    {



        [Theory]
        [InlineData("vacaciones los dias 23 de enero y 5 de febrero")]
        [InlineData("vacaciones los días 21, 22, 23, 26 de enero y 1,2,3 de febrero 2020")]
        [InlineData("solicito vacaciones del 23 de febrero al 25 de febrero del 2020")]
        [InlineData("quiero vacaciones el dia 23 de junio de 2020")]
        public async Task GetVacationsIntent(string utterance)
        {
            //////////
            // Values to modify.

            // YOUR-APP-ID: The App ID GUID found on the www.luis.ai Application Settings page.
            var appId = "6c734624-729d-41cc-8895-39e2e50cc5d2";

            // YOUR-PREDICTION-KEY: 32 character key.
            var predictionKey = "38dc988269fe4a368111821de2d0de67";

            // YOUR-PREDICTION-ENDPOINT: Example is "https://westus.api.cognitive.microsoft.com/"
            var predictionEndpoint = "https://westus.api.cognitive.microsoft.com/";           
            var res = await MakeRequest(predictionKey, predictionEndpoint, appId, utterance);

            Assert.NotNull(res);
            Assert.NotEmpty(res);

        }

        static async Task<string>  MakeRequest(string predictionKey, string predictionEndpoint, string appId, string utterance)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", predictionKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["query"] = utterance;

            // These optional request parameters are set to their default values
            queryString["verbose"] = "true";
            queryString["show-all-intents"] = "true";
            queryString["staging"] = "false";
            queryString["timezoneOffset"] = "0";

            var predictionEndpointUri = String.Format("{0}luis/prediction/v3.0/apps/{1}/slots/production/predict?{2}", predictionEndpoint, appId, queryString);

            // Remove these before updating the article.
            Console.WriteLine("endpoint: " + predictionEndpoint);
            Console.WriteLine("appId: " + appId);
            Console.WriteLine("queryString: " + queryString);
            Console.WriteLine("endpointUri: " + predictionEndpointUri);

            var response = await client.GetAsync(predictionEndpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            // Display the JSON result from LUIS.
           return strResponseContent.ToString();
        }






    }
}

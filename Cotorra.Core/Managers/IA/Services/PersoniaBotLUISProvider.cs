using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CotorraNode.Common.Config;

namespace Cotorra.Core
{
    public class CotorriaBotLUISProvider : ICotorriaBotProvider
    {
        private readonly string _appId;
        private readonly string _predictionKey;
        private readonly string _predictionEndpoint;

        public CotorriaBotLUISProvider()
        {
            _appId = ConfigManager.GetValue("LUISCotorriaBotAppId");
            _predictionKey = ConfigManager.GetValue("LUISCotorriaBotPredictionID");
            _predictionEndpoint = ConfigManager.GetValue("LUISCotorriaBotPredictionEndpoint");
        }

        public async Task<string> GetIntent(string utterance)
        {
           
            var res = await MakeRequest(_predictionKey, _predictionEndpoint, _appId, utterance);
            return res;
        }

        static async Task<string> MakeRequest(string predictionKey, string predictionEndpoint, string appId, string utterance)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", predictionKey);
            queryString["query"] = utterance;
            queryString["verbose"] = "true";
            queryString["show-all-intents"] = "true";
            queryString["staging"] = "false";
            queryString["timezoneOffset"] = "0";

            var predictionEndpointUri = String.Format("{0}luis/prediction/v3.0/apps/{1}/slots/production/predict?{2}", predictionEndpoint, appId, queryString); 
            var response = await client.GetAsync(predictionEndpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync(); 
            return strResponseContent.ToString();
        }


    }

}

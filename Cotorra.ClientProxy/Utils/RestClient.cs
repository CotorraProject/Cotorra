using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.ClientProxy
{
    public class RestClient
    {
        #region Properties
        private string url;

        private string endPoint;

        private string token;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FacturaClient"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="enpoint">The enpoint.</param>
        /// <param name="token">The token.</param>
        public RestClient(string url, string enpoint, string token)
        {
            this.url = url;
            this.endPoint = enpoint;
            this.token = token;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Get web service call.
        /// </summary>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<string> Get(string urlParams = "")
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Authorization", token);
                HttpResponseMessage responseMessage = await client.GetAsync(endPoint + urlParams);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return await responseMessage.Content.ReadAsStringAsync();
                }
                var errorContent = await responseMessage.Content.ReadAsStringAsync();
                throw new Exception(errorContent);
            }
        }

        /// <summary>
        /// Post web service call.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async Task<string> Post(object parameters)
        {
            var json = JsonConvert.SerializeObject(parameters);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Authorization", token);
                var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await client.PostAsync(endPoint, content);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return await responseMessage.Content.ReadAsStringAsync();
                }
                var errorContent = await responseMessage.Content.ReadAsStringAsync();
                throw new Exception(errorContent);                
            }

        }
        #endregion
    }
}

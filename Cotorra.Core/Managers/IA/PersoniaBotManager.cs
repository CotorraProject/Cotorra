using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Cotorra.Core
{
    public class CotorriaBotManager : ICotorriaBotManager
    {
        public ICotorriaBotProvider BotProvider { get; set; }
        public CotorriaBotManager(ICotorriaBotProvider botProvider)
        {
            this.BotProvider = botProvider;

        }

        public async Task<string> GetIntent(string utterance)
        {

            return await BotProvider.GetIntent(utterance);

        }




    }

}

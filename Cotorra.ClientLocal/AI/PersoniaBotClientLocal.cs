using Cotorra.Core;
using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class CotorriaBotClientLocal : ICotorriaBotClient
    {
        public CotorriaBotClientLocal(string authorizationHeader)
        {
        } 

        public async Task<string> GetIntent(GetIntentParams parameters)
        {
            CotorriaBotManager CotorriaBotManager = new CotorriaBotManager(new CotorriaBotLUISProvider());
            return await CotorriaBotManager.GetIntent(parameters.Utterance);
        }
    }
}

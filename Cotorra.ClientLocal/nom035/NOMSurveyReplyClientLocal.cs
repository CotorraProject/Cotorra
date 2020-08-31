using Cotorra.Client.nom035;
using Cotorra.Core.Managers.nom035;
using Cotorra.Schema.nom035;
using System.Threading.Tasks;

namespace Cotorra.ClientLocal.nom035
{
    public class NOMSurveyReplyClientLocal : INOMSurveyReplyClient
    {
        public async Task<NOMSurveyReplyResult> CreateAsync(NOMSurveyReplyParams nOMSurveyReplyParams)
        {
            return await new NOMSurveyManager().CreateAsync(nOMSurveyReplyParams);
        }
    }
}

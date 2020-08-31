using Cotorra.Schema.nom035;
using System.Threading.Tasks;

namespace Cotorra.Client.nom035
{
    public interface INOMSurveyReplyClient
    {
        Task<NOMSurveyReplyResult> CreateAsync(NOMSurveyReplyParams nOMSurveyReplyParams);
    }
}

using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface ICotorriaBotClient
    {
        Task<string> GetIntent(GetIntentParams parameters);
    }
}

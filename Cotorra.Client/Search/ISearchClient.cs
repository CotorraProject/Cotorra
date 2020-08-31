using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface ISearchClient
    {
        Task<T> QueryAsync<T>(string query);
    }
}

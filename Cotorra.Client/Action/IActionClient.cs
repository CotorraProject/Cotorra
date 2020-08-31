using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IActionClient
    {
        Task DispatchAsync(Guid actionID, Guid registerID);
    }
}

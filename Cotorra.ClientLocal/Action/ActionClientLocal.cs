using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class ActionClientLocal : IActionClient
    {
        public ActionClientLocal(string authorizationHeader)
        {
        }

        public async Task DispatchAsync(Guid actionID, Guid registerID)
        {
            throw new NotSupportedException("Try proxy or implement Dispatch localy");
        }
         
    }
}

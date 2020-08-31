using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public interface ICotorriaBotProvider
    {
        Task<string> GetIntent(string utterance);
    }
}

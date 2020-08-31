using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.FiscalValidator
{
    public interface IFiscalValidator
    {
        Task<string> ValidateCFDIAsync(SignDocumentResult<ICFDINomProvider> signDocumentResult);

        Task<string> ValidateCFDIAsync(string xml);
    }
}

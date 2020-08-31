using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.FiscalStampingProviders.PAC
{
    public interface IPACProvider
    {
        Task<SignDocumentResult<ICFDINomProvider>> StampDocumetAsync(
             SignDocumentResult<ICFDINomProvider> signDocumentResult, FiscalStampingVersion fiscalStampingVersion, string xml);

        Task<CancelDocumentResult<ICFDINomProvider>> CancelStampingDocumentAsync(
                CancelDocumentResult<ICFDINomProvider> cancelDocumentResult);
    }
}

using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.FiscalStamping
{
    public interface ICancelStamping
    {
        Task<CancelPayrollStampingResult> CancelDocumetAsync(CancelDocumentParams cancelDocumentParams);
    }
}

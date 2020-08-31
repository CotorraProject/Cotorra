using NPOI.SS.UserModel;
using System;
using System.Threading.Tasks;

namespace Cotorra.Core.Utils
{
    public interface IMSSpreadsheetWriter
    {
        Task<string> WriteAsync(Guid instanceID, string FileName, IWorkbook workbook);
    }
}
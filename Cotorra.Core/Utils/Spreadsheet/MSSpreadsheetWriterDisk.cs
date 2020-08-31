using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Utils
{
    public class MSSpreadsheetWriterDisk : IMSSpreadsheetWriter
    {
        public async Task<string> WriteAsync(Guid instanceID, string FileName, IWorkbook workbook)
        {
            using var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            workbook.Write(fs);
            return await Task.FromResult(FileName);
        }
    }
}

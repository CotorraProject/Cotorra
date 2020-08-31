using System; 
using System.Threading.Tasks;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Mvc;
using Cotorra.Core;

namespace Cotorra.WebAPI.Controllers
{
    /// <summary>
    /// Cotorria Bot Service - Consultas en lenguaje natural
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CotorriaBotController : BaseCotorraController
    {
         

        /// <summary>
        /// Consultas en lenguaje natural
        /// </summary>
        /// <param name="parameters">Parámetros requeridos para hacer la consulta</param>
        /// <returns>La intención de consulta</returns>
        [HttpGet("GetIntent/{companyId}/{instanceId}/{utterance}")]
        //[Security(ServiceID = PermissionContants.SuperMamalonPermission.ServiceID,
        //  UseAuthorization = false,
        //  UseSession = false,
        //  Permissions = new[] { PermissionContants.SuperMamalonPermission.PermissionID },
        //  ResourceID = PermissionContants.SuperMamalonPermission.CotoRRA_Cloud_ID_String,
        //  UserInstanceAsOwner = false)]
        public async Task<string> GetIntent(Guid companyId, Guid instanceId, string utterance)
        {
            CotorriaBotManager CotorriaBotManager = new CotorriaBotManager(new CotorriaBotLUISProvider());
            return await CotorriaBotManager.GetIntent(utterance);
        }


    
        [HttpGet("GetExcel")]

        public Task<string> GetExcel( )
        {
            IWorkbook workbook = new XSSFWorkbook();

            ISheet sheet1 = workbook.CreateSheet("Sheet1");

            sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
            var rowIndex = 0;
            IRow row = sheet1.CreateRow(rowIndex);
            row.Height = 30 * 80;
            row.CreateCell(0).SetCellValue("this is content");
            sheet1.AutoSizeColumn(0);
            rowIndex++;

            var sheet2 = workbook.CreateSheet("Sheet2");
            var style1 = workbook.CreateCellStyle();
            style1.FillForegroundColor = HSSFColor.Blue.Index2;
            style1.FillPattern = FillPattern.SolidForeground;

            var style2 = workbook.CreateCellStyle();
            style2.FillForegroundColor = HSSFColor.Yellow.Index2;
            style2.FillPattern = FillPattern.SolidForeground;

            var cell2 = sheet2.CreateRow(0).CreateCell(0);
            cell2.CellStyle = style1;
            cell2.SetCellValue(0);

            cell2 = sheet2.CreateRow(1).CreateCell(0);
            cell2.CellStyle = style2;
            cell2.SetCellValue(1);


            return Task.FromResult("ok 3");
        }



    }
}

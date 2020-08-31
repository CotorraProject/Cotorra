
using Cotorra.Core.Managers;
using Cotorra.Schema;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;


namespace Cotorra.Core.Utils
{

    public class SettlementLetterMSSpreadsheet
    {
        /// <summary>
        /// Generates the settlement letter.
        /// </summary>
        /// <param name="overdraft">The overdraft.</param>
        /// <param name="header">The header.</param>
        /// <param name="footer">The footer.</param>
        /// <param name="date">The date.</param>
        /// <param name="newFile">The new file.</param>
        public IWorkbook GenerateSettlementLetter(List<Overdraft> overdrafts, string header, string footer, string date,
            string employeeName, OverdraftTotalsResult totals)
        {
            var rowIndex = 3;
            var colIndex = 1;
            var headerColIndex = 0;
            var datRow = 0;
            var dateCol = 5;
            var signCol = 3;


            IWorkbook workbook = new XSSFWorkbook();

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.FontName = "Calibri";
            font.Boldweight = (short)FontBoldWeight.Bold;

            ISheet sheet1 = workbook.CreateSheet("Carta Finiquito");
            sheet1.SetColumnWidth(2, 5000);
            sheet1.SetColumnWidth(5, 5000);
            sheet1.SetColumnWidth(0, 1000);
            sheet1.SetColumnWidth(3, 3000);
            sheet1.SetColumnWidth(6, 3000);

            //Date
            IRow row = sheet1.CreateRow(datRow);
            var cell = row.CreateCell(dateCol);
            cell.SetCellValue(date);

            //Header
            CreateHeader(sheet1, workbook, row, font, cell, rowIndex, headerColIndex, header);
            rowIndex += 5;

            //perceptions deductions Title
            CreateDetailsTitle(sheet1, workbook, row, font, cell, rowIndex, colIndex);
            rowIndex++;
            CreateRowSeparation(sheet1, row, rowIndex);
            rowIndex++;
            //perceptions deductions header
            CreateDetailsHeader(sheet1, workbook, row, font, rowIndex, colIndex);
            rowIndex++;
            rowIndex = CreateOverDraftDetails(overdrafts, sheet1, workbook, row, font, rowIndex, colIndex);
            rowIndex++;

            CreateSums(sheet1, workbook, row, font, cell, rowIndex, colIndex, totals);
            rowIndex++;

            CreateNetPayment(sheet1, workbook, row, font, cell, rowIndex, colIndex, totals);

            rowIndex += 4;
            Createfooter(sheet1, workbook, row, font, cell, rowIndex, headerColIndex, footer, signCol,
                employeeName);
            //workbook.Write(fs);

            return workbook;
        }

        private void CreateHeader(ISheet sheet1, IWorkbook workbook, IRow row, IFont font, ICell cell,
            int rowIndex, int headerColIndex, string header)
        {
            sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, headerColIndex, headerColIndex + 6));
            row = sheet1.CreateRow(rowIndex);
            row.Height = 1500;
            ICellStyle cs = workbook.CreateCellStyle();
            cs.WrapText = true;
            cell = row.CreateCell(headerColIndex);
            cell.SetCellValue(header);
            cell.CellStyle = cs;
        }

        private void Createfooter(ISheet sheet1, IWorkbook workbook, IRow row, IFont font, ICell cell,
           int rowIndex, int headerColIndex, string footer, int signCol, string employeeName)
        {
            ICellStyle attstyle = workbook.CreateCellStyle();
            attstyle.Alignment = HorizontalAlignment.Center;


            sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, headerColIndex, headerColIndex + 6));
            row = sheet1.CreateRow(rowIndex);
            row.Height = 3000;
            ICellStyle cs = workbook.CreateCellStyle();
            cs.WrapText = true;
            cell = row.CreateCell(headerColIndex);
            cell.SetCellValue(footer);
            cell.CellStyle = cs;

            rowIndex += 2;

            sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, signCol, signCol + 1));

            row = sheet1.CreateRow(rowIndex);
            var attcell = row.CreateCell(signCol);
            attcell.SetCellValue("ATENTAMENTE");
            attcell.CellStyle = attstyle;

            ICellStyle css = workbook.CreateCellStyle();
            css.BorderBottom = BorderStyle.Medium;

            rowIndex += 2;
            row = sheet1.CreateRow(rowIndex);
            var cell2 = row.CreateCell(signCol);
            cell2.CellStyle = css;

            var cell3 = row.CreateCell(signCol + 1);
            cell3.CellStyle = css;
            var cell6 = row.CreateCell(signCol - 1);
            cell6.CellStyle = css;
            var cell5 = row.CreateCell(signCol + 2);
            cell5.CellStyle = css;

            rowIndex++;
            sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, signCol - 1, signCol + 2));
            row = sheet1.CreateRow(rowIndex);
            var cell4 = row.CreateCell(signCol - 1);
            cell4.SetCellValue(employeeName);
            cell4.CellStyle = attstyle;

        }

        private void CreateDetailsTitle(ISheet sheet1, IWorkbook workbook, IRow row, IFont font, ICell cell,
            int rowIndex, int colIndex)
        {
            var SalDeducStyle = workbook.CreateCellStyle();
            SalDeducStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            SalDeducStyle.FillPattern = FillPattern.SolidForeground;
            SalDeducStyle.SetFont(font);
            SalDeducStyle.Alignment = HorizontalAlignment.Center;
            SalDeducStyle.BorderBottom = BorderStyle.Thin;
            SalDeducStyle.BorderTop = BorderStyle.Thin;
            SalDeducStyle.BorderLeft = BorderStyle.Thin;
            SalDeducStyle.BorderRight = BorderStyle.Thin;
            row = sheet1.CreateRow(rowIndex);


            var mg1 = sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, colIndex, colIndex + 2));
            cell = row.CreateCell(colIndex);
            cell.SetCellValue("Percepciones");
            cell.CellStyle = SalDeducStyle;
            var cell1 = row.CreateCell(colIndex + 1);
            cell1.CellStyle = SalDeducStyle;
            var cell2 = row.CreateCell(colIndex + 2);
            cell2.CellStyle = SalDeducStyle;


            sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, colIndex + 3, colIndex + 5));
            cell = row.CreateCell(colIndex + 3);
            cell.SetCellValue("Deducciones");
            cell.CellStyle = SalDeducStyle;
            var cell3 = row.CreateCell(colIndex + 4);
            cell3.CellStyle = SalDeducStyle;
            var cell4 = row.CreateCell(colIndex + 5);
            cell4.CellStyle = SalDeducStyle;

        }

        private void CreateDetailsHeader(ISheet sheet1, IWorkbook workbook, IRow row, IFont font, int rowIndex, int colIndex)
        {
            row = sheet1.CreateRow(rowIndex);
            row.Height = 700;

            var perNomStyle = workbook.CreateCellStyle();
            perNomStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            perNomStyle.FillPattern = FillPattern.SolidForeground;
            perNomStyle.SetFont(font);
            perNomStyle.Alignment = HorizontalAlignment.Center;
            perNomStyle.BorderTop = BorderStyle.Medium;
            perNomStyle.BorderLeft = BorderStyle.Medium;

            var cell = row.CreateCell(colIndex);
            cell.SetCellValue("No.");
            cell.CellStyle = perNomStyle;

            var perConceptStyle = workbook.CreateCellStyle();
            perConceptStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            perConceptStyle.FillPattern = FillPattern.SolidForeground;
            perConceptStyle.SetFont(font);
            perConceptStyle.Alignment = HorizontalAlignment.Center;
            perConceptStyle.BorderTop = BorderStyle.Medium;
            perConceptStyle.BorderLeft = BorderStyle.None;

            var cell1 = row.CreateCell(colIndex + 1);
            cell1.CellStyle = perConceptStyle;
            cell1.SetCellValue("Concepto");


            var perTotalStyle = workbook.CreateCellStyle();
            perTotalStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            perTotalStyle.FillPattern = FillPattern.SolidForeground;
            perTotalStyle.SetFont(font);
            perTotalStyle.Alignment = HorizontalAlignment.Center;
            perTotalStyle.BorderRight = BorderStyle.Medium;
            perTotalStyle.BorderTop = BorderStyle.Medium;


            var cell2 = row.CreateCell(colIndex + 2);
            cell2.CellStyle = perTotalStyle;
            cell2.SetCellValue("Total");

            var perConceptStyle2 = workbook.CreateCellStyle();
            perConceptStyle2.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            perConceptStyle2.FillPattern = FillPattern.SolidForeground;
            perConceptStyle2.SetFont(font);
            perConceptStyle2.Alignment = HorizontalAlignment.Center;
            perConceptStyle2.BorderRight = BorderStyle.None;
            perConceptStyle2.BorderLeft = BorderStyle.None;
            perConceptStyle2.BorderTop = BorderStyle.Medium;

            var cell3 = row.CreateCell(colIndex + 3);
            cell3.CellStyle = perConceptStyle2;
            cell3.SetCellValue("No. ");

            var cell4 = row.CreateCell(colIndex + 4);
            cell4.CellStyle = perConceptStyle2;
            cell4.SetCellValue("Concepto ");

            var GreyCenteredStyleBTR = workbook.CreateCellStyle();
            GreyCenteredStyleBTR.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            GreyCenteredStyleBTR.FillPattern = FillPattern.SolidForeground;
            GreyCenteredStyleBTR.SetFont(font);
            GreyCenteredStyleBTR.Alignment = HorizontalAlignment.Center;
            GreyCenteredStyleBTR.BorderTop = BorderStyle.Medium;
            GreyCenteredStyleBTR.BorderRight = BorderStyle.Medium;

            var cell5 = row.CreateCell(colIndex + 5);
            cell5.CellStyle = GreyCenteredStyleBTR;
            cell5.SetCellValue("Total ");

            sheet1.AutoSizeColumn(0);
        }

        private void CreateRowSeparation(ISheet sheet1, IRow row, int rowIndex)
        {
            row = sheet1.CreateRow(rowIndex);
            row.Height = 70;
        }


        private int CreateOverDraftDetails(List<Overdraft> overdrafts, ISheet sheet1, IWorkbook workbook, IRow row, IFont font,
             int rowIndex, int colIndex)
        {

            var perceptions = overdrafts.SelectMany(x => x.OverdraftDetails.Where(x => x.ConceptPayment.ConceptType == ConceptType.SalaryPayment)).AsParallel().OrderBy(p => p.ConceptPayment.Code).ToList();

            var deductions = overdrafts.SelectMany(x => x.OverdraftDetails.Where(x => x.ConceptPayment.ConceptType == ConceptType.DeductionPayment)).AsParallel().OrderBy(p => p.ConceptPayment.Code).ToList();


            int laps = 0;
            var salariesCount = perceptions.Count();
            var deductionsCount = deductions.Count();
            laps = salariesCount > deductionsCount ? salariesCount : deductionsCount;

            var blStyle = workbook.CreateCellStyle();
            blStyle.BorderLeft = BorderStyle.Medium;
            var brStyle = workbook.CreateCellStyle();
            brStyle.BorderRight = BorderStyle.Medium;

            var btStyle = workbook.CreateCellStyle();
            btStyle.BorderTop = BorderStyle.Medium;

            for (int i = 0; i < laps; i++)
            {
                var CurrentRow = sheet1.CreateRow(rowIndex);
                if (i <= salariesCount - 1)
                {

                    var salaryActual = perceptions[i];
                    var cell = CurrentRow.CreateCell(colIndex + 0);
                    cell.SetCellValue(salaryActual.ConceptPayment.Code.ToString());
                    cell.CellStyle = blStyle;
                    CurrentRow.CreateCell(colIndex + 1).SetCellValue(salaryActual.ConceptPayment.Name.ToString());
                    cell = CurrentRow.CreateCell(colIndex + 2);
                    cell.SetCellValue((double)Math.Round(salaryActual.Amount, 2));
                    cell.CellStyle = brStyle;
                }
                else
                {
                    var cell = CurrentRow.CreateCell(colIndex + 0);
                    cell.SetCellValue("");
                    cell.CellStyle = blStyle;
                    CurrentRow.CreateCell(colIndex + 1).SetCellValue("");
                    cell = CurrentRow.CreateCell(colIndex + 2);
                    cell.SetCellValue("");
                    cell.CellStyle = brStyle;
                }

                if (i <= deductionsCount - 1)
                {
                    var deductionActual = deductions[i];
                    CurrentRow.CreateCell(colIndex + 3).SetCellValue(deductionActual.ConceptPayment.Code.ToString());
                    CurrentRow.CreateCell(colIndex + 4).SetCellValue(deductionActual.ConceptPayment.Name.ToString());
                    var cell = CurrentRow.CreateCell(colIndex + 5);
                    cell.SetCellValue((double)Math.Round(deductionActual.Amount, 2));
                    cell.CellStyle = brStyle;

                }
                else
                {
                    CurrentRow.CreateCell(colIndex + 3).SetCellValue("");
                    CurrentRow.CreateCell(colIndex + 4).SetCellValue("");
                    var cell = CurrentRow.CreateCell(colIndex + 5);
                    cell.SetCellValue("");
                    cell.CellStyle = brStyle;
                }
                rowIndex++;
            }

            for (int i = 0; i < 5; i++)
            {
                var CurrentRow = sheet1.CreateRow(rowIndex);
                var cell = CurrentRow.CreateCell(colIndex + 0);
                cell.CellStyle = blStyle;
                cell = CurrentRow.CreateCell(colIndex + 2);
                cell.CellStyle = brStyle;
                cell = CurrentRow.CreateCell(colIndex + 5);
                cell.CellStyle = brStyle;
                rowIndex++;
            }

            var actualRow = sheet1.CreateRow(rowIndex);

            var cells = actualRow.CreateCell(colIndex);
            cells.CellStyle = btStyle;

            cells = actualRow.CreateCell(colIndex + 1);
            cells.CellStyle = btStyle;
            cells = actualRow.CreateCell(colIndex + 2);
            cells.CellStyle = btStyle;
            cells = actualRow.CreateCell(colIndex + 3);
            cells.CellStyle = btStyle;
            cells = actualRow.CreateCell(colIndex + 4);
            cells.CellStyle = btStyle;
            cells = actualRow.CreateCell(colIndex + 5);
            cells.CellStyle = btStyle;

            return rowIndex;
        }

        private void CreateSums(ISheet sheet1, IWorkbook workbook, IRow row, IFont font, ICell cell, int rowIndex, int colIndex, OverdraftTotalsResult totals)
        {
            var SalDeducStyle = workbook.CreateCellStyle();
            SalDeducStyle.SetFont(font);
            SalDeducStyle.BorderBottom = BorderStyle.Medium;
            SalDeducStyle.BorderTop = BorderStyle.Medium;
            SalDeducStyle.BorderLeft = BorderStyle.Medium;
            SalDeducStyle.BorderRight = BorderStyle.Medium;


            row = sheet1.CreateRow(rowIndex);

            sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, colIndex, colIndex + 1));
            cell = row.CreateCell(colIndex);
            cell.SetCellValue("Suma de Percepciones $");
            cell.CellStyle = SalDeducStyle;


            var cell2 = row.CreateCell(colIndex + 1);
            cell2.SetCellValue((double)totals.TotalSalaryPayments);
            cell2.CellStyle = SalDeducStyle;


            var cell3 = row.CreateCell(colIndex + 2);
            cell3.SetCellValue((double)totals.TotalSalaryPayments);
            cell3.CellStyle = SalDeducStyle;

            sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, colIndex + 3, colIndex + 4));
            var cell4 = row.CreateCell(colIndex + 3);
            cell4.SetCellValue("Suma de Deducciones $");
            cell4.CellStyle = SalDeducStyle;

            var cell5 = row.CreateCell(colIndex + 4);
            cell5.CellStyle = SalDeducStyle;

            var cell6 = row.CreateCell(colIndex + 5);
            cell6.SetCellValue((double)totals.TotalDeductionPayments);
            cell6.CellStyle = SalDeducStyle;


        }

        private void CreateNetPayment(ISheet sheet1, IWorkbook workbook, IRow row, IFont font, ICell cell, int rowIndex, int colIndex, OverdraftTotalsResult totals)
        {
            var SalDeducStyle = workbook.CreateCellStyle();
            SalDeducStyle.SetFont(font);
            SalDeducStyle.BorderBottom = BorderStyle.Medium;
            SalDeducStyle.BorderTop = BorderStyle.Medium;
            SalDeducStyle.BorderLeft = BorderStyle.Medium;
            SalDeducStyle.BorderRight = BorderStyle.Medium;


            row = sheet1.CreateRow(rowIndex);

            sheet1.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, colIndex + 3, colIndex + 4));
            var cell4 = row.CreateCell(colIndex + 3);
            cell4.SetCellValue("Neto a pagar $");
            cell4.CellStyle = SalDeducStyle;

            var cell5 = row.CreateCell(colIndex + 4);
            cell5.CellStyle = SalDeducStyle;

            var cell6 = row.CreateCell(colIndex + 5);
            cell6.SetCellValue((double)totals.Total);
            cell6.CellStyle = SalDeducStyle;


        }
    }
}

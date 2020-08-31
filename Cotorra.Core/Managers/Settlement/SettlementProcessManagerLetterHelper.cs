using MoreLinq.Extensions;
using Cotorra.Core.Managers;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class SettlementProcessManagerLetterHelper : ISettlementProcessManagerLetterHelper
    {

        /// <summary>
        /// Generates the settlement letter.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async Task<string> GenerateSettlementLetter(GenerateSettlementLetterParams parameters, IMSSpreadsheetWriter writer)
        {
            var activeOverdraft = await GetOverdraftByIDs(parameters.OverdraftIDs, parameters.IdentityWorkID);

            return await GenerateSettlementLetter(activeOverdraft, parameters.IdentityWorkID, parameters.InstanceID,
                parameters.Token, writer);
        }

        /// <summary>
        /// Generates the settlement letter indicates writer.
        /// </summary>
        /// <param name="activeOverdraft">The active overdraft.</param>
        /// <param name="identityWorkID">The identity work identifier.</param>
        /// <param name="instanceID">The instance identifier.</param>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        public async Task<string> GenerateSettlementLetter(List<Overdraft> activeOverdrafts, Guid identityWorkID, Guid instanceID, string token, IMSSpreadsheetWriter writer)
        {
            var instanceMgr = new InstanceManager();
            var instance = await instanceMgr.GetByIDAsync(token, instanceID);

            var payrrollCompanyConfigurationMgr = new MiddlewareManager<PayrollCompanyConfiguration>(
                        new BaseRecordManager<PayrollCompanyConfiguration>(),
                        new PayrollCompanyConfigurationValidator());

            var address = (await payrrollCompanyConfigurationMgr.FindByExpressionAsync(x => x.InstanceID == instanceID,
                identityWorkID, new string[] { "Address" })).FirstOrDefault().Address;
            var fullAddress = string.Empty;
            if (address != null)
            {
                fullAddress = $" {address.Street}, {address.ZipCode}, {address.Municipality}, {address.FederalEntity}";
            }

            var overdraftManager = new OverdraftManager();
            OverdraftTotalsResult totals = new OverdraftTotalsResult();

            activeOverdrafts.ForEach(activeOverdraft =>
            {
                var totalSalaryPayments = activeOverdraft.OverdraftDetails.Where(p => p.ConceptPayment.ConceptType == ConceptType.SalaryPayment &&
                       p.ConceptPayment.Print &&
                       !p.ConceptPayment.Kind).
               Select(y => y.Amount).Sum();



                var totalDeductionPayments = activeOverdraft.OverdraftDetails.Where(p => p.ConceptPayment.ConceptType == ConceptType.DeductionPayment &&
                        p.ConceptPayment.Print &&
                        !p.ConceptPayment.Kind).
                Select(y => y.Amount).Sum();


                totals.TotalSalaryPayments += Math.Round(totalSalaryPayments, 2);
                totals.TotalDeductionPayments += Math.Round(totalDeductionPayments, 2);
                totals.Total += Math.Round(totals.TotalSalaryPayments - totals.TotalDeductionPayments, 2);

            });

            string header =
                $"Recibí de la empresa {instance.Name} con domicilio en \n" +
                $"{fullAddress} " +
                $"la cantidad de: \n";
            string header2 = " por concepto de mi finiquito con motivo de la terminación\n" +
                "de mi relación laboral con la empresa con fecha indicada en el documento, cantidad \n" +
                "que resulta de los siguientes conceptos:";

            string footer =
                "Así mismo manifiesto que hasta el momento en que se da por terminada la relación laboral\n" +
                 "no se me adeuda ninguna cantidad de dinero por concepto de salarios devengados,\n" +
                 "diferencia de los mismos, participacion de utilidades, comisiones, horas extras, \n" +
                 "vacaciones, septimos días, días festivos, prima dominical, vacacional y de antigüedad\n" +
                 "y demás prestaciones que otorga la Ley Federal Del Trabajo, ya que las mismas siempre\n" +
                 "me fueron íntegramente cubiertas en los términos de la ley. También hago constar para\n" +
                 "todos los efectos legales conducentes, que durante la vigencia de mi relación\n" +
                 "obrero-patronal no fui objeto de riesgo profesional alguno, motivo por el cual libero\n" +
                "a mi patron de toda responsabilidad laboral y de seguridad\n" +
                "social o de cualquier otro concpeto derivado del contrato de trabajo.";
            string date = DateTime.Now.ToString("MM/dd/yyyy");



            var newHeader = ConcatenateHeader(header, header2, totals);
            var filename = "CartaFiniquito - " + activeOverdrafts.FirstOrDefault().Employee.FullName + ".xlsx";

            IWorkbook wb = GenerateSettlementLetter(activeOverdrafts, newHeader, footer, date, activeOverdrafts.FirstOrDefault().Employee.FullName, totals);
            var url = await WriteSettlementLetterAsync(instanceID, filename, wb, writer);

            return url;
        }

        private string ConcatenateHeader(string header, string header2, OverdraftTotalsResult totals)
        {
            CurrencyUtil cur = new CurrencyUtil();
            var stringQuantity = cur.ToText(new CurrencyToTextParams() { CurrencyAmount = Math.Round(totals.Total, 2), CurrencyCode = "MXN", CurrencyName = "Pesos" });
            return header + "$ " + Math.Round(totals.Total, 2) + " " + stringQuantity + header2;
        }

        private IWorkbook GenerateSettlementLetter(List<Overdraft> overdrafts, string header, string footer, string date, string employeeName,
            OverdraftTotalsResult totals)
        {
            SettlementLetterMSSpreadsheet builder = new SettlementLetterMSSpreadsheet();
            return builder.GenerateSettlementLetter(overdrafts, header, footer, date, employeeName, totals);
        }

       
        private async Task<List<Overdraft>> GetOverdraftByIDs(List<Guid> ids, Guid identityWorkID)
        {
            var manager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
            var overdrafts = await manager.FindByExpressionAsync(p =>
                ids.Contains(p.ID) &&
                p.company == identityWorkID &&
                p.Active,
                identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                     });

            return overdrafts;
        }

        private async Task<string> WriteSettlementLetterAsync(Guid instanceID, string FileName, IWorkbook workbook, IMSSpreadsheetWriter writer)
        {
            return await writer.WriteAsync(instanceID, FileName, workbook);
        }

    }

}

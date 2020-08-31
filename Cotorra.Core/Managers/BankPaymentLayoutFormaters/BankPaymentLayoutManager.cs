using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class BankPaymentLayoutManager
    {
        public async Task<string> GenerateBankLayoutPeriod(BankLayoutPaymentInformation bankLayoutPaymentInformation)
        {
            var fileName = bankLayoutPaymentInformation.BankName + bankLayoutPaymentInformation.PeriodDescription.Replace(" ", String.Empty) + ".txt";
            //obtener overdrafts
            var statusOverdraft = new List<OverdraftStatus>()
            {
                OverdraftStatus.Authorized,
                OverdraftStatus.None,
                OverdraftStatus.Stamped
            };

            var overdrafts = await GetOverdraftsAsync(bankLayoutPaymentInformation.InstanceID, bankLayoutPaymentInformation.IdentityWorkID, 
                    bankLayoutPaymentInformation.PeriodDetailID, statusOverdraft, Int32.Parse(bankLayoutPaymentInformation.BankCode));

            if (overdrafts.Count <= 0)
            {
                throw new CotorraException(801, "801", "No hay colaboradores con el banco seleccionado."+Environment.NewLine+"Asegurate de asignar el banco y cuenta correspondiente desde Colaboradores.", null);
            }
            
            //genera objeto para generar layout
            (var bankLayoutManager, var additionalInformation) = await new BankPaymentLayoutManagerFactory().GetBankPaymentLayoutManager(bankLayoutPaymentInformation);

            //genera layout string 
            var layoutString = bankLayoutManager.GenerateLayout(overdrafts, additionalInformation);

            //escribir en bloob
            var urlLayout = await new TXTWriterBlob().WriteAsync(bankLayoutPaymentInformation.InstanceID, fileName, layoutString);

            //regresar string uri
            return urlLayout;
        }

        private async Task<List<Overdraft>> GetOverdraftsAsync(Guid instanceID, Guid identityWorkID, Guid periodDetailID, List<OverdraftStatus> statusOverdrafts, int bankCode)
        {
            var manager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());


            var overdrafts = await manager.FindByExpressionAsync(x => x.InstanceID == instanceID 
                && x.OverdraftType == OverdraftType.Ordinary && statusOverdrafts.Contains(x.OverdraftStatus) 
                && x.PeriodDetailID == periodDetailID && x.Employee.Bank.Code == bankCode,
                  identityWorkID, new string[] { "OverdraftDetails", "OverdraftDetails.ConceptPayment", "Employee"});

            return overdrafts;
        }
    }
}

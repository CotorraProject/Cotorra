using Cotorra.Schema;
using NPOI.OpenXmlFormats.Dml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class BankPaymentLayoutManagerFactory
    {
        private const string BANAMEX = "2";
        private const string BANORTE = "72";
        private const string SANTANDER = "14";
        private const string BBVA = "12";
        private const string SCOTIABANK = "44";
        private const string HSBC = "21";

        public async Task<(IBankPaymentLayoutManager, IBankAdditionalInformation)> GetBankPaymentLayoutManager(BankLayoutPaymentInformation layoutInformation)
        {
            var instanceMgr = new InstanceManager();
            var instance = await instanceMgr.GetByIDAsync(layoutInformation.TokeUser, layoutInformation.InstanceID);

            switch(layoutInformation.BankCode)
            {
                case BANAMEX:
                    layoutInformation.BankExtraParams.Banamex.CompanyName = instance.Name;
                    return (new BanamexPaymentLayoutManager(), layoutInformation.BankExtraParams.Banamex);
                case BANORTE:
                    layoutInformation.BankExtraParams.Banorte.CompanyName = instance.Name;
                    return (new BanortePaymentLayoutManager(), layoutInformation.BankExtraParams.Banorte);
                case SANTANDER:
                    return (new SantanderPaymentLayoutManager(), layoutInformation.BankExtraParams.Santander);
                case BBVA:
                    return (new BBVAPaymentLayoutManager(), null);
                case SCOTIABANK:
                    return (new ScotiabankPaymentLayoutManager(), layoutInformation.BankExtraParams.Scotiabank);
                case HSBC:
                    return (new HSBCPaymentLayoutManager(), null);
                default:
                    throw new ArgumentException("Banco no manejado para la dispersión");
            }
        }
    }
}

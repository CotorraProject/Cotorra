using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Cotorra.Core.Utils
{
    public class CatalogSATUtil
    {
        private Guid _identityWorkID;
        private Guid _instanceID;
        private IEnumerable<PayrollCompanyConfiguration> _payrollCompanyConfiguration;
        private IEnumerable<Overdraft> _overdrafts;
        private Dictionary<string, string> _fiscalRegimeDictionary;

        public CatalogSATUtil(Guid identityWorkID, Guid instanceID, IEnumerable<Overdraft> overdrafts, IEnumerable<PayrollCompanyConfiguration> payrollCompanyConfigurations)
        {
            _identityWorkID = identityWorkID;
            _instanceID = instanceID;
            _overdrafts = overdrafts;
            _payrollCompanyConfiguration = payrollCompanyConfigurations;

            _fiscalRegimeDictionary = new Dictionary<string, string>();
            _fiscalRegimeDictionary.Add("601", "General de Ley Personas Morales");
            _fiscalRegimeDictionary.Add("603", "Personas Morales con Fines no Lucrativos");
            _fiscalRegimeDictionary.Add("605", "Sueldos y Salarios e Ingresos Asimilados a Salarios");
            _fiscalRegimeDictionary.Add("606", "Arrendamiento");
            _fiscalRegimeDictionary.Add("607", "Régimen de Enajenación o Adquisición de Bienes");
            _fiscalRegimeDictionary.Add("608", "Demás ingresos");
            _fiscalRegimeDictionary.Add("609", "Consolidación");
            _fiscalRegimeDictionary.Add("610", "Residentes en el Extranjero sin Establecimiento Permanente en México");
            _fiscalRegimeDictionary.Add("611", "Ingresos por Dividendos (socios y accionistas)");
            _fiscalRegimeDictionary.Add("612", "Personas Físicas con Actividades Empresariales y Profesionales");
            _fiscalRegimeDictionary.Add("614", "Ingresos por intereses");
            _fiscalRegimeDictionary.Add("615", "Régimen de los ingresos por obtención de premios");
            _fiscalRegimeDictionary.Add("616", "Sin obligaciones fiscales");
            _fiscalRegimeDictionary.Add("620", "Sociedades Cooperativas de Producción que optan por diferir sus ingresos");
            _fiscalRegimeDictionary.Add("621", "Incorporación Fiscal");
            _fiscalRegimeDictionary.Add("622", "Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras");
            _fiscalRegimeDictionary.Add("623", "Opcional para Grupos de Sociedades");
            _fiscalRegimeDictionary.Add("624", "Coordinados");
            _fiscalRegimeDictionary.Add("628", "Hidrocarburos");
            _fiscalRegimeDictionary.Add("629", "De los Regímenes Fiscales Preferentes y de las Empresas Multinacionales");
            _fiscalRegimeDictionary.Add("630", "Enajenación de acciones en bolsa de valores");

            _fiscalRegimeDictionary.Add("0", "No especificado");
        }

        public string GetDescriptionFiscalProductByCode(string code)
        {
            return string.Empty;
        }

        public string GetDescriptionFiscalUnitByCode(string code)
        {
            return string.Empty;
        }

        public string GetDescriptionDocumentTypeByCode(string code)
        {
            return string.Empty;
        }

        public string GetTaxRegimeByCode(string overdraftID, string code)
        {
            var fiscalRegime = _payrollCompanyConfiguration.FirstOrDefault().FiscalRegime;
            _fiscalRegimeDictionary.TryGetValue(((int)fiscalRegime).ToString(), out string strTraduction);
            return strTraduction;
        }

        public string GetInvoicePurposeByCode(string code)
        {
            return string.Empty;
        }

        public string GetPaymentFormByCode(string code)
        {
            return "Por definir";
        }

        public string GetPaymentMethodByCode(string code)
        {
            return "Pago en una sola exhibición";
        }

        public string GetCurrencyByCode(string code)
        {
            return string.Empty;
        }

        public string GetTypeRelationShipByCode(string code)
        {
            return string.Empty;
        }

        public string GetStateByZipCode(string code)
        {
            return string.Empty;
        }

        public string GetLocalityByZipCode(string code)
        {
            return string.Empty;
        }

        public string GetTownShipByZipCode(string code)
        {
            return string.Empty;
        }

        public string GetPaymentPeriodByCode(string code)
        {
            Dictionary<string, string> paymentPeriodicityDictionary;
            paymentPeriodicityDictionary = new Dictionary<string, string>();
            //Bimestral
            paymentPeriodicityDictionary.Add("06", "Bi-Mensual");
            //Quincenal
            paymentPeriodicityDictionary.Add("04", "Quincenal");
            //Comisión
            paymentPeriodicityDictionary.Add("08", "Comisión");
            //Diario
            paymentPeriodicityDictionary.Add("01", "Diario");
            //Decenal
            paymentPeriodicityDictionary.Add("10", "Decenal");
            //Precio alzado
            paymentPeriodicityDictionary.Add("09", "Precio Alzado");
            //Catorcenal
            paymentPeriodicityDictionary.Add("03", "Catorcenal");
            //Mes
            paymentPeriodicityDictionary.Add("05", "Mensual");
            //Semanal
            paymentPeriodicityDictionary.Add("02", "Semanal");
            //Unidad de obra
            paymentPeriodicityDictionary.Add("07", "Unidad de obra");
            //Otra periodicidad
            paymentPeriodicityDictionary.Add("99", "Otra periodicidad");

            if (paymentPeriodicityDictionary.TryGetValue(code, out string result))
            {
                return result;
            }
            else
            {
                return "99 - Otra periodicidad";
            }
        }

        public string GetTypeDayByCode(string overdraftID, string code)
        {
            var workshiftDescription = _overdrafts.FirstOrDefault(p => p.ID == Guid.Parse(overdraftID)).HistoricEmployee.WorkshiftDescription;
            return workshiftDescription;
        }

        public string GetTypeDeductionByCode(string code)
        {
            return string.Empty;
        }

        public string GetTypePerceptionByCode(string code)
        {
            return string.Empty;
        }
        public string GetOthersDescription(string node, string key)
        {
            //catalog
            return GetDescription(null, node, key);
        }

        /// <summary>
        /// Metodo para buscar en catalogo del sat en base a pathNode y claveSat
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetDescription(XDocument xml, string node, string key)
        {
            if (xml != null)
            {
                string path = node + "[@clave='" + key + "']";
                var elemt = ((IEnumerable<object>)xml.XPathEvaluate(path)).OfType<XElement>().FirstOrDefault();
                if (elemt != null)
                {
                    return elemt.Attribute("Descripcion").Value;
                }
            }
            return string.Empty;
        }

        private string GetCatalogSAT()
        {
            Byte[] ct = File.ReadAllBytes(@".\Resources\" + "catalogoCFDI33.xml");
            string catalog = Encoding.UTF8.GetString(ct);
            return catalog;
        }
    }
}

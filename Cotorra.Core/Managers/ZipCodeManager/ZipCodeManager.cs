using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Cotorra.Core
{
    public class ZipCodeManager
    {
        private readonly List<catCFDI_CodigoPostal> _catCFDI_CodigoPostals;

        public ZipCodeManager(List<catCFDI_CodigoPostal> catCFDI_CodigoPostals)
        {
            _catCFDI_CodigoPostals = catCFDI_CodigoPostals;
        }

        public async Task<(string, DateTime)> GetZipCode(Overdraft overdraft, PayrollCompanyConfiguration payrollCompanyConfiguration)
        {
            var zipCode = String.Empty;
          
            if (!String.IsNullOrEmpty(overdraft.HistoricEmployee.EmployerRegistrationZipCode))
            {
                zipCode = overdraft.HistoricEmployee.EmployerRegistrationZipCode;
            }
            else if (!String.IsNullOrEmpty(payrollCompanyConfiguration.Address?.ZipCode))
            {
                zipCode = payrollCompanyConfiguration.Address?.ZipCode;
            }
            else
            {
                zipCode = "44600";
            }

            return (zipCode, await getDateByZipCode(zipCode));
        }

        public async Task<(string, DateTime)> GetZipCode(string zipCode)
        {
            return (zipCode, await getDateByZipCode(zipCode));
        }

        private  async Task<DateTime> getDateByZipCode(string getDateByZipCode)
        {
            var dateTimeNow = DateTime.Now;
            var zipCode = _catCFDI_CodigoPostals.FirstOrDefault(p => p.c_CodigoPostal == getDateByZipCode);

            Dictionary<string, string> timeZones = new Dictionary<string, string>();
            timeZones.Add("Tiempo del Centro", "Central Standard Time (Mexico)");
            timeZones.Add("Tiempo del Pacífico", "Pacific Standard Time (Mexico)");
            timeZones.Add("Tiempo del Pacífico en Frontera", "Pacific Standard Time");
            timeZones.Add("Tiempo del Noroeste", "Mountain Standard Time (Mexico)");
            timeZones.Add("Tiempo del Pacífico Sonora", "Pacific Standard Time (Mexico)");
            timeZones.Add("Tiempo del Centro en Frontera", "Central Standard Time");
            timeZones.Add("Tiempo del Noroeste en Frontera", "Pacific Standard Time");
            timeZones.Add("Tiempo del Sureste", "Eastern Standard Time");

            timeZones.TryGetValue(zipCode.DescripcionHusoHorario, out string timeZone);
            TimeZoneInfo tzi = TZConvert.GetTimeZoneInfo(timeZone);
            var result = TimeZoneInfo.ConvertTime(dateTimeNow, tzi);

            //Fix por problemas de sincronización del PAC
            result = result.AddMinutes(-5);

            return result;
        }
    }
}

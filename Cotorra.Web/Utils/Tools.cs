using Microsoft.Extensions.Caching.Memory;
using Cotorra.Client;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Web.Utils
{
    public static class CotorraTools
    {
        public static IMemoryCache MemoryCache { get; set; }

        public static DateTime ValidateDate(String clientDate)
        {
            var result = DateTime.ParseExact(clientDate, new string[] { "dd/MM/yyyy", "dd/MM/yy" }, System.Globalization.CultureInfo.InvariantCulture);
            return result;
        }

        public static DateTime GetMexicoCentralTime()
        {
            var mexicoTimeZone = "Central Standard Time (Mexico)";
            var convertedDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, mexicoTimeZone);
            return convertedDateTime;
        }

        public static async Task<DateTime?> GetDateByCompanyZipCode()
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                var clientPCC = new Client<PayrollCompanyConfiguration>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
                var clientCP = new Client<catCFDI_CodigoPostal>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
                var payrollCompanyConfig = await clientPCC.FindAsync(x => x.InstanceID == SessionModel.InstanceID, SessionModel.CompanyID, new String[] { "Address" });
                var companyZipCode = payrollCompanyConfig.FirstOrDefault().Address.ZipCode;
                var catCFDI = await clientCP.FindAsync(x => x.c_CodigoPostal == companyZipCode, SessionModel.CompanyID);
                var zipCodeTimeZone = catCFDI.FirstOrDefault();
                var difference = -5;

                //When both differences are equal, just get the realdate
                if (zipCodeTimeZone.Diferencia_Horaria_Verano == zipCodeTimeZone.Diferencia_Horaria_Invierno)
                {
                    difference = Int32.Parse(zipCodeTimeZone.Diferencia_Horaria_Verano);
                    return utcNow.AddHours(difference);
                }
                else
                {
                    var actualYear = utcNow.Year;
                    var exit = false;

                    //Get number of months
                    var winterMonth = CotorraTools.GetMonthNumber(zipCodeTimeZone.Mes_Inicio_Horario_Invierno);
                    var summerMonth = CotorraTools.GetMonthNumber(zipCodeTimeZone.Mes_Inicio_Horario_Verano);

                    //Get sunday position
                    var winterSunday = 0;
                    switch (zipCodeTimeZone.Dia_Inicio_Horario_Invierno.ToLower())
                    {
                        case "primer domingo": winterSunday = 1; break;
                        case "segundo domingo": winterSunday = 2; break;
                    }

                    var summerSunday = 0;
                    switch (zipCodeTimeZone.Dia_Inicio_Horario_Verano.ToLower())
                    {
                        case "primer domingo": summerSunday = 1; break;
                        case "último domingo": summerSunday = 0; break;
                    }

                    //Get first winter (initial)
                    var winter1 = new DateTime(actualYear - 1, winterMonth, 1, 2, 0, 0);
                    exit = false;
                    do
                    {
                        if (winter1.DayOfWeek == DayOfWeek.Sunday && (Convert.ToInt32(winter1.Day / 7) + 1 == winterSunday)) { exit = true; }
                        else { winter1 = winter1.AddDays(1); }
                    } while (!exit);

                    //Get first summer
                    var summer1 = new DateTime(actualYear - 0, summerSunday != 0 ? summerMonth : summerMonth + 1, 1, 2, 0, 0);
                    if (summerSunday == 0) { summer1.AddDays(-1); }

                    exit = false;
                    do
                    {
                        //Not last sunday
                        if (summerSunday != 0)
                        {
                            if (summer1.DayOfWeek == DayOfWeek.Sunday && (Convert.ToInt32(summer1.Day / 7) + 1 == summerSunday)) { exit = true; }
                            else { summer1 = summer1.AddDays(1); }
                        }
                        //Last sunday
                        else
                        {
                            if (summer1.DayOfWeek == DayOfWeek.Sunday) { exit = true; }
                            else { summer1 = summer1.AddDays(-1); }
                        }
                    } while (!exit);

                    //Get second winter
                    var winter2 = new DateTime(actualYear - 0, winterMonth, 1, 2, 0, 0);
                    exit = false;
                    do
                    {
                        if (winter2.DayOfWeek == DayOfWeek.Sunday && (Convert.ToInt32(winter2.Day / 7) + 1 == winterSunday)) { exit = true; }
                        else { winter2 = winter2.AddDays(1); }
                    } while (!exit);

                    //Get second summer (final)
                    var summer2 = new DateTime(actualYear + 2, summerSunday != 0 ? summerMonth : summerMonth + 1, 1, 2, 0, 0);
                    if (summerSunday == 0) { summer2.AddDays(-1); }

                    exit = false;
                    do
                    {
                        //Not last sunday
                        if (summerSunday != 0)
                        {
                            if (summer2.DayOfWeek == DayOfWeek.Sunday && (Convert.ToInt32(summer2.Day / 7) + 1 == summerSunday)) { exit = true; }
                            else { summer2 = summer2.AddDays(1); }
                        }
                        //Last sunday
                        else
                        {
                            if (summer2.DayOfWeek == DayOfWeek.Sunday) { exit = true; }
                            else { summer2 = summer2.AddDays(-1); }
                        }
                    } while (!exit);

                    //Create possible dates
                    var possibleWinterDate = utcNow.AddDays(Int32.Parse(zipCodeTimeZone.Diferencia_Horaria_Invierno));
                    var possibleSummerDate = utcNow.AddDays(Int32.Parse(zipCodeTimeZone.Diferencia_Horaria_Verano));

                    //Compare with all ranges
                    if ((possibleWinterDate >= winter1) && (possibleWinterDate < summer1) && (possibleSummerDate >= winter1) && (possibleSummerDate < summer1))
                    {
                        //Winter
                        utcNow.AddHours(Int32.Parse(zipCodeTimeZone.Diferencia_Horaria_Invierno));
                    }
                    else if ((possibleWinterDate >= summer1) && (possibleWinterDate < winter2) && (possibleSummerDate >= summer1) && (possibleSummerDate < winter2))
                    {
                        //Summer
                        utcNow.AddHours(Int32.Parse(zipCodeTimeZone.Diferencia_Horaria_Verano));
                    }
                    else if ((possibleWinterDate >= winter2) && (possibleWinterDate < summer2) && (possibleSummerDate >= winter2) && (possibleSummerDate < summer2))
                    {
                        //Winter
                        utcNow.AddHours(Int32.Parse(zipCodeTimeZone.Diferencia_Horaria_Invierno));
                    }
                    else
                    {
                        //Summer
                        utcNow.AddHours(Int32.Parse(zipCodeTimeZone.Diferencia_Horaria_Verano));
                    }

                    return utcNow;
                }


            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Int32 GetMonthNumber(String monthName)
        {
            var months = new Dictionary<String, Int32>()
            {
                { "enero", 1 },
                { "febrero", 2 },
                { "marzo", 3 },
                { "abril", 4 },
                { "mayo", 5 },
                { "junio", 6 },
                { "julio", 7 },
                { "agosto", 8 },
                { "septiembre", 9 },
                { "octubre", 10 },
                { "noviembre", 11 },
                { "diciembre", 12 },
            };

            return months.GetValueOrDefault(monthName.ToLower());
        }
    }
}

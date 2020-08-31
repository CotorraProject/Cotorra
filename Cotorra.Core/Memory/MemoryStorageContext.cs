using AutoMapper;
using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class MemoryStorageContext : DbContext
    {
        private readonly IMapper _mapper;

        public MemoryStorageContext()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IdentityCatalogEntityExt, MinimunSalary>();
                cfg.CreateMap<IdentityCatalogEntityExt, BenefitType>();
                cfg.CreateMap<IdentityCatalogEntityExt, AnualIncomeTax>();
                cfg.CreateMap<IdentityCatalogEntityExt, MonthlyIncomeTax>();
                cfg.CreateMap<IdentityCatalogEntityExt, IMSSFare>();
                cfg.CreateMap<IdentityCatalogEntityExt, Workshift>();
                cfg.CreateMap<IdentityCatalogEntityExt, ConceptPayment>();
                cfg.CreateMap<IdentityCatalogEntityExt, AccumulatedType>();
                cfg.CreateMap<IdentityCatalogEntityExt, IncidentType>();
                cfg.CreateMap<IdentityCatalogEntityExt, IncidentTypeRelationship>();
                cfg.CreateMap<IdentityCatalogEntityExt, EmployerRegistration>();
                cfg.CreateMap<IdentityCatalogEntityExt, PeriodType>();
                cfg.CreateMap<IdentityCatalogEntityExt, ConceptPaymentRelationship>();
                cfg.CreateMap<IdentityCatalogEntityExt, SGDFLimits>();
                cfg.CreateMap<IdentityCatalogEntityExt, IMSSEmployeeTable>();
                cfg.CreateMap<IdentityCatalogEntityExt, IMSSEmployerTable>();
                cfg.CreateMap<IdentityCatalogEntityExt, IMSSWorkRisk>();
                cfg.CreateMap<IdentityCatalogEntityExt, MonthlyEmploymentSubsidy>();
                cfg.CreateMap<IdentityCatalogEntityExt, AnualEmploymentSubsidy>();
                cfg.CreateMap<IdentityCatalogEntityExt, UMA>();
                cfg.CreateMap<IdentityCatalogEntityExt, SettlementCatalog>();
                cfg.CreateMap<BaseEntity, UMI>();
                cfg.CreateMap<BaseEntity, InfonavitInsurance>();
            });

            _mapper = config.CreateMapper();
        }

        public MemoryStorageContext(DbContextOptions<MemoryStorageContext> options)
            : base(options)
        { }

        private T CreateDefaultObject<T>(Guid id, string name = "", string description = "")
            where T : IdentityCatalogEntity
        {

            var result = new IdentityCatalogEntityExt()
            {
                Active = true,
                CreationDate = DateTime.Now,
                DeleteDate = null,
                ID = id,
                StatusID = 1,
                Timestamp = DateTime.Now,
                Name = name,
                Description = description
            };

            return _mapper.Map<IdentityCatalogEntityExt, T>(result);
        }

        private T CreateDefaultObjectEntity<T>(Guid id)
            where T : BaseEntity
        {
            var result = new BaseEntity()
            {
                Active = true,
                DeleteDate = null,
                ID = id,
                Timestamp = DateTime.Now,
            };

            return _mapper.Map<BaseEntity, T>(result);
        }

        private string[] splitRow(string content, string sepatator = "\r\n")
        {
            return content.Split(sepatator, StringSplitOptions.None);
        }

        private string[] splitColumn(string content, string sepatator = "|")
        {
            return content.Split(sepatator, StringSplitOptions.None);
        }

        public List<Schema.MinimunSalary> GetDefaultMinimunSalaries(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<MinimunSalary>();
            const int DATE = 0;
            const int ZONEA = 1;
            const int ZONEB = 2;
            const int ZONEC = 3;

            string input = @"01/01/1998|30.2|28.9|26.05
01/01/1999|34.45|31.9|29.72
01/01/2000|37.9|35.1|32.7
01/01/2001|40.35|37.95|35.85
01/01/2002|42.15|40.1|38.3
01/01/2003|43.65|41.85|40.3
01/01/2004|45.24|43.73|42.11
01/01/2005|46.8|45.35|44.05
01/01/2006|48.67|47.16|45.81
01/01/2007|50.57|49|47.6
01/01/2008|52.59|50.96|49.5
01/01/2009|54.8|53.26|51.95
01/01/2010|57.46|55.84|54.47
01/01/2011|59.82|58.13|56.7
01/01/2012|62.33|60.57|59.08
27/11/2012|62.33|59.08|59.08
01/01/2013|64.76|61.38|61.38
01/01/2014|67.29|63.77|63.77
01/01/2015|70.1|66.45|66.45
01/04/2015|70.1|68.28|68.28
01/10/2015|70.1|70.1|70.1
01/01/2016|73.04|73.04|73.04
01/01/2017|80.04|80.04|80.04
01/12/2017|88.36|88.36|88.36
01/01/2019|102.68|102.68|176.72
01/01/2020|123.22|123.22|185.56";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();
                var date = DateTime.ParseExact(superSplitted[DATE], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var zoneA = decimal.Parse(superSplitted[ZONEA]);
                var zoneB = decimal.Parse(superSplitted[ZONEB]);
                var zoneC = decimal.Parse(superSplitted[ZONEC]);

                result.Add(
                     CreateDefaultObject<MinimunSalary>(id)
                        .SetData(date, zoneA, zoneB, zoneC)
                        .SetOwnerData(companyId, instanceId, user)
                );
            }

            return result;
        }

        public List<Schema.BenefitType> GetDefaultBenefitType(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<BenefitType>();

            string input =
                @"1|6|25|15|1.0452|0
                 2|8|25|15|1.0466|0
                 3|10|25|15|1.0479|0
                 4|12|25|15|1.0493|0
                 5|14|25|15|1.0507|0
                 6|14|25|15|1.0507|0
                 7|14|25|15|1.0507|0
                 8|14|25|15|1.0507|0
                 9|14|25|15|1.0507|0
                10|16|25|15|1.0521|0
                11|16|25|15|1.0521|0
                12|16|25|15|1.0521|0
                13|16|25|15|1.0521|0
                14|16|25|15|1.0521|0
                15|18|25|15|1.0534|0
                16|18|25|15|1.0534|0
                17|18|25|15|1.0534|0
                18|18|25|15|1.0534|0
                19|18|25|15|1.0534|0
                20|20|25|15|1.0548|0
                21|20|25|15|1.0548|0
                22|20|25|15|1.0548|0
                23|20|25|15|1.0548|0
                24|20|25|15|1.0548|0
                25|22|25|15|1.0562|0
                26|22|25|15|1.0562|0
                27|22|25|15|1.0562|0
                28|22|25|15|1.0562|0
                29|22|25|15|1.0562|0
                30|24|25|15|1.0575|0
                31|24|25|15|1.0575|0
                32|24|25|15|1.0575|0
                33|24|25|15|1.0575|0
                34|24|25|15|1.0575|0
                35|26|25|15|1.0589|0
                36|26|25|15|1.0589|0
                37|26|25|15|1.0589|0
                38|26|25|15|1.0589|0
                39|26|25|15|1.0589|0
                40|28|25|15|1.0603|0
                41|28|25|15|1.0603|0
                42|28|25|15|1.0603|0
                43|28|25|15|1.0603|0
                44|28|25|15|1.0603|0
                45|30|25|15|1.0616|0";

            var splitted = splitRow(input);

            var names = new String[] { "Personalizada", "De ley" };
            for (int j = 0; j < names.Length; j++)
            {
                for (int i = 0; i < splitted.Length; i++)
                {
                    var superSplitted = splitColumn(splitted[i]);

                    var id = Guid.NewGuid();

                    var seniority = CreateDefaultObject<BenefitType>(id);
                    seniority.company = companyId;
                    seniority.InstanceID = instanceId;
                    seniority.user = user;
                    seniority.Name = names[j];
                    seniority.Antiquity = int.Parse(superSplitted[0]);

                    //Holidays - Dias de vacaciones
                    seniority.Holidays = decimal.Parse(superSplitted[1]);

                    //HolidayPremiumPortion - Porc. prima vacacional
                    seniority.HolidayPremiumPortion = decimal.Parse(superSplitted[2]);

                    //DaysOfChristmasBonus - Días de aguinaldo
                    seniority.DaysOfChristmasBonus = decimal.Parse(superSplitted[3]);

                    //IntegrationFactor - Factor de integración
                    seniority.IntegrationFactor = decimal.Parse(superSplitted[4]);

                    //DaysOfAntiquity - Días de antigüedad
                    seniority.DaysOfAntiquity = decimal.Parse(superSplitted[5]);

                    result.Add(seniority);
                }
            }

            return result;
        }

        public List<Schema.SGDFLimits> GetDefaultSGDFLimits(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<SGDFLimits>();

            string input =
                @"01/07/1997|25|1|25|25|15|15|25|25|25
01/07/1998|25|1|25|25|16|16|25|25|25
01/07/1999|25|1|25|25|17|17|25|25|25
01/07/2000|25|1|25|25|18|18|25|25|25
01/07/2001|25|1|25|25|19|19|25|25|25
01/07/2002|25|1|25|25|20|20|25|25|25
01/07/2003|25|1|25|25|21|21|25|25|25
01/07/2004|25|1|25|25|22|22|25|25|25
01/07/2005|25|1|25|25|23|23|25|25|25
01/07/2006|25|1|25|25|24|24|25|25|25
01/07/2007|25|1|25|25|25|25|25|25|25
01/07/2008|25|1|25|25|25|25|25|25|25";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var sgdfLimits = CreateDefaultObject<SGDFLimits>(id);
                sgdfLimits.company = companyId;
                sgdfLimits.InstanceID = instanceId;
                sgdfLimits.user = user;

                sgdfLimits.ValidityDate = DateTime.ParseExact(superSplitted[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                sgdfLimits.EG_Especie_GastosMedicos_1 = decimal.Parse(superSplitted[1]);
                sgdfLimits.EG_Especie_Fija_2 = decimal.Parse(superSplitted[2]);
                sgdfLimits.EG_Especie_mas_3SMDF_3 = decimal.Parse(superSplitted[3]);
                sgdfLimits.EG_Prestaciones_en_Dinero_4 = decimal.Parse(superSplitted[4]);
                sgdfLimits.Invalidez_y_vida_5 = decimal.Parse(superSplitted[5]);
                sgdfLimits.Cesantia_y_vejez_6 = decimal.Parse(superSplitted[6]);
                sgdfLimits.Guarderias_7 = decimal.Parse(superSplitted[7]);
                sgdfLimits.Retiro_8 = decimal.Parse(superSplitted[8]);
                sgdfLimits.RiesgodeTrabajo_9 = decimal.Parse(superSplitted[9]);

                result.Add(sgdfLimits);
            }

            return result;
        }

        //TablaIMSSPatron
        public List<Schema.IMSSEmployerTable> GetDefaultIMSSEmployerTable(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<IMSSEmployerTable>();

            string input =
                @"01/07/1997|1.05|13.9|6|0.7|1.75|3.15|1|2
01/07/1998|1.05|14.55|5.51|0.7|1.75|3.15|1|2
01/01/1999|1.05|15.2|5.02|0.7|1.75|3.15|1|2
01/01/2000|1.05|15.85|4.53|0.7|1.75|3.15|1|2
01/01/2001|1.05|16.5|4.04|0.7|1.75|3.15|1|2
01/01/2002|1.05|17.15|3.55|0.7|1.75|3.15|1|2
01/01/2003|1.05|17.15|3.55|0.7|1.75|3.15|1|2
01/01/2004|1.05|17.8|3.06|0.7|1.75|3.15|1|2
01/01/2005|1.05|18.45|2.57|0.7|1.75|3.15|1|2
01/01/2006|1.05|19.1|2.08|0.7|1.75|3.15|1|2
01/01/2007|1.05|19.75|1.59|0.7|1.75|3.15|1|2
01/01/2008|1.05|20.4|1.1|0.7|1.75|3.15|1|2";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var iMSSEmployer = CreateDefaultObject<IMSSEmployerTable>(id);
                iMSSEmployer.company = companyId;
                iMSSEmployer.InstanceID = instanceId;
                iMSSEmployer.user = user;

                iMSSEmployer.ValidityDate = DateTime.ParseExact(superSplitted[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                iMSSEmployer.EG_Especie_GastosMedicos_1 = decimal.Parse(superSplitted[1]);
                iMSSEmployer.EG_Especie_Fija_2 = decimal.Parse(superSplitted[2]);
                iMSSEmployer.EG_Especie_mas_3SMDF_3 = decimal.Parse(superSplitted[3]);
                iMSSEmployer.EG_Prestaciones_en_Dinero_4 = decimal.Parse(superSplitted[4]);
                iMSSEmployer.Invalidez_y_vida_5 = decimal.Parse(superSplitted[5]);
                iMSSEmployer.Cesantia_y_vejez_6 = decimal.Parse(superSplitted[6]);
                iMSSEmployer.Guarderias_7 = decimal.Parse(superSplitted[7]);
                iMSSEmployer.Retiro_8 = decimal.Parse(superSplitted[8]);

                result.Add(iMSSEmployer);
            }

            return result;
        }

        //SettlementCatalog
        public List<Schema.SettlementCatalog> GetDefaultSettlementCatalogTable(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<SettlementCatalog>();

            string input =
                @"01/01/2020|1|0|0|0|0|0|0";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var settlementCatalog = CreateDefaultObject<SettlementCatalog>(id);
                settlementCatalog.company = companyId;
                settlementCatalog.InstanceID = instanceId;
                settlementCatalog.user = user;

                settlementCatalog.ValidityDate = DateTime.ParseExact(superSplitted[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                settlementCatalog.Number = Int32.Parse(superSplitted[1]);
                settlementCatalog.CASUSMO = decimal.Parse(superSplitted[2]);
                settlementCatalog.CASISR86 = decimal.Parse(superSplitted[3]);
                settlementCatalog.CalDirecPerc = decimal.Parse(superSplitted[4]);
                settlementCatalog.Indem90 = decimal.Parse(superSplitted[5]);
                settlementCatalog.Indem20 = decimal.Parse(superSplitted[6]);
                settlementCatalog.PrimaAntig = decimal.Parse(superSplitted[7]);

                result.Add(settlementCatalog);
            }

            return result;
        }

        //TablaIMSSEmpleado
        public List<Schema.IMSSEmployeeTable> GetDefaultIMSSEmployeeTable(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<IMSSEmployeeTable>();

            string input =
                @"01/07/1997|0.375|0|2|0.25|0.625|1.125|0|0
01/07/1998|0.375|0|1.84|0.25|0.625|1.125|0|0
01/01/1999|0.375|0|1.68|0.25|0.625|1.125|0|0
01/01/2000|0.375|0|1.52|0.25|0.625|1.125|0|0
01/01/2001|0.375|0|1.36|0.25|0.625|1.125|0|0
01/01/2002|0.375|0|1.2|0.25|0.625|1.125|0|0
01/01/2003|0.375|0|1.2|0.25|0.625|1.125|0|0
01/01/2004|0.375|0|1.04|0.25|0.625|1.125|0|0
01/01/2005|0.375|0|0.88|0.25|0.625|1.125|0|0
01/01/2006|0.375|0|0.72|0.25|0.625|1.125|0|0
01/01/2007|0.375|0|0.56|0.25|0.625|1.125|0|0
01/01/2008|0.375|0|0.4|0.25|0.625|1.125|0|0";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var iMSSEmployee = CreateDefaultObject<IMSSEmployeeTable>(id);
                iMSSEmployee.company = companyId;
                iMSSEmployee.InstanceID = instanceId;
                iMSSEmployee.user = user;

                iMSSEmployee.ValidityDate = DateTime.ParseExact(superSplitted[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                iMSSEmployee.EG_Especie_GastosMedicos_1 = decimal.Parse(superSplitted[1]);
                iMSSEmployee.EG_Especie_Fija_2 = decimal.Parse(superSplitted[2]);
                iMSSEmployee.EG_Especie_mas_3SMDF_3 = decimal.Parse(superSplitted[3]);
                iMSSEmployee.EG_Prestaciones_en_Dinero_4 = decimal.Parse(superSplitted[4]);
                iMSSEmployee.Invalidez_y_vida_5 = decimal.Parse(superSplitted[5]);
                iMSSEmployee.Cesantia_y_vejez_6 = decimal.Parse(superSplitted[6]);
                iMSSEmployee.Guarderias_7 = decimal.Parse(superSplitted[7]);
                iMSSEmployee.Retiro_8 = decimal.Parse(superSplitted[8]);

                result.Add(iMSSEmployee);
            }

            return result;
        }

        //IMSSWorkRisk
        public List<Schema.IMSSWorkRisk> GetDefaultIMSSWorkRisk(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<IMSSWorkRisk>();

            string input =
                @"01/01/1998|0.07
01/01/1999|0.06";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var iMSSWorkRisk = CreateDefaultObject<IMSSWorkRisk>(id);
                iMSSWorkRisk.company = companyId;
                iMSSWorkRisk.InstanceID = instanceId;
                iMSSWorkRisk.user = user;

                iMSSWorkRisk.ValidityDate = DateTime.ParseExact(superSplitted[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                iMSSWorkRisk.Value = decimal.Parse(superSplitted[1]);

                result.Add(iMSSWorkRisk);
            }

            return result;
        }

        /// <summary>
        /// GetDefaultAnualIncomeTax
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="instanceId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Schema.AnualIncomeTax> GetDefaultAnualIncomeTax(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<AnualIncomeTax>();

            string input =
                    @"0.01|0|1.92
6942.21|133.28|6.4
58922.17|3460.01|10.88
103550.45|8315.57|16
120372.84|11007.14|17.92
144119.24|15262.49|21.36
290667.76|46565.26|23.52
458132.3|85952.92|30
874650.01|210908.23|32
1166200.01|304204.21|34
3498600.01|1097220.21|35";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var anualIncomeTax = CreateDefaultObject<AnualIncomeTax>(id);
                anualIncomeTax.company = companyId;
                anualIncomeTax.InstanceID = instanceId;
                anualIncomeTax.user = user;

                anualIncomeTax.ValidityDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                anualIncomeTax.LowerLimit = Decimal.Parse(superSplitted[0]);
                anualIncomeTax.FixedFee = Decimal.Parse(superSplitted[1]);
                anualIncomeTax.Rate = Decimal.Parse(superSplitted[2]);

                result.Add(anualIncomeTax);
            }

            return result;
        }

        public List<Schema.IMSSFare> GetDefaultIMSSFare(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<IMSSFare>();

            string input =
            @"EM - Prestación en Especie Cuota fija|0.204|0|1
EM - Prestación en Especie Cuota excedente|0.011|0.004|25
EM - Prestación En Dinero|0.007|0.0025|25
EM - Prestación Pensionados|0.0105|0.00375|25
INVALIDEZ Y VIDA|0.0175|0.00625|25
GUARDERIAS Y PREST. SOCIALES|0.01|0|25
RETIRO|0.02|0|25
CESANTÍA Y VEJEZ|0.0315|0.01125|25
INFONAVIT|0.05|0|0";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var imssFare = CreateDefaultObject<IMSSFare>(id);
                imssFare.company = companyId;
                imssFare.InstanceID = instanceId;
                imssFare.user = user;

                imssFare.IMMSBranch = superSplitted[0].ToString();
                imssFare.EmployerShare = decimal.Parse(superSplitted[1]);
                imssFare.EmployeeShare = decimal.Parse(superSplitted[2]);
                imssFare.MaxSMDF = int.Parse(superSplitted[3]);

                result.Add(imssFare);
            }

            return result;
        }

        public List<MonthlyIncomeTax> GetDefaultMonthlyIncomeTax(Guid companyId, Guid instanceId, Guid user)
        {
            var result = new List<MonthlyIncomeTax>();

            string input =
                    @"0.01|0|1.92
578.53|11.11|6.4
4910.19|288.33|10.88
8629.21|692.96|16
10031.08|917.26|17.92
12009.95|1271.87|21.36
24222.32|3880.44|23.52
38177.7|7162.74|30
72887.51|17575.69|32
97183.34|25350.35|34
291550.01|91435.02|35";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var monthlyIncomeTax = CreateDefaultObject<MonthlyIncomeTax>(id);
                monthlyIncomeTax.company = companyId;
                monthlyIncomeTax.InstanceID = instanceId;
                monthlyIncomeTax.user = user;

                monthlyIncomeTax.ValidityDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                monthlyIncomeTax.LowerLimit = Decimal.Parse(superSplitted[0]);
                monthlyIncomeTax.FixedFee = Decimal.Parse(superSplitted[1]);
                monthlyIncomeTax.Rate = Decimal.Parse(superSplitted[2]);

                result.Add(monthlyIncomeTax);
            }

            return result;
        }

        public List<UMA> GetdDefaultUMA(Guid companyId, Guid instanceID, Guid user)
        {
            var result = new List<UMA>();

            string input =
                    @"01/01/2016|73.04
01/02/2017|75.49
01/02/2018|80.6
01/02/2019|84.49
01/02/2020|86.88";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var uma = CreateDefaultObject<UMA>(id);
                uma.company = companyId;
                uma.InstanceID = instanceID;
                uma.user = user;
                var date = DateTime.ParseExact(superSplitted[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                uma.ValidityDate = date;
                uma.Value = Decimal.Parse(superSplitted[1]);

                result.Add(uma);
            }

            return result;
        }

        public List<MonthlyEmploymentSubsidy> GetDefaultMonthlyEmploymentSubsidy(Guid companyId, Guid instanceID, Guid user)
        {
            var result = new List<MonthlyEmploymentSubsidy>();

            string input =
                    @"0.01|407.02
1768.97|406.83
2653.39|406.62
3472.85|392.77
3537.88|382.46
4446.16|354.23
4717.19|324.87
5335.43|294.63
6224.68|253.54
7113.91|217.61
7382.34|0";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var monthlyIncomeTax = CreateDefaultObject<MonthlyEmploymentSubsidy>(id);
                monthlyIncomeTax.company = companyId;
                monthlyIncomeTax.InstanceID = instanceID;
                monthlyIncomeTax.user = user;

                monthlyIncomeTax.ValidityDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                monthlyIncomeTax.LowerLimit = Decimal.Parse(superSplitted[0]);
                monthlyIncomeTax.MonthlySubsidy = Decimal.Parse(superSplitted[1]);

                result.Add(monthlyIncomeTax);
            }

            return result;
        }

        public List<AnualEmploymentSubsidy> GetDefaultAnualEmploymentSubsidy(Guid companyId, Guid instanceID, Guid user)
        {
            var result = new List<AnualEmploymentSubsidy>();

            string input =
                    @"0.01|4884.24
21227.64|4881.96
31840.68|4879.44
41674.2|4713.24
42454.56|4589.52
53353.92|4250.76
56606.28|3898.44
64025.16|3535.56
74696.16|3042.48
85366.92|2611.32
88588.08|0";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var anualIncomeTax = CreateDefaultObject<AnualEmploymentSubsidy>(id);
                anualIncomeTax.company = companyId;
                anualIncomeTax.InstanceID = instanceID;
                anualIncomeTax.user = user;

                anualIncomeTax.ValidityDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                anualIncomeTax.LowerLimit = Decimal.Parse(superSplitted[0]);
                anualIncomeTax.AnualSubsidy = Decimal.Parse(superSplitted[1]);

                result.Add(anualIncomeTax);
            }

            return result;
        }

        public List<Schema.Workshift> GetDefaultWorkShift(Guid companyId, Guid instanceID, Guid user)
        {
            var result = new List<Workshift>();

            string input =
                @"Turno diurno|8|01
Turno nocturno|7|02";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var workshift = CreateDefaultObject<Workshift>(id);
                workshift.company = companyId;
                workshift.InstanceID = instanceID;
                workshift.user = user;

                //Name
                workshift.Name = superSplitted[0];

                //Hours
                workshift.Hours = double.Parse(superSplitted[1]);

                //work shift type
                if (superSplitted[2] == "01")
                {
                    workshift.ShiftWorkingDayType = ShiftWorkingDayType.Daytime;
                }
                else if (superSplitted[2] == "02")
                {
                    workshift.ShiftWorkingDayType = ShiftWorkingDayType.Night;
                }

                result.Add(workshift);
            }

            return result;
        }

        public string GetFormulaValue(ConceptPayment conceptPayment)
        {
            var formulaValue = "";
            if (conceptPayment.Code == 1 && conceptPayment.ConceptType == ConceptType.SalaryPayment)
            {
                formulaValue = "VDiasDerechoSueldoVigente + VDiasDerechoSueldoAnterior - ((VHorasRetAnt + VHorasRetVig) / HorasPorTurno(0))";
            }
            else if (conceptPayment.Code == 3 && conceptPayment.ConceptType == ConceptType.SalaryPayment)
            {
                formulaValue = "ROUNDTO(VParteProporcional7oDiaVig + VParteProporcional7oDiaAnt, 2)";
            }
            else if (conceptPayment.Code == 4 && conceptPayment.ConceptType == ConceptType.SalaryPayment)
            {
                formulaValue = "Incidencia[Horas extras]";
            }
            else if (conceptPayment.Code == 19 && conceptPayment.ConceptType == ConceptType.SalaryPayment)
            {
                formulaValue = "VacacionesATiempoValor";
            }
            else if (conceptPayment.Code == 20 && conceptPayment.ConceptType == ConceptType.SalaryPayment)
            {
                formulaValue = "PrimaVacacionesATiempoValor";
            }
            //Deducción Infonavit Procentaje
            else if (conceptPayment.Code == 59 && conceptPayment.ConceptType == ConceptType.DeductionPayment)
            {
                formulaValue = "MontoDescINFONAVIT";
            }
            //Deducción Infonavit Factor de Descuento
            else if (conceptPayment.Code == 15 && conceptPayment.ConceptType == ConceptType.DeductionPayment)
            {
                formulaValue = "MontoDescINFONAVIT";
            }
            //Deducción Infonavit Cuota fija
            else if (conceptPayment.Code == 16 && conceptPayment.ConceptType == ConceptType.DeductionPayment)
            {
                formulaValue = "MontoDescINFONAVIT";
            }

            return formulaValue;
        }

        public (List<Schema.IConcept>, List<ConceptPaymentRelationship>) GetDefaultConcept<T>(Guid companyId, Guid instanceId, Guid user, List<AccumulatedType> accumulatedTypes) where T : class
        {
            //Perceptions
            var result = new List<Schema.IConcept>();

            string input =
                @"0|N|Neto|0|0|0|1|0|||||| ||G|G||||||||||||
99|D|Ajuste al neto|0|0|0|1|0|||||| ||G|G||||||||||D|004|
1|P|Sueldo|0|1|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|HORAS|IIF(VPeriodoDeVacaciones , 0 ,(VDiasDerechoSueldoAnterior * VSalDiarioAnt) +(VDiasDerechoSueldoVigente * VSalDiarioVigente) - VImpRetardos) + VPago_SueldoFin +(DiasDescansoVacPeriodoCompleto(_0) * VSalDiarioVigente)|Percepción[Sueldo][Total]|0|Percepción[Sueldo][Total]|0||||P|001|
3|P|Séptimo día|0|1|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||VSeptimos_Dias|Percepción[Séptimo día][Total]|0|Percepción[Séptimo día][Total]|0||||P|001|
4|P|Horas extras|0|1|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||VSalarioHEAnt + VSalarioHEVig|MAX(Percepción[Horas extras][Total] - HE_topesXsemana(_0), 0)|Percepción[Horas extras][Total] - Percepción[Horas extras][Gravado ISR]|MAX(Percepción[Horas extras][Total] - VSalarioExentoIMSSXhora,0)|Percepción[Horas extras][Total] - Percepción[Horas extras][Gravado IMSS]||||P|019|01,02,03
5|P|Destajos|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||ImporteDestajo(_0)|Percepción[Destajos][Total]|0|Percepción[Destajos][Total]|0||||P|001|
6|P|Comisiones|0|0|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Comisiones][Total]|0|Percepción[Comisiones][Total]|0||||P|028|
7|P|Incentivo productividad|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Incentivo productividad][Total]|0|Percepción[Incentivo productividad][Total]|0||||P|038|
8|P|Incentivos (demoras)|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Incentivos demoras][Total]|0|Percepción[Incentivos demoras][Total]|0||||P|038|
9|P|Incapacidad pagada empresa|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||IncidenciaVigente[Incapacidad pagada por la empresa] * Empleado[Salario diario] + IncidenciaAnterior[Incapacidad pagada por la empresa] * SalCuotaDiariaAnt(_0)|0|Percepción[Incapacidad pagada empresa][Total]|Percepción[Incapacidad pagada empresa][Total]|0||||P|014|
10|P|Prima dominical|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|Dias de Prima|Percepción[Prima dominical][Dias de Prima] * Empleado[Salario diario] * 0.25|MAX(Percepción[Prima dominical][Total] - Percepción[Prima dominical][Dias de Prima] * UMA , 0)|Percepción[Prima dominical][Total] - Percepción[Prima dominical][Gravado ISR]|Percepción[Prima dominical][Total]|0||||P|020|
11|P|Día festivo / descanso|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||(IncidenciaVigente[Día festivo trabajado] * Empleado[Salario diario] * 2) + (IncidenciaAnterior[Día festivo trabajado] * 2 * SalCuotaDiariaAnt(_0)) |IIF(Empleado[Salario diario] <= SalarioMinimo, Percepción[Día festivo descanso][Total] - MIN(Percepción[Día festivo descanso][Total], DFT_topesXsemana()), Percepción[Día festivo descanso][Total] - MIN(Percepción[Día festivo descanso][Total] /2, DFT_topesXsemana()))|Percepción[Día festivo descanso][Total] - Percepción[Día festivo descanso][Gravado ISR]|Percepción[Día festivo descanso][Total]|0||||P|038|
12|P|Gratificación|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Gratificación][Total]|0|Percepción[Gratificación][Total]|0||||P|038|
13|P|Compensación|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Compensación][Total]|0|Percepción[Compensación][Total]|0||||P|038|
14|P|Premios eficiencia|0|1|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||Percepción[Premios eficiencia][Valor]|Percepción[Premios eficiencia][Total]|0|Percepción[Premios eficiencia][Total]|0||||P|038|
15|P|Bono puntualidad|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Bono puntualidad][Total]|0|Percepción[Bono puntualidad][Total] - MIN(VSBCVigente * 0.1 * VdiasdePeriodo , Percepción[Bono puntualidad][Total])|Percepción[Bono puntualidad][Total] - Percepción[Bono puntualidad][Gravado IMSS]||||P|010|
16|P|Retroactivo|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Retroactivo][Total]|0|Percepción[Retroactivo][Total]|0||||P|001|
17|P|Ajuste en sueldos|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Ajuste en sueldos][Total]|0|Percepción[Ajuste en sueldos][Total]|0||||P|001|
18|P|Anticipo de sueldos|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Anticipo de sueldos][Total]|0||||||P|038|
19|P|Vacaciones a tiempo|0|0|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|Dias|VSalarioPrestaciones * Percepción[Vacaciones a tiempo][Dias]|Percepción[Vacaciones a tiempo][Total]|0|0|0||||P|001|
20|P|Prima de vacaciones a tiempo|0|0|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|Dias de Prima|VSalarioPrestaciones * Percepción[Prima de vacaciones a tiempo][Dias de Prima]|Percepción[Prima de vacaciones a tiempo][Total] - MIN(Percepción[Prima de vacaciones a tiempo][Total] , MAX(15 * UMA - AcumuladoAnual[ISR Prima vac. exenta],0))|Percepción[Prima de vacaciones a tiempo][Total] - Percepción[Prima de vacaciones a tiempo][Gravado ISR]|Percepción[Prima de vacaciones a tiempo][Total]|0||||P|021|
21|P|Vacaciones reportadas $|0|0|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|Dias|VSalarioPrestaciones * Percepción[Vacaciones reportadas $][Dias]|Percepción[Vacaciones reportadas $][Total]|0|Percepción[Vacaciones reportadas $][Total]|||||P|001|
22|P|Prima de vacaciones reportada $|0|0|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|Dias|VSalarioPrestaciones * Percepción[Prima de vacaciones reportada $][Dias]|Percepción[Prima de vacaciones reportada $][Total] - MIN(Percepción[Prima de vacaciones reportada $][Total] , MAX(15 * UMA - AcumuladoAnual[ISR Prima vac. exenta] - Acumulado[ISR Prima vac. exenta],0))|Percepción[Prima de vacaciones reportada $][Total] - Percepción[Prima de vacaciones reportada $][Gravado ISR]|Percepción[Prima de vacaciones reportada $][Total]|0||||P|021|
23|P|Días de vacaciones|1|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|Dias|Percepción[Días de vacaciones][Dias] * VSalarioPrestaciones|Percepción[Días de vacaciones][Total]|0||||||P|001|
24|P|Aguinaldo|0|0|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||Percepción[Aguinaldo][Valor]  * VSalarioPrestaciones|Percepción[Aguinaldo][Total] - MIN(MAX(UMA * 30 - AcumuladoAnual[ISR Gratificación exenta],0), Percepción[Aguinaldo][Total])|MIN(Percepción[Aguinaldo][Total] , MAX(30 * UMA - AcumuladoAnual[ISR Gratificación exenta],0))|Percepción[Aguinaldo][Total]|0||||P|002|
25|P|Reparto de utilidades|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G|||Percepción[Reparto de utilidades][Total] - MIN(15 * UMA , Percepción[Reparto de utilidades][Total])|MIN(15 * UMA , Percepción[Reparto de utilidades][Total])|0|Percepción[Reparto de utilidades][Total]||||P|003|
26|P|Indemnización|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||VIndenmizacion_90|Percepción[Indemnización][Total] - MIN(Percepción[Indemnización][Total] , 90 * UMA * VFinAnosISR)|MIN(Percepción[Indemnización][Total] , 90 * UMA * VFinAnosISR)||||||P|025|
27|P|Separación Unica|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||VIndenmizacionTotal_20|Percepción[Separación Unica][Total] - MIN(Percepción[Separación Unica][Total] , 90 * UMA * VFinAnosISR - Acumulado[ISR Liquidacion exento])|MIN(Percepción[Separación Unica][Total] , 90 * UMA * VFinAnosISR - Acumulado[ISR Liquidacion exento])||||||P|023|
29|P|Prima de antiguedad|0|0|1|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||VPago_Prima_Anti|Percepción[Prima de antiguedad][Total] - MIN(Percepción[Prima de antiguedad][Total] , 90 * UMA * VFinAnosISR - Acumulado[ISR Liquidacion exento])|MIN(Percepción[Prima de antiguedad][Total] , 90 * UMA * VFinAnosISR - Acumulado[ISR Liquidacion exento])||||||P|022|
31|P|Fondo ahorro empresa|1|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||MIN(0.13 * UMA * 10 * Periodo[Días pago] ,(Percepción[Fondo ahorro empresa][Valor] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total] + Percepción[Vacaciones a tiempo][Total] + Percepción[Vacaciones reportadas $][Total] + Percepción[Retroactivo][Total] + Percepción[Ajuste en sueldos][Total]))||Percepción[Fondo ahorro empresa][Total]||||||P|005|
32|P|Despensa|1|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||(Percepción[Sueldo][Total] + Percepción[Retroactivo][Total] + Percepción[Ajuste en sueldos][Total] + Percepción[Vacaciones a tiempo][Total] + Percepción[Vacaciones reportadas $][Total]) * Percepción[Despensa][Valor] / 100|Percepción[Despensa][Total]|0|Percepción[Despensa][Total] - MIN(0.40 * UMA *(VDiasLFTSalarioAnterior + VDiasLFTSalarioVigente), Percepción[Despensa][Total])|Percepción[Despensa][Total] - Percepción[Despensa][Gravado IMSS]||||P|029|
33|P|Deporte y cultura|1|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||(Percepción[Deporte y cultura][Valor] / 100) *(Percepción[Sueldo][Total] + Percepción[Retroactivo][Total] + Percepción[Ajuste en sueldos][Total] + Percepción[Vacaciones a tiempo][Total] + Percepción[Vacaciones reportadas $][Total])|Percepción[Deporte y cultura][Total]|0||||||P|038|
35|P|Anticipo vacaciones Percepción|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS|| ||G|G|||Percepción[Anticipo vacaciones Percepción][Total]|0||||||P|001|
36|P|Destajo - sueldo|0|0|0|1|0|Gravado ISR|Exento ISR|Gravado IMSS|Exento IMSS||F||G|G||Percepción[Sueldo][Total] + ImporteDestajo(_0)|Percepción[Destajo sueldo][Total]|0|Percepción[Destajo sueldo][Total]|0||||P|001|
37|P|Comisión sueldo|0|0|0|1|0|Gravado ISR|Exento ISR|IMSS Gravado|Exento IMSS||F||G|G|Unidades||Percepción[Comisión sueldo][Total]|0|Percepción[Comisión sueldo][Total]|||||P|028|
52|D|I.M.S.S.|0|1|1|1|0||||||F||G|G||IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0) O (vTotalPercepciones <= 0), 0 , CuotaObreroEG1 + CuotaObreroEG3SMGDF + CuotaObreroEG4 + CuotaObreroInvalidezYVida5 + CuotaObreroCesantia6)|0|0|0|0||||D|001|
53|D|I.E.|0|0|0|1|0||||||F||G|G||0|0|0|0|0||||D|004|
54|D|Cuota sindical|0|0|0|1|0|||||| ||G|G|Porcentaje|(Deducción[Cuota sindical][Porcentaje] / 100) * Percepción[Sueldo][Total]||||||||D|019|
56|D|Caja de ahorro|0|0|0|1|0||||||F||G|G|Porcentaje|(Deducción[Caja de ahorro][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total] + Percepción[Retroactivo][Total] + Percepción[Ajuste en sueldos][Total] + Percepción[Vacaciones a tiempo][Total] + Percepción[Vacaciones reportadas $][Total] + Percepción[Comisiones][Total])||||||||D|004|
57|D|Préstamo caja de ahorro|0|0|0|1|0|||||| ||G|G|Porcentaje|(Deducción[Préstamo caja de ahorro][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
58|D|Intereses Ptmo ahorro|0|0|0|1|0||||||F||G|G|Porcentaje|(Deducción[Intereses Ptmo ahorro][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
59|D|Préstamo Infonavit|0|0|0|1|0||||||F||G|G|Porcentaje|(Deducción[Préstamo Infonavit][Porcentaje] / 100) * (DiasInfonavitPorcentajeAnterior(_0) * SBCAnterior(_0) + DiasInfonavitPorcentajeVigente(_0) * SBCVigente(_0))||||||||D|009|
60|D|Intereses Ptmo Infonavit|0|0|0|1|0||||||F||G|G||||||||||D|009|
61|D|Préstamo FONACOT|0|0|0|1|0|||||| ||G|G|Retención creditos|vRetencionFONACOTPeriodo||||||||D|011|
62|D|Fonacot revolvente|0|0|0|1|0|||||| ||G|G|Porcentaje|(Deducción[Fonacot revolvente][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|011|
63|D|Intereses Ptmo fonacot|0|0|0|1|0||||||F||G|G||||||||||D|011|
64|D|Préstamo empresa|0|0|0|1|0|||||| ||G|G|Porcentaje|(Deducción[Préstamo empresa][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
65|D|Intereses Ptmo empresa|0|0|0|1|0||||||F||G|G||||||||||D|004|
66|D|Anticipo sueldo|0|0|0|1|0|||||| ||G|G||||||||||D|012|
67|D|Fondo de ahorro|0|0|0|1|0||||||F||G|G|Porcentaje|(Deducción[Fondo de ahorro][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total] + Percepción[Retroactivo][Total] + Percepción[Ajuste en sueldos][Total] + Percepción[Vacaciones a tiempo][Total] + Percepción[Vacaciones reportadas $][Total] + Percepción[Comisión sueldo][Total])||||||||D|004|
69|D|Reintegración|0|0|0|1|0|||||| ||G|G||||||||||D|004|
70|D|Deduccion general|0|0|0|1|0|||||| ||G|G|Dias|Deducción[Deduccion general][Dias] * VSalDiarioVigente||||||||D|004|
71|D|Ajuste en Subsidio para el empleo|0|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||VAjuste26_SubsEntregado_deMas|0|0|0|0||||D|071|
72|D|Préstamo fondo de ahorro|0|0|0|1|0||||||F||G|G||(Deducción[Préstamo fondo de ahorro][Valor] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
73|D|Intereses Ptmo fondo de ahorro|0|0|0|1|0||||||F||G|G||||||||||D|004|
74|D|Anticipo vacaciones|0|0|0|1|0|||||| ||G|G||||||||||D|012|
75|D|Subs entregado que no correspondía|0|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||0 - VAjuste27_SubsEntregado_no_correspondia|0|0|0|0||||OP|008|
87|D|Aportación voluntaria Infonavit|0|0|0|1|0|||||| ||G|G||||||||||D|005|
88|D|Aportación voluntaria SAR|0|0|0|1|0|||||| ||G|G||||||||||D|003|
89|O|2% Fondo retiro SAR (8)|0|1|1|1|0||||||F||G|G||IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0), 0 , CuotaRetiro8)|0|0|0|0||||||
90|O|2% Impuesto estatal|0|1|1|1|0||||||F||G|G||Acumulado[ISR Total de percepciones] * 0.02||||||||||
91|D|Subsidio acreditable|1|0|0|1|0||||||F||G|G||0|0|0|0|0||||D||
92|D|Subsidio no acreditable|1|0|0|1|0||||||F||G|G||0|0|0|0|0||||D||
93|O|Riesgo de trabajo (9)|0|1|1|1|0||||||F||G|G||IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0), 0 , CuotaRT9)|0|0|0|0||||||
94|D|Sobregiro|1|0|0|1|0|||||| ||G|G||||||||||D||
95|O|1% Educación empresa|0|0|0|1|0||||||F||G|G||||||||||||
96|O|I.M.S.S. empresa|0|1|1|1|0||||||F||G|G||IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaPatronEG1 + CuotaPatronEG2 + CuotaPatronEG3SMGDF + CuotaPatronEG4 + CuotaPatronInvalidezyVida5 + CuotaPatronCesantia6)|0|0|0|0||||||
97|O|Infonavit empresa|0|1|1|1|0||||||F||G|G||IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0), 0 , CuotaINFONAVIT)|0|0|0|0||||||
98|O|Guarderia I.M.S.S. (7)|0|1|1|1|0||||||F||G|G||IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0), 0 , CuotaGuarderias7)|0|0|0|0||||||
5|D|Ret. Inv. Y Vida|1|1|1|0|0||||||F||G|G||IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaObreroInvalidezYVida5)|0|0|0|0||||D||
6|D|Ret. Cesantia|1|1|1|0|0||||||F||G|G||IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaObreroCesantia6)|0|0|0|0||||D||
11|D|Ret. Enf. y Mat. obrero|1|1|1|0|0|RET. GASTOS MED. PENS|RET. EXE 3SMGDF|RET. PREST. EN DINERO|||F||G|G||IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0) O (vTotalPercepciones <= 0), 0 , CuotaObreroEG1 + CuotaObreroEG3SMGDF + CuotaObreroEG4)|IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0) O (vTotalPercepciones <= 0), 0 , CuotaObreroEG1)|IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0) O (vTotalPercepciones <= 0), 0 , CuotaObreroEG3SMGDF)|IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0) O (vTotalPercepciones <= 0), 0 , CuotaObreroEG4)|0||||D||
5|O|Invalidez y Vida|1|1|1|0|0||||||F||G|G||IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaPatronInvalidezyVida5)|0|0|0|0||||||
6|O|Cesantia y Vejez|1|1|1|0|0||||||F||G|G||IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaPatronCesantia6)|0|0|0|0||||||
45|D|I.S.R. (mes)|0|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|ISR anual días|VAjuste11_ISR_RETENER|0|0|0|0||||D|002|
44|D|I.S.R. (anual)|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|ISR anual directo|0|0|0|0|0||||D|002|
35|D|Subsidio al Empleo (mes)|0|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Subs anual días| -1 * (VAjuste20_SUBSIDIO_ENTREGAR_DEF) + VAjuste28_SubsEntregado_deMenos|0|0|0|0||||OP|002|
34|D|Subsidio al Empleo (anual)|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Subs al salario|0|0|0|0|0||||OP|002|
7|O|Enf. y Mat. Patron|1|1|1|0|0|GASTOS MEDICOS PENS.|CUOTA FIJA|EXEDENTE 3SMGDF|PREST. EN ESPECIE||F||G|G||IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaPatronEG1 + CuotaPatronEG2 + CuotaPatronEG3SMGDF + CuotaPatronEG4)|IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaPatronEG1)|IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaPatronEG2)|IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaPatronEG3SMGDF)|IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones(_0) = 0, 0 , CuotaPatronEG4)||||||
43|D|I.S.R. Art174|0|0|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||Vart142_Fracc_IV|0|0|0|0||||D|002|
131|P|Fondo de ahorro Empresa|0|0|0|1|0|ISR Gravado|Exento ISR|Integra IMSS|Exento IMSS||F||G|G||MIN(0.13 * UMA * 10 * Periodo[Días pago] ,(Percepción[Fondo ahorro empresa][Valor] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total] + Percepción[Vacaciones a tiempo][Total] + Percepción[Vacaciones reportadas $][Total] + Percepción[Retroactivo][Total] + Percepción[Ajuste en sueldos][Total] + Percepción[Destajo sueldo][Total] + Percepción[Comisión sueldo][Total]))|0|Percepción[Fondo ahorro empresa][Total]|0|0||||P|005|
157|D|Ptmo caja de ahorro2|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Porcentaje|(Deducción[Ptmo caja de ahorro2][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
158|D|Ptmo caja de ahorro3|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Porcentaje|(Deducción[Ptmo caja de ahorro3][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
159|D|Ptmo caja de ahorro4|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Porcentaje|(Deducción[Ptmo caja de ahorro4][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
164|D|Ptmo empresa2|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Porcentaje|(Deducción[Ptmo empresa2][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
165|D|Ptmo empresa3|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Porcentaje|(Deducción[Ptmo empresa3][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
166|D|Ptmo empresa4|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Porcentaje|(Deducción[Ptmo empresa4][Porcentaje] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
172|D|Ptmo fondo de ahorro2|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||(Deducción[Ptmo fondo de ahorro2][Valor] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
173|D|Ptmo fondo de ahorro3|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||(Deducción[Ptmo fondo de ahorro3][Valor] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
174|D|Ptmo fondo de ahorro4|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||(Deducción[Ptmo fondo de ahorro4][Valor] / 100) *(Percepción[Sueldo][Total] + Percepción[Séptimo día][Total])||||||||D|004|
32|D|Subs al Empleo acreditado|1|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Subs de Tarifa| -1 * (VAjuste10_SubsCausaCorrespDef) + VAjuste25_SubsCausado_deMenos|0|0|0|0||||D||
41|D|I.S.R. antes de Subs al Empleo|1|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|ISR|IIF(1=1, vISPT_CORRESPONDE_P , vISPT_CORRESPONDEmz_P)|0|0|0|0||||D||
33|D|ISR retenido de ejercicio anterior|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||||||||||D|101|
101|D|I.S.R. finiquito|0|0|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||VFin_ISPT_Definitivo_de_Finiquitos|0|0|0|0||||D|002|
175|D|Concepto vacio 1|0|0|0|0|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||||||||||D|004|
176|D|Concepto vacio 2|0|0|0|0|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||||||||||D|004|
55|D|I.S.R. a compensar|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||||||||||OP|004|
15|D|Préstamo Infonavit (vsm)|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Valor|Deducción[Préstamo Infonavit vsm][Valor] * FactorDescINFONAVIT * 2*DiasInfonavitAmortizacion(_0) / DiasBimestreCalendario(_0)||||||||D|009|
14|D|Seguro de vivienda Infonavit|0|0|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||IIF(AplicaPagoSeguro(0) = 1 , TINFONAVITSegVivienda.Cuota{Periodo[Fecha fin]}, 0)|0|0|0|0||||D|009|
16|D|Préstamo Infonavit (cf)|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|Valor|Deducción[Préstamo Infonavit cf][Valor] * 2 / DiasBimestreCalendario(_0) * DiasInfonavitPesosAmortizacion(_0)||||||||D|009|
50|D|Reintegro de ISR retenido en exceso|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||0|0|0|0|0||||OP|001|
102|D|ISR Retenido de Ejercicio Vigente|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||0|0|0|0|0||||D|002|
103|D|Reintegro ISR en exceso de ejercicio ant|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||0|0|0|0|0||||OP|005|
104|D|ISR de ajuste mensual|0|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||VAjuste29_ISR_ajuste_mensual|0|0|0|0||||D|002|
105|D|ISR ajustado por subsidio|0|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|| - VAjuste30_ISR_ajustado_por_Subsidio|0|0|0|0||||OP|007|
106|D|Reintegro ISR retenido de mas|0|0|0|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G|| - VAjuste32_ISR_Retenido_deMas|0|0|0|0||||OP|001|
107|D|Ajuste al Subsidio Causado|0|1|1|1|0|Descripción Uno|Descripción Dos|Descripción Tres|Descripción Cuatro||F||G|G||VAjuste24_SubsCausado_deMas|0|0|0|0||||D|107|";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var type = superSplitted[1].ToString();

                var satGRoupCode = String.Empty;
                if (!String.IsNullOrEmpty(superSplitted[27].ToString()))
                {
                    satGRoupCode = $"{superSplitted[26]}-{superSplitted[27]}";
                }

                var id = Guid.NewGuid();
                var conceptPayment = CreateDefaultObject<ConceptPayment>(id);
                conceptPayment.company = companyId;
                conceptPayment.InstanceID = instanceId;
                conceptPayment.user = user;
                conceptPayment.Kind = Convert.ToBoolean(Convert.ToInt16(superSplitted[3]));
                conceptPayment.GlobalAutomatic = Convert.ToBoolean(Convert.ToInt16(superSplitted[4]));
                conceptPayment.AutomaticDismissal = Convert.ToBoolean(Convert.ToInt16(superSplitted[5]));
                conceptPayment.Print = Convert.ToBoolean(Convert.ToInt16(superSplitted[6]));

                conceptPayment.SATGroupCode = satGRoupCode;
                conceptPayment.Code = Int32.Parse(superSplitted[0]);
                conceptPayment.Description = superSplitted[2];
                conceptPayment.Name = superSplitted[2];

                conceptPayment.Label1 = superSplitted[8];
                conceptPayment.Label2 = superSplitted[9];
                conceptPayment.Label3 = superSplitted[10];
                conceptPayment.Label4 = superSplitted[11];
                conceptPayment.Formula = superSplitted[18];
                conceptPayment.Formula1 = superSplitted[19];
                conceptPayment.Formula2 = superSplitted[20];
                conceptPayment.Formula3 = superSplitted[21];
                conceptPayment.Formula4 = superSplitted[22];

                if (type == "P" || type == "N")
                {
                    conceptPayment.ConceptType = ConceptType.SalaryPayment;
                }
                else if (type == "O")
                {
                    conceptPayment.ConceptType = ConceptType.LiabilityPayment;
                }
                else if (type == "D")
                {
                    conceptPayment.ConceptType = ConceptType.DeductionPayment;
                }

                //Fórmula del Valor
                conceptPayment.FormulaValue = GetFormulaValue(conceptPayment);

                result.Add(conceptPayment);
            }

            var conceptPayments = result.Where(p => p.GetType().Equals(typeof(T))).ToList();

            var relationships = GetDefaultConceptPaymentRelationship(conceptPayments, accumulatedTypes);

            return (conceptPayments, relationships);
        }

        /// <summary>
        /// Creates objects relationship
        /// </summary>
        /// <param name="accumulatedDescription"></param>
        /// <param name="conceptPaymentCode"></param>
        /// <param name="conceptPayments"></param>
        /// <param name="accumulatedTypes"></param>
        /// <returns></returns>
        private ConceptPaymentRelationship createObjectRelathionshipFiscalAccumulates(string accumulatedDescription,
            int conceptPaymentCode, ConceptPaymentType conceptPaymentRelationshipType, ConceptType conceptType,
            List<IConcept> conceptPayments, List<AccumulatedType> accumulatedTypes)
        {
            var conceptPayment = conceptPayments.Cast<ConceptPayment>().FirstOrDefault();

            var conceptPaymentRelationship = CreateDefaultObject<ConceptPaymentRelationship>(Guid.NewGuid());
            conceptPaymentRelationship.InstanceID = conceptPayment.InstanceID;
            conceptPaymentRelationship.company = conceptPayment.company;
            conceptPaymentRelationship.AccumulatedTypeID = accumulatedTypes.FirstOrDefault(p => p.Name == accumulatedDescription).ID;
            conceptPaymentRelationship.ConceptPaymentID = conceptPayments.FirstOrDefault(p => p.Code == conceptPaymentCode && p.ConceptType == conceptType).ID;
            conceptPaymentRelationship.ConceptPaymentType = conceptPaymentRelationshipType;
            conceptPaymentRelationship.ConceptPaymentRelationshipType = ConceptPaymentRelationshipType.FiscalAccumulates;
            return conceptPaymentRelationship;
        }

        /// <summary>
        /// Get default concept payment relationship
        /// </summary>
        /// <param name="conceptPayments"></param>
        /// <param name="accumulatedTypes"></param>
        /// <returns></returns>
        public List<ConceptPaymentRelationship> GetDefaultConceptPaymentRelationship(List<IConcept> conceptPayments, List<AccumulatedType> accumulatedTypes)
        {
            var result = new List<ConceptPaymentRelationship>();
            //GetDefaultAccumulatedTypes for the default concepts

            //Percepciones
            //Sueldo 1 
            var conceptType = ConceptType.SalaryPayment;
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 1, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 1, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 1, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 1, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Séptimo día 3
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 3, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 3, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 3, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 3, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Horas extras 4
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 4, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 4, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 4, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 4, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Destajos extras 5
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 5, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 5, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 5, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 5, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 5, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Comisiones 6
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 6, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 6, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 6, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 6, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 6, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Incentivo productividad 7
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 7, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 7, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 7, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 7, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Incentivo (demoras) 8 
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 8, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 8, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 8, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 8, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Incapacidad pagada empresa 9
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 9, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 9, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 9, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Prima dominical empresa 10
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 10, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 10, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 10, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 10, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Día festivo / descanso 11
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 11, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 11, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 11, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 11, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Gratificación / descanso 12
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 12, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 12, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 12, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 12, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Compensación 13
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 13, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 13, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 13, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 13, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Premios eficiencia 14
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 14, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 14, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 14, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 14, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Bono de puntualidad 15
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 15, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 15, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 15, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 15, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Retroactivo 16
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 16, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 16, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 16, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 16, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 16, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Ajuste de sueldos 17
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 17, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 17, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 17, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 17, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 17, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Anticipo de sueldos 18
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 18, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 18, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Vacaciones a tiempo 19
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 19, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 19, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 19, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));

            //Prima de vacaciones a tiempo 20
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 20, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada  Art142", 20, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Prima vac. exenta", 20, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Vaciones reportadas $ 21
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 21, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 21, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 21, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales exentas", 21, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Prima de vacaciones reportad $ 22
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 22, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada  Art142", 22, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Prima vac. exenta", 22, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Dias de vacaciones 23
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 23, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 23, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Perc.especiales grav.", 23, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 23, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Aguinaldo 24
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 24, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada  Art142", 24, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Gratificación exenta", 24, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Reparto de utilidades 25
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 25, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada  Art142", 25, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU ISR exento", 25, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Indemnización 26
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 26, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Liquidacion gravado", 26, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Liquidacion exento", 26, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Separación Única 27
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 27, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Liquidacion gravado", 27, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Liquidacion exento", 27, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Prima de antiguedad 29
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 29, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 29, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Liquidacion gravado", 29, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Liquidacion exento", 29, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Fondo de ahorro empresa 31
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 31, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 31, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 31, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Despensa 32
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 32, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 32, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 32, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 32, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Deporte y cultura 33
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 33, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 33, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 33, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //Anticipo vacaciones Percepción 35
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 35, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Destajo - sueldo 36
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 36, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 36, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 36, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 36, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 36, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Comisión sueldo 37
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 37, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("PTU Ingresos acumulados para", 37, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 37, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 37, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Percepcion Variable", 37, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Fondo de ahorro Empresa 131
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Total de percepciones", 131, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Gravada", 131, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Base Exenta", 131, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //----------------------------------------------------------------------
            //Deducciones
            conceptType = ConceptType.DeductionPayment;

            //Ret. Inv. Y Vida 5
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 5 Invalidez y Vida", 5, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ret. Cesantia 6
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 6 Cesantia y Vejez", 6, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ret. Enf. y Mat. obrero 11
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 1 EG (Art. 25)", 11, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 3 EG (Art.106-II)", 11, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 4 EG (Art.107)", 11, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));

            //Seguro de vivienda Infonavit 14
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 14, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Préstamo Infonavit (vsm) 15
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 15, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Préstamo Infonavit (cf) 16
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 16, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Subs. al Empleo acreditado 32
            result.Add(createObjectRelathionshipFiscalAccumulates("Subs al Empleo  Acreditado", 32, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Subs. al Empleo acreditado 33
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR  a compensar o retener", 33, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Subsidio al Empleo (anual) 34
            // No tiene

            //Subsidio al Empleo (mes) 35
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR SUBS AL EMPLEO DEL", 35, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("Subsidio al empleo Entregado", 35, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //ISR antes de Subs. al Empleo 41
            result.Add(createObjectRelathionshipFiscalAccumulates("ISPT antes de Subs al Empleo", 41, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Directo", 41, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //ISR Art142 43
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR DEL", 43, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Subsidio acreditable", 43, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Subsidio no acreditable", 43, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //ISR (anual) 44

            //ISR (mes) 45
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR DEL", 45, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR retenido mes", 45, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Reintegro del ISR retenido 50
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR  a compensar o retener", 50, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //I.M.S.S. 52
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Total empleado", 52, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //I.E. 53

            //Cuota sindical 54
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 54, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //I.S.R. a compensar 55
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR  a compensar o retener", 55, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Caja de ahorro 56
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 56, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Préstamos caja de ahorro 57
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 57, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Intereses Ptmo ahorro 58
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 58, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Préstamo Infonavit 59
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 59, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Intereses Préstamo Infonavit 60
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 60, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Préstamo FONACOT 61
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 61, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //FONACOT revolvente 62
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 62, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Intereses ptmo FONACOT 63
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 63, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Préstamo empresa 64
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 64, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Intereses préstamo empresa 65
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 65, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Anticipo sueldo 66
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 66, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Fondo de ahorro 67
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 67, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Reintegración 69

            //Deduccion general 70
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 70, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ajuste en Subsidio para el empleo 71
            result.Add(createObjectRelathionshipFiscalAccumulates("Ajuste Subs Entregado a Cargo", 71, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Préstamo fondo de ahorro 72
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 72, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Intereses ptmo fondo de ahorro 73
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 73, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Anticipo vacaciones 74
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 74, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //subs entregado que no correspondía 75

            //Aportacion voluntaria infonavit 87
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 87, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Aportacion voluntaria SAR 88
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 8 Retiro", 88, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Subsidio acreditable 91

            //Subsidio no acreditable 92

            //Sobregiro 94
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 94, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ajuste el neto 99

            //I.S.R. finiquito 101
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR DEL", 101, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Subsidio acreditable", 101, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR Subsidio no acreditable", 101, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));

            //I.S.R. retenido del ejercicio vigente 102
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR DEL", 102, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Reintegro ISR en exceso de ejercicio ant 103
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR reintegro ejercicio anterior", 103, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //ISR de ajuste mensual 104
            result.Add(createObjectRelathionshipFiscalAccumulates("ISR DEL", 104, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //ISR ajustado por subsidio 105
            result.Add(createObjectRelathionshipFiscalAccumulates("Ajuste Subs Causado", 105, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Reintegro ISR retenido de mas 106
            result.Add(createObjectRelathionshipFiscalAccumulates("Reintegro ISR pago en exceso", 106, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ajuste al Subsidio causado 107
            result.Add(createObjectRelathionshipFiscalAccumulates("Ajuste Subs Causado a Cargo", 107, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo caja de ahorro2 157
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 157, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo caja de ahorro3 158
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 158, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo caja de ahorro4 159
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 159, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo empresa2 164
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 164, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo empresa3 165
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 165, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo empresa4 166
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 166, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo fondo de ahorro2 172
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 172, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo fondo de ahorro3 173
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 173, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Ptmo fondo de ahorro4 174
            result.Add(createObjectRelathionshipFiscalAccumulates("Otras deducciones", 174, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Concepto vacio 1 175

            //Concepto vacio 2 176

            //----------------------------------------------------------------------
            //Obligaciones
            conceptType = ConceptType.LiabilityPayment;

            //Invalidez y Vida 5
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 5 Invalidez y Vida", 5, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Cesantia y Vejez 6
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 6 Cesantia y Vejez", 6, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Enf. y Mat. Patron 7
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 1 EG (Art. 25)", 7, ConceptPaymentType.Amount1, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 2 EG (Art.106-I,108)", 7, ConceptPaymentType.Amount2, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 3 EG (Art.106-II)", 7, ConceptPaymentType.Amount3, conceptType, conceptPayments, accumulatedTypes));
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 4 EG (Art.107)", 7, ConceptPaymentType.Amount4, conceptType, conceptPayments, accumulatedTypes));

            //2% Fondo de retiro SAR (8) 89
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 8 Retiro", 89, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //2% Impuesto estatal 90

            //Riesgo de trabajo 93
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 9 Riesgo de Trabajo", 93, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //1% Educación empresa 95

            //I.M.S.S. empresa 96
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS Total Empresa", 96, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));

            //Infonavit empresa 97            

            //Guarderia I.M.S.S. (7) 98            
            result.Add(createObjectRelathionshipFiscalAccumulates("IMSS 7 Guarderias", 98, ConceptPaymentType.TotalAmount, conceptType, conceptPayments, accumulatedTypes));


            return result;
        }

        /// <summary>
        /// Creates default objects for accumulated Types
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="instanceID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Schema.AccumulatedType> GetDefaultAccumulatedType(Guid companyId, Guid instanceID, Guid user)
        {
            var result = new List<AccumulatedType>();

            string input =
                @"ISR Total de percepciones|P|1               
ISR Base Gravada|P|2                        
ISR Base Exenta|P|3                         
ISR DEL|P|4                                 
ISR SUBS AL EMPLEO DEL|P|5                  
ISR Subsidio acreditable|P|6                
ISR Subsidio no acreditable|P|7             
ISR Gratificación exenta|P|8                
ISR Prima vac. exenta|P|9                   
ISR Liquidacion exento|P|10                  
ISR Liquidacion gravado|P|11                 
PTU Ingresos acumulados para|P|12            
IMSS 3 EG (Art.106-II)|P|13                  
IMSS 2 EG (Art.106-I,108)|P|14               
IMSS 4 EG (Art.107)|P|15                     
IMSS 5 Invalidez y Vida|P|16                 
IMSS 6 Cesantia y Vejez|P|17                 
IMSS 1 EG (Art. 25)|P|18                     
IMSS 8 Retiro|P|19                           
IMSS 9 Riesgo de Trabajo|P|20                
IMSS 7 Guarderias|P|21                       
IMSS Percepcion Variable|P|22                
ISR Perc.especiales grav.|P|23               
Dias IMSS Incapacidad|D|24                   
Dias IMSS Ausencias|D|25                     
Dias PTU no participan|D|26                  
ISR Perc.especiales exentas|P|27             
PTU ISR exento|P|28                          
ISR Base Gravada  Art142|P|29                
IMSS Total empleado|P|30                     
IMSS Total Empresa|P|31                      
Otras deducciones|P|32                       
Acumulado especial 1|P|33                    
Acumulado especial 2|P|34                    
Acumulado especial 3|P|35                    
Acumulado especial 4|P|36                    
ISPT antes de Subs al Empleo|P|37            
Subs al Empleo  Acreditado|P|38              
ISR  a compensar o retener|P|39              
ISR reintegro ejercicio anterior|P|40
Ajuste Subs Causado|P|41
Ajuste Subs Causado a Cargo|P|42
Ajuste Subs Entregado a Cargo|P|43           
ISR retenido mes|P|44
Reintegro ISR pago en exceso|P|45
Subsidio al empleo Entregado|P|46
ISR Directo|P|47";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var accumulatedType = CreateDefaultObject<AccumulatedType>(id);
                accumulatedType.company = companyId;
                accumulatedType.InstanceID = instanceID;
                accumulatedType.user = user;

                //Name
                accumulatedType.Name = superSplitted[0];

                //TypeOfAccumulated
                if (superSplitted[1].StartsWith("P"))
                {
                    accumulatedType.TypeOfAccumulated = TypeOfAccumulated.SalaryPaymentDeductionLiability;
                }
                else
                {
                    accumulatedType.TypeOfAccumulated = TypeOfAccumulated.DaysHours;
                }

                accumulatedType.Code = Int32.Parse(superSplitted[2]);

                result.Add(accumulatedType);
            }

            return result;
        }

        public List<Schema.IncidentType> GetDefaultIncidentType(Guid companyId, Guid instanceID, Guid user, List<Schema.AccumulatedType> accumulatedTypes)
        {
            var result = new List<IncidentType>();

            string input =
                @"Hora(s) extra(s) |HE|0|0.00|N|H|0
Día trabajado|TRAB|1|100.00|N|D|0
Permiso con goce de sueldo|PCS|1|100.00|N|D|0
Permiso sin goce de sueldo|PSS|0|0.00|A|D|1
Accidente de trabajo|ATRB|0|0.00|I|D|0
Accidente de trayecto|ATRY|0|0.00|I|D|0
Enf. Gral./Acc. Fuera trab.|ENFG|0|0.00|I|D|1
Incapacidad pagada por la empresa|INC|0|0.00|I|D|0
Incapacidad por maternidad|MAT|0|0.00|I|D|0
Vacaciones a pagar|VAC|0|0.00|V|D|0
Falta injustificada|FINJ|0|0.00|A|D|1
Licencia 140 Bis|L140|1|100.00|I|D|0
Día Festivo Trabajado|DFT|1|100.00|N|D|0
Retardo(s)|RET|0|0.00|N|H|0";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.NewGuid();

                var incidentType = CreateDefaultObject<IncidentType>(id);
                incidentType.company = companyId;
                incidentType.InstanceID = instanceID;
                incidentType.user = user;

                //Name
                incidentType.Name = superSplitted[0];

                //Code
                incidentType.Code = superSplitted[1];

                //SalaryRight
                incidentType.SalaryRight = Convert.ToBoolean(Convert.ToInt16(superSplitted[2]));

                //Percentage
                incidentType.Percentage = decimal.Parse(superSplitted[3]);

                //ItConsiders
                if (superSplitted[4] == "N")
                {
                    incidentType.ItConsiders = ItConsiders.None;
                }
                else if (superSplitted[4] == "A")
                {
                    incidentType.ItConsiders = ItConsiders.Absence;
                }
                else if (superSplitted[4] == "V")
                {
                    incidentType.ItConsiders = ItConsiders.Holidays;
                }
                else if (superSplitted[4] == "I")
                {
                    incidentType.ItConsiders = ItConsiders.Inhability;
                }

                //Type
                if (superSplitted[5] == "D")
                {
                    incidentType.TypeOfIncident = TypeOfIncident.Days;
                }
                else if (superSplitted[5] == "H")
                {
                    incidentType.TypeOfIncident = TypeOfIncident.Hours;
                }

                //DecreasesSeventhDay
                incidentType.DecreasesSeventhDay = Convert.ToBoolean(Convert.ToInt16(superSplitted[6]));

                var accumulatedImssDays = accumulatedTypes.FirstOrDefault(p => p.Name == "Dias IMSS Incapacidad");
                var accumulatedImssAbsense = accumulatedTypes.FirstOrDefault(p => p.Name == "Dias IMSS Ausencias");
                var accumulatedNoPTU = accumulatedTypes.FirstOrDefault(p => p.Name == "Dias PTU no participan");

                //TODO: AccumulatedType
                if (incidentType.Code == "ATRB")
                {
                    incidentType.AccumulatedTypes.Add(accumulatedImssDays);
                }
                else if (incidentType.Code == "ATRY")
                {
                    incidentType.AccumulatedTypes.Add(accumulatedImssDays);
                }
                else if (incidentType.Code == "L140")
                {
                    incidentType.AccumulatedTypes.Add(accumulatedImssDays);
                }
                else if (incidentType.Code == "CAST" || incidentType.Code == "FINJ" || incidentType.Code == "PSS ")
                {
                    incidentType.AccumulatedTypes.Add(accumulatedImssAbsense);
                    incidentType.AccumulatedTypes.Add(accumulatedNoPTU);
                }
                else if (incidentType.Code == "ENFG")
                {
                    incidentType.AccumulatedTypes.Add(accumulatedImssDays);
                    incidentType.AccumulatedTypes.Add(accumulatedNoPTU);
                }
                else if (incidentType.Code == "INC ")
                {
                    incidentType.AccumulatedTypes.Add(accumulatedImssDays);
                }
                else if (incidentType.Code == "MAT ")
                {
                    incidentType.AccumulatedTypes.Add(accumulatedImssDays);
                }

                result.Add(incidentType);
            }

            return result;
        }

        public List<Schema.IncidentTypeRelationship> GetDefaultIncidentTypeRelationship(Guid companyId, Guid instanceID, Guid user, List<Schema.IncidentType> incidentTypes)
        {
            var lstincidentRelationship = new List<Schema.IncidentTypeRelationship>();

            if (incidentTypes.Any())
            {
                Parallel.ForEach(incidentTypes, (incidentType) =>
                {
                    if (incidentType.AccumulatedTypes.Any())
                    {
                        incidentType.AccumulatedTypes.ForEach(accumulatedType =>
                        {
                            var idRelationship = Guid.NewGuid();
                            var relationship = CreateDefaultObject<IncidentTypeRelationship>(idRelationship);
                            relationship.company = companyId;
                            relationship.InstanceID = instanceID;
                            relationship.user = user;

                            relationship.IncidentTypeID = incidentType.ID;
                            relationship.AccumulatedTypeID = accumulatedType.ID;

                            lstincidentRelationship.Add(relationship);
                        });
                    }
                });
            }

            return lstincidentRelationship;
        }

        public List<Schema.PeriodType> GetDefaultPeriodType(Guid companyId, Guid instanceId, Guid user,
            PaymentPeriodicity paymentPeriodicity, decimal paymentDays, AdjustmentPay_16Days_Febrary adjustmentPay,
            bool forceUpdate = false, WeeklySeventhDay weeklySeventhDay = WeeklySeventhDay.None)
        {
            var periodTypes = new List<PeriodType>();
            var namePeriodicity = Enum.GetName(typeof(PaymentPeriodicity), paymentPeriodicity);
            var periodTotalDays = 0;
            var paymentDayPosition = 0;
            bool monthCalendarFixed = true;
            var seventhDays = 0;
            var seventDayPosition = String.Empty;

            if (weeklySeventhDay != WeeklySeventhDay.None)
            {
                seventhDays = 1;
            }



            seventDayPosition = ((int)weeklySeventhDay).ToString();


            if (paymentPeriodicity == PaymentPeriodicity.Biweekly)
            {
                namePeriodicity = "Quincenal";
                periodTotalDays = 15;
                paymentDayPosition = 15;
                monthCalendarFixed = true;
                paymentDays = !forceUpdate ? 15 : paymentDays;
            }
            else if (paymentPeriodicity == PaymentPeriodicity.Monthly)
            {
                namePeriodicity = "Mensual";
                periodTotalDays = 30;
                paymentDayPosition = 30;
                monthCalendarFixed = true;
                paymentDays = !forceUpdate ? 30 : paymentDays;
            }
            else if (paymentPeriodicity == PaymentPeriodicity.Weekly)
            {
                namePeriodicity = "Semanal";
                periodTotalDays = 7;
                paymentDayPosition = 7;
                monthCalendarFixed = false;
                paymentDays = !forceUpdate ? 7 : paymentDays;

            }
            else if (paymentPeriodicity == PaymentPeriodicity.OtherPeriodicity)
            {
                namePeriodicity = "Extraordinario";
                periodTotalDays = 1;
                paymentDayPosition = 1;
                monthCalendarFixed = false;
                paymentDays = !forceUpdate ? 1 : paymentDays;
            }

            periodTypes.Add(new PeriodType()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = companyId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                user = user,
                Description = $"Periodo de pago {namePeriodicity}",
                CreationDate = DateTime.UtcNow,
                Name = namePeriodicity,
                StatusID = 1,
                PaymentPeriodicity = paymentPeriodicity,
                PeriodTotalDays = periodTotalDays,
                PaymentDays = paymentDays,
                ExtraordinaryPeriod = (paymentPeriodicity == PaymentPeriodicity.OtherPeriodicity),
                MonthCalendarFixed = monthCalendarFixed,
                FortnightPaymentDays = adjustmentPay,
                PaymentDayPosition = paymentDayPosition,
                SeventhDays = seventhDays,
                SeventhDayPosition = seventDayPosition,
            });

            return periodTypes;
        }

        public List<Schema.Period> GetDefaultPeriod(Guid companyId, Guid instanceId, Guid user, DateTime initialDate,
            DateTime finalDate, int fiscalYear, PeriodType periodType)
        {
            var result = new List<Period>();

            result.Add(new Period()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = companyId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = $"Periodo del Ejercicio {fiscalYear}",
                CreationDate = DateTime.UtcNow,
                user = user,
                Name = $"{fiscalYear}",
                InitialDate = initialDate,
                FinalDate = finalDate,
                FiscalYear = fiscalYear,
                IsActualFiscalYear = true,
                IsFiscalYearClosed = false,
                PeriodTypeID = periodType.ID,
                ExtraordinaryPeriod = periodType.ExtraordinaryPeriod,
                FortnightPaymentDays = periodType.FortnightPaymentDays,
                MonthCalendarFixed = periodType.MonthCalendarFixed,
                PaymentDayPosition = periodType.PaymentDayPosition,
                PaymentDays = periodType.PaymentDays,
                PaymentPeriodicity = periodType.PaymentPeriodicity,
                PeriodTotalDays = periodType.PeriodTotalDays,
                StatusID = 1,
                SeventhDays = periodType.SeventhDays,
                SeventhDayPosition = periodType.SeventhDayPosition,
            });

            return result;
        }

        /// <summary>
        /// Getds the default umi.
        /// Only for Repository purposes not use in create company
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="instanceID">The instance identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public List<UMI> GetdDefaultUMI()
        {
            var result = new List<UMI>();

            string input =
                          @"3BCB2A5D-9E8E-4274-AF1B-0630FD93F0B2|01/01/2017|75.49
                            D3EED992-7762-48B3-8B30-7F26BFF9BAF3|01/01/2018|78.43
                            470CE473-B26C-4DB9-BB3E-BF1F9344DFDD|01/01/2019|82.22
                            8ACB4512-6A87-4F9C-B5C7-62AF4598DA42|01/01/2020|84.55";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.Parse(superSplitted[0].Trim());

                var umi = CreateDefaultObjectEntity<UMI>(id);
                var date = DateTime.ParseExact(superSplitted[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                umi.ValidityDate = date;
                umi.Value = decimal.Parse(superSplitted[2].Trim());

                result.Add(umi);
            }

            return result;
        }

        /// <summary>
        /// Getds the default umi.
        /// Only for Repository purposes not use in create company
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="instanceID">The instance identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public List<InfonavitInsurance> GetdDefaultInfonavitInsurance()
        {
            var result = new List<InfonavitInsurance>();

            string input =
                          @"6ca2960b-0106-4786-8fbb-ebd90f3cd18e|01/01/2009|13.00
                            41f18484-eb4f-4629-81cb-a24b5f9c73de|01/01/2010|15.00";

            var splitted = splitRow(input);

            for (int i = 0; i < splitted.Length; i++)
            {
                var superSplitted = splitColumn(splitted[i]);

                var id = Guid.Parse(superSplitted[0].Trim());

                var infonavitInsurance = CreateDefaultObjectEntity<InfonavitInsurance>(id);
                var date = DateTime.ParseExact(superSplitted[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                infonavitInsurance.ValidityDate = date;
                infonavitInsurance.Value = decimal.Parse(superSplitted[2].Trim());

                result.Add(infonavitInsurance);
            }

            return result;
        }

    }
}

using CotorraNode.Common.Library.Public;
using MoreLinq;
using Cotorra.Core.Log;
using Cotorra.Core.Managers.FiscalStampingProviders.PAC;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using Cotorra.Schema.CFDI33Nom12;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace Cotorra.Core.Managers.FiscalStamping
{
    /// <summary>
    /// Versión de Stmaping Provider _ CFDI v3.3 y Nom v1.2
    /// </summary>
    public class FiscalStampingCFDIv33Nomv12 : FiscalStampingBase, IFiscalStamping
    {
        #region "Attributes"
        private Dictionary<string, c_Estado> statesDictionary;
        private Dictionary<string, c_RegimenFiscal> fiscalRegimesDictionary;
        private Dictionary<OverdraftType, c_TipoNomina> overdraftTypeDictionary;
        private Dictionary<SNCFOriginResourceType, c_OrigenRecurso> originResourceDictionary;
        private Dictionary<ContractType, c_TipoContrato> contractTypeDictionary;
        private Dictionary<EmployeeTrustLevel, NominaReceptorSindicalizado> employeeTrustLevelDictionary;
        private Dictionary<ShiftWorkingDayType, c_TipoJornada> shiftWorkingDayTypeDictionary;
        private Dictionary<EmployeeRegimeType, c_TipoRegimen> employeeRegimeTypeDictionary;
        private Dictionary<JobPositionRiskType, c_RiesgoPuesto> jobPositionRiskTypeDictionary;
        private Dictionary<PaymentPeriodicity, c_PeriodicidadPago> paymentPeriodicityDictionary;
        private Dictionary<CategoryInsurance, c_TipoIncapacidad> inhabilitTypeDictionary;
        private const string CFDI_VERSION = "3.3";
        private const string PAYROLL_VERSION = "1.2";
        private readonly FiscalStampingVersion _fiscalStampingVersion;
        #endregion

        #region "Constructor"
        private void fillStatesDictionary()
        {
            statesDictionary = new Dictionary<string, c_Estado>();
            statesDictionary.Add("Aguascalientes", c_Estado.AGU);
            statesDictionary.Add("Baja California", c_Estado.BCN);
            statesDictionary.Add("Baja California Sur", c_Estado.BCS);
            statesDictionary.Add("Campeche", c_Estado.CAM);
            statesDictionary.Add("Chiapas", c_Estado.CHP);
            statesDictionary.Add("Chihuahua", c_Estado.CHH);
            statesDictionary.Add("Coahuila", c_Estado.COA);
            statesDictionary.Add("Colima", c_Estado.COL);
            statesDictionary.Add("Jalisco", c_Estado.JAL);
            statesDictionary.Add("Ciudad de México", c_Estado.DIF);
            statesDictionary.Add("Durango", c_Estado.DUR);
            statesDictionary.Add("Guanajuato", c_Estado.GUA);
            statesDictionary.Add("Guerrero", c_Estado.GRO);
            statesDictionary.Add("Hidalgo", c_Estado.HID);
            statesDictionary.Add("Estado de México", c_Estado.MEX);
            statesDictionary.Add("Michoacán", c_Estado.MIC);
            statesDictionary.Add("Morelos", c_Estado.MOR);
            statesDictionary.Add("Nayarit", c_Estado.NAY);
            statesDictionary.Add("Nuevo León", c_Estado.NLE);
            statesDictionary.Add("Oaxaca", c_Estado.OAX);
            statesDictionary.Add("Puebla", c_Estado.PUE);
            statesDictionary.Add("Querétaro", c_Estado.QUE);
            statesDictionary.Add("Quintana Roo", c_Estado.ROO);
            statesDictionary.Add("San Luis Potosí", c_Estado.SLP);
            statesDictionary.Add("Sinaloa", c_Estado.SIN);
            statesDictionary.Add("Sonora", c_Estado.SON);
            statesDictionary.Add("Tabasco", c_Estado.TAB);
            statesDictionary.Add("Tamaulipas", c_Estado.TAM);
            statesDictionary.Add("Tlaxcala", c_Estado.TLA);
            statesDictionary.Add("Veracruz", c_Estado.VER);
            statesDictionary.Add("Yucatán", c_Estado.YUC);
            statesDictionary.Add("Zacatecas", c_Estado.ZAC);

        }

        private void fillFiscalRegimesDictionary()
        {
            fiscalRegimesDictionary = new Dictionary<string, c_RegimenFiscal>();
            fiscalRegimesDictionary.Add("601", c_RegimenFiscal.Item601);
            fiscalRegimesDictionary.Add("603", c_RegimenFiscal.Item603);
            fiscalRegimesDictionary.Add("605", c_RegimenFiscal.Item605);
            fiscalRegimesDictionary.Add("606", c_RegimenFiscal.Item606);
            fiscalRegimesDictionary.Add("607", c_RegimenFiscal.Item607);
            fiscalRegimesDictionary.Add("608", c_RegimenFiscal.Item608);
            fiscalRegimesDictionary.Add("610", c_RegimenFiscal.Item610);
            fiscalRegimesDictionary.Add("611", c_RegimenFiscal.Item611);
            fiscalRegimesDictionary.Add("612", c_RegimenFiscal.Item612);
            fiscalRegimesDictionary.Add("614", c_RegimenFiscal.Item614);
            fiscalRegimesDictionary.Add("615", c_RegimenFiscal.Item615);
            fiscalRegimesDictionary.Add("616", c_RegimenFiscal.Item616);
            fiscalRegimesDictionary.Add("620", c_RegimenFiscal.Item620);
            fiscalRegimesDictionary.Add("621", c_RegimenFiscal.Item621);
            fiscalRegimesDictionary.Add("622", c_RegimenFiscal.Item622);
            fiscalRegimesDictionary.Add("623", c_RegimenFiscal.Item623);
            fiscalRegimesDictionary.Add("624", c_RegimenFiscal.Item624);
            fiscalRegimesDictionary.Add("628", c_RegimenFiscal.Item628);
            fiscalRegimesDictionary.Add("629", c_RegimenFiscal.Item629);
            fiscalRegimesDictionary.Add("630", c_RegimenFiscal.Item630);
        }

        private void fillOverdraftTypeDictionary()
        {
            overdraftTypeDictionary = new Dictionary<OverdraftType, c_TipoNomina>();
            overdraftTypeDictionary.Add(OverdraftType.Ordinary, c_TipoNomina.O);
            overdraftTypeDictionary.Add(OverdraftType.Extraordinary, c_TipoNomina.E);
        }

        private void fillOriginResourceDictionary()
        {
            originResourceDictionary = new Dictionary<SNCFOriginResourceType, c_OrigenRecurso>();
            originResourceDictionary.Add(SNCFOriginResourceType.IF, c_OrigenRecurso.IF);
            originResourceDictionary.Add(SNCFOriginResourceType.IM, c_OrigenRecurso.IM);
            originResourceDictionary.Add(SNCFOriginResourceType.IP, c_OrigenRecurso.IP);
        }

        private void fillEmployeeTrustLevel()
        {
            employeeTrustLevelDictionary = new Dictionary<EmployeeTrustLevel, NominaReceptorSindicalizado>();
            employeeTrustLevelDictionary.Add(EmployeeTrustLevel.Trusted, NominaReceptorSindicalizado.No);
            employeeTrustLevelDictionary.Add(EmployeeTrustLevel.Unionized, NominaReceptorSindicalizado.Sí);
        }

        private void fillContractType()
        {
            contractTypeDictionary = new Dictionary<ContractType, c_TipoContrato>();
            //01 Contrato de trabajo por tiempo indeterminado
            contractTypeDictionary.Add(ContractType.IndefiniteTermEmploymentContract, c_TipoContrato.Item01);
            //02	Contrato de trabajo para obra determinada
            contractTypeDictionary.Add(ContractType.WorkContractForSpecificWork, c_TipoContrato.Item02);
            //03	Contrato de trabajo por tiempo determinado
            contractTypeDictionary.Add(ContractType.FixedTermEmploymentContract, c_TipoContrato.Item03);
            //04	Contrato de trabajo por temporada
            contractTypeDictionary.Add(ContractType.SeasonalEmploymentContract, c_TipoContrato.Item04);
            //05 Contrato de trabajo sujeto a prueba
            contractTypeDictionary.Add(ContractType.TestEmploymentContract, c_TipoContrato.Item05);
            //06 Contrato de trabajo con capacitación inicial
            contractTypeDictionary.Add(ContractType.EmploymentContractWithInitialTraining, c_TipoContrato.Item06);
            //07 Modalidad de pago por horas trabajadas
            contractTypeDictionary.Add(ContractType.ContractModalityForPaymentOfHoursWorked, c_TipoContrato.Item07);
            //08 Modalidad de comisiones por trabajo
            contractTypeDictionary.Add(ContractType.ModalityOfWorkByLaborCommission, c_TipoContrato.Item08);
            //09 Comisiones cuando no hay relación laboral con el empleador
            contractTypeDictionary.Add(ContractType.ContractModalitiesWhereThereIsNoEmploymentRelationship, c_TipoContrato.Item09);
            //10 Jubilados, Retirados y Pensionados
            contractTypeDictionary.Add(ContractType.RetirementPensionAndRetirement, c_TipoContrato.Item10);

            //Another one
            contractTypeDictionary.Add(ContractType.OtherContract, c_TipoContrato.Item99);
        }

        private void fillShiftWorkingDayType()
        {
            shiftWorkingDayTypeDictionary = new Dictionary<ShiftWorkingDayType, c_TipoJornada>();
            //01	Diurna
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.Daytime, c_TipoJornada.Item01);
            // 02	Nocturna
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.Night, c_TipoJornada.Item02);
            // 03	Mixta
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.Mixed, c_TipoJornada.Item03);
            // 04	Por hora
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.PerHours, c_TipoJornada.Item04);
            // 05	Reducida
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.Reduced, c_TipoJornada.Item05);
            // 06	Continuada
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.Continued, c_TipoJornada.Item06);
            // 07	Partida
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.Entry, c_TipoJornada.Item07);
            // 08	Por turnos
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.ByTurns, c_TipoJornada.Item08);
            // 99	OtraJornada
            shiftWorkingDayTypeDictionary.Add(ShiftWorkingDayType.Other, c_TipoJornada.Item99);
        }

        private void fillEmployeeRegimeType()
        {
            employeeRegimeTypeDictionary = new Dictionary<EmployeeRegimeType, c_TipoRegimen>();
            employeeRegimeTypeDictionary.Add(EmployeeRegimeType.Salaries, c_TipoRegimen.Item02);
            employeeRegimeTypeDictionary.Add(EmployeeRegimeType.Retired, c_TipoRegimen.Item03);
            employeeRegimeTypeDictionary.Add(EmployeeRegimeType.AssimilatedFee, c_TipoRegimen.Item09);
            employeeRegimeTypeDictionary.Add(EmployeeRegimeType.Compensation, c_TipoRegimen.Item13);
            employeeRegimeTypeDictionary.Add(EmployeeRegimeType.Other, c_TipoRegimen.Item99);
        }

        private void fillJobPositionRiskType()
        {
            jobPositionRiskTypeDictionary = new Dictionary<JobPositionRiskType, c_RiesgoPuesto>();
            jobPositionRiskTypeDictionary.Add(JobPositionRiskType.Class_I, c_RiesgoPuesto.Item1);
            jobPositionRiskTypeDictionary.Add(JobPositionRiskType.Class_II, c_RiesgoPuesto.Item2);
            jobPositionRiskTypeDictionary.Add(JobPositionRiskType.Class_III, c_RiesgoPuesto.Item3);
            jobPositionRiskTypeDictionary.Add(JobPositionRiskType.Class_IV, c_RiesgoPuesto.Item4);
            jobPositionRiskTypeDictionary.Add(JobPositionRiskType.Class_V, c_RiesgoPuesto.Item5);
            jobPositionRiskTypeDictionary.Add(JobPositionRiskType.NA, c_RiesgoPuesto.Item99);
        }

        private void fillPaymentPeriodicity()
        {
            paymentPeriodicityDictionary = new Dictionary<PaymentPeriodicity, c_PeriodicidadPago>();
            //Bimestral
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.Bimonthly, c_PeriodicidadPago.Item06);
            //Quincenal
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.Biweekly, c_PeriodicidadPago.Item04);
            //Comisión
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.Commission, c_PeriodicidadPago.Item08);
            //Diario
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.Daily, c_PeriodicidadPago.Item01);
            //Decenal
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.Decennial, c_PeriodicidadPago.Item99);
            //Precio alzado
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.ElevatedPrice, c_PeriodicidadPago.Item09);
            //Catorcenal
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.Fourteen, c_PeriodicidadPago.Item03);
            //Mes
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.Monthly, c_PeriodicidadPago.Item05);
            //Semanal
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.Weekly, c_PeriodicidadPago.Item02);
            //Unidad de obra
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.WorkUnit, c_PeriodicidadPago.Item07);
            //Otra periodicidad
            paymentPeriodicityDictionary.Add(PaymentPeriodicity.OtherPeriodicity, c_PeriodicidadPago.Item99);
        }

        private void fillInhabilitType()
        {
            inhabilitTypeDictionary = new Dictionary<CategoryInsurance, c_TipoIncapacidad>();
            inhabilitTypeDictionary.Add(CategoryInsurance.WorkRisk, c_TipoIncapacidad.Item01);
            inhabilitTypeDictionary.Add(CategoryInsurance.GeneralDisease, c_TipoIncapacidad.Item02);
            inhabilitTypeDictionary.Add(CategoryInsurance.PrenatalMaternity, c_TipoIncapacidad.Item03);
        }

        public FiscalStampingCFDIv33Nomv12()
        {
            //Fill the dictionaries
            fillStatesDictionary();
            fillFiscalRegimesDictionary();
            fillOverdraftTypeDictionary();
            fillOriginResourceDictionary();
            fillContractType();
            fillEmployeeTrustLevel();
            fillShiftWorkingDayType();
            fillEmployeeRegimeType();
            fillJobPositionRiskType();
            fillPaymentPeriodicity();
            fillInhabilitType();

            //override xslt path
            base.xsltPath = Path.Combine("fiscal", "cfdi33nom12", "xslt", "cadenaoriginal_3_3.xslt");
            base.xsltTFDPath = Path.Combine("fiscal", "cfdi33nom12", "xslt", "cadenaoriginal_TFD_1_1.xslt");

            //override namespaces
            base.CFDI_Namespace = "http://www.sat.gob.mx/cfd/3";
            base.TFD_Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital";
            base.XSI_Namespace = "http://www.w3.org/2001/XMLSchema-instance";
            base.Payroll_Namespace = "http://www.sat.gob.mx/nomina12";

            //xmlns
            base.Payroll_xmlns = "xmlns:nomina12";
            base.TFD_xmlns = "xmlns:tfd";

            //prefix 
            base.CFDI_PrefixName = "cfdi";
            base.TFD_PrefixName = "tfd";
            base.Payroll_PrefixName = "nomina12";

            //Seal XPath
            base.SealXPath = "cfdi:Comprobante/cfdi:Complemento/tfd:TimbreFiscalDigital";

            _fiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12;
        }
        #endregion

        [DataContract]
        public class HEResult
        {
            [DataMember]
            public int HEDoubleDays { get; set; }

            [DataMember]
            public double HEDoubleHours { get; set; }

            [DataMember]
            public double HEDoublesAmount { get; set; }

            [DataMember]
            public int HETripleDays { get; set; }

            [DataMember]
            public double HETripleHours { get; set; }

            [DataMember]
            public double HETriplesAmount { get; set; }
        }

        private HEResult CalculateHE(CreateComprobanteParams createComprobanteParams)
        {
            //Salario Diario           
            HEResult hEResult = new HEResult();
            const int MAX_DOUBLE_PER_WEEK = 9;
            var workingHours = createComprobanteParams.Overdraft.HistoricEmployee.Employee.Workshift.Hours;
            var dailySalaryVig = Convert.ToDouble(createComprobanteParams.Overdraft.HistoricEmployee.DailySalary);
            var salaryxHour = (dailySalaryVig / workingHours);

            var incidents = createComprobanteParams.Incidents;

            var incidentHE = incidents.Where(p =>
                    p.EmployeeID == createComprobanteParams.Overdraft.EmployeeID &&
                    p.IncidentType.TypeOfIncident == TypeOfIncident.Hours &&
                    p.IncidentType.Code == "HE");

            var initialDate = createComprobanteParams.Overdraft.PeriodDetail.InitialDate;
            var finalDate = createComprobanteParams.Overdraft.PeriodDetail.FinalDate;
            var initialTempDate = initialDate;
            var finalTempDate = initialTempDate.AddDays(7);

            while (finalTempDate >= initialDate && finalTempDate <= finalDate &&
                initialTempDate >= initialDate && initialTempDate <= finalDate)
            {
                //incidencias de la semana
                var incidentsFirstRule = incidentHE.Where(p =>
                      p.Date >= initialTempDate &&
                      p.Date <= finalTempDate).OrderBy(p => p.Date);

                //Suma de las incidencias semanales
                var WeeklyIncidents = incidentsFirstRule.Sum(p => p.Value);

                if (WeeklyIncidents > MAX_DOUBLE_PER_WEEK)
                {
                    hEResult.HEDoublesAmount += salaryxHour * MAX_DOUBLE_PER_WEEK * 2;
                    hEResult.HETriplesAmount += salaryxHour * (Convert.ToDouble(WeeklyIncidents) - MAX_DOUBLE_PER_WEEK) * 3;
                    hEResult.HEDoubleHours += MAX_DOUBLE_PER_WEEK;
                    hEResult.HETripleHours += (Convert.ToDouble(WeeklyIncidents) - MAX_DOUBLE_PER_WEEK);
                    var weeklyAccumulated = 0.0m;
                    foreach (var inci in incidentsFirstRule)
                    {
                        weeklyAccumulated += inci.Value;
                        if (weeklyAccumulated > MAX_DOUBLE_PER_WEEK)
                        {
                            hEResult.HETripleDays++;

                            if ((weeklyAccumulated - inci.Value) < MAX_DOUBLE_PER_WEEK)
                            {
                                hEResult.HEDoubleDays++;
                            }
                        }
                        else
                        {
                            hEResult.HEDoubleDays++;
                        }
                    }
                }
                else
                {
                    hEResult.HEDoublesAmount += salaryxHour * Convert.ToDouble(WeeklyIncidents) * 2;
                    hEResult.HEDoubleDays += incidentsFirstRule.Count();
                    hEResult.HEDoubleHours += Convert.ToDouble(WeeklyIncidents);
                }

                initialTempDate = initialTempDate.AddDays(7 + 1);

                //regla de poner los dias restantes del periodo                        
                finalTempDate = finalTempDate.AddDays(7);
                if (finalTempDate > finalDate)
                {
                    finalTempDate = finalDate;
                }
            }

            return hEResult;
        }

        /// <summary>
        /// Create comprobante
        /// </summary>
        /// <param name="payrollStampingParams"></param>
        /// <param name="payrollCompanyConfiguration"></param>
        /// <param name="overdraft"></param>
        /// <param name="createComprobanteParams.OverdraftResults"></param>
        /// <returns></returns>
        public ICFDINomProvider CreateComprobante(CreateComprobanteParams createComprobanteParams)
        {
            //Obtiene la moneda para el cálculo - CFDI
            Enum.TryParse(createComprobanteParams.RoundUtil.GetCurrencyName(), out c_Moneda c_money);

            DateTimeUtil dateTimeUtil = new DateTimeUtil();

            createComprobanteParams.OverdraftResults.TotalSalaryPayments = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalExempt) + createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalTaxed);

            var comprobante = new Comprobante();
            //Debe tener el valor "3.3" para esta versión, Este dato lo integra el sistema que utiliza el contribuyente para la emisión del
            //comprobante fiscal.
            comprobante.Version = CFDI_VERSION;
            if (!string.IsNullOrEmpty(createComprobanteParams.PayrollStampingDetail.Series))
            {
                comprobante.Serie = createComprobanteParams.PayrollStampingDetail.Series;
            }
            else
            {
                comprobante.Serie = createComprobanteParams.Overdraft.PeriodDetail.InitialDate.Year.ToString();
            }
            if (!string.IsNullOrEmpty(createComprobanteParams.PayrollStampingDetail.Folio))
            {
                comprobante.Folio = createComprobanteParams.PayrollStampingDetail.Folio;
            }
            else
            {
                comprobante.Folio = createComprobanteParams.Overdraft.PeriodDetail.Number.ToString();
            }
            comprobante.Fecha = dateTimeUtil.FixDateTime(createComprobanteParams.CFDIDateTimeStamp);

            /*Es el sello digital del comprobante fiscal generado con el certificado de sello
            digital del contribuyente emisor del comprobante; éste funge como la firma
            del emisor del comprobante y lo integra el sistema que utiliza el
            contribuyente para la emisión del comprobante.
            FormaPago Se debe registrar la clave de la forma de pago “99” (Por definir) del catálogo
            c_FormaPago, es aplicable al comprobante emitido al trabajador asalariado o
            asimilado a salarios*/
            comprobante.FormaPago = c_FormaPago.Item99;
            comprobante.FormaPagoSpecified = true;

            //Información del sellado /*Se llena hasta el final*/
            comprobante.Sello = null;
            comprobante.NoCertificado = null;
            comprobante.Certificado = null;

            //Este campo no debe existir. 
            comprobante.CondicionesDePago = null;

            //Es el importe del concepto antes de descuentos e impuestos. No se permiten
            //valores negativos.
            //El importe registrado en este campo debe tener hasta la cantidad de
            //decimales que soporte la moneda.
            comprobante.SubTotal = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalSalaryPayments + createComprobanteParams.OverdraftResults.TotalOtherPayments); //Total de Percepciones
            comprobante.Descuento = createComprobanteParams.OverdraftResults.TotalDeductionPayments; //Total de Deducciones
            comprobante.DescuentoSpecified = true;

            //moneda
            comprobante.Moneda = c_money;

            //Este campo no debe existir. 
            comprobante.TipoCambio = 0;
            comprobante.TipoCambioSpecified = false;

            //Total
            comprobante.Total = createComprobanteParams.RoundUtil.RoundValue(comprobante.SubTotal - comprobante.Descuento);
           
            if (comprobante.SubTotal == 0 && comprobante.Descuento == 0)
            {
                throw new CotorraException(103, "103",
                    $"El recibo del empleado {createComprobanteParams.Overdraft.HistoricEmployee.FullName} no se requiere timbrarlo por que no tiene percepciones ni deducciones, no te preocupes se quedará en estado Por Timbrar para llevar su registro histórico", null);
            }

            //Se debe registrar la clave “N” (Nómina) con la que se identifica el tipo de
            //comprobante fiscal para el contribuyente emisor.
            comprobante.TipoDeComprobante = c_TipoDeComprobante.N;

            //Se debe registrar la clave PUE (Pago en una sola exhibición) del catálogo
            //c_MetodoPago publicado en el Portal del SAT.
            comprobante.MetodoPago = c_MetodoPago.PUE;
            comprobante.MetodoPagoSpecified = true;

            /*Se debe registrar el código postal del lugar de expedición del comprobante
            (domicilio de la matriz o de la sucursal), debe corresponder con una clave de
            código postal incluida en el catálogo.
            Al ingresar el Código Postal en este campo se cumple con el requisito de
            señalar el domicilio y lugar de expedición del comprobante a que se refieren
            las fracciones I y III del artículo 29-A del CFF, en los términos de la regla 2.7.1.32.,
            fracción I, de la Resolución Miscelánea Fiscal vigente.
            En el caso de que se emita un comprobante fiscal en una sucursal, en dicho
            comprobante se debe registrar el código postal de ésta, independientemente
            de que los sistemas de facturación de la empresa se encuentren en un
            domicilio distinto al de la sucursal.*/
            comprobante.LugarExpedicion = Int32.Parse(createComprobanteParams.ZipCode);

            /*
             Se debe registrar la clave de confirmación única e irrepetible que entrega el
            proveedor de certificación de CFDI o el SAT a los emisores (usuarios) para
            expedir el comprobante con importes fuera del rango establecido.
            Ejemplo:
            Confirmacion= ECVH1
            Se deben registrar valores alfanuméricos a 5 posiciones.
            Nota importante:
            El uso de esta clave estará vigente únicamente a partir de que el SAT
            publique en su Portal de Internet los procedimientos para generar la clave
            de confirmación y para parametrizar los montos y rangos máximos
            aplicables.
             */
            comprobante.Confirmacion = null;

            /*Nodo: CfdiRelacionados En este nodo se puede expresar la información del comprobante con Tipo de
            comprobante “N” relacionado que sustituya con los datos correctos al CDFI
            de nómina emitido anteriormente cuando existan errores.
            TipoRelacion Se debe registrar la clave “04” (Sustitución de los CFDI previos) de la relación
            que existe entre este comprobante que se está generando y el CFDI que se
            sustituye.
            Ejemplo:             
                TipoRelacion= 04
            Nodo:CfdiRelacionado En este nodo se debe expresar la información del comprobante relacionado.
            UUID Se debe registrar el folio fiscal (UUID) de un CFDI de nómina relacionado que
            sustituye con el presente comprobante.
            Ejemplo:
                UUID= 5FB2822E-396D-4725-8521-CDC4BDD20CCF*/
            if (createComprobanteParams.Overdraft.OverdraftPreviousCancelRelationshipID != null)
            {
                comprobante.CfdiRelacionados = new ComprobanteCfdiRelacionados();
                comprobante.CfdiRelacionados.TipoRelacion = c_TipoRelacion.Item04; //Sustitución de los CFDI previos
                comprobante.CfdiRelacionados.CfdiRelacionado = new ComprobanteCfdiRelacionadosCfdiRelacionado[1];
                comprobante.CfdiRelacionados.CfdiRelacionado[0] = new ComprobanteCfdiRelacionadosCfdiRelacionado();
                comprobante.CfdiRelacionados.CfdiRelacionado[0].UUID = createComprobanteParams.Overdraft.OverdraftPreviousCancelRelationship?.UUID.ToString();
            }

            //EMISOR
            /*En este nodo se debe expresar la información del contribuyente que emite el
            comprobante fiscal (empleador).
            Rfc Se debe registrar la Clave en el Registro Federal de Contribuyentes del emisor
            del comprobante.
            En el caso de que el emisor sea una persona física, este campo debe contener
            una longitud de 13 posiciones, tratándose de personas morales debe contener
            una longitud de 12 posiciones.
            ****Cuando el campo contenga un RFC de persona moral, no debe existir el
            campo CURP.
            Ejemplo:
            En el caso de una persona física se debe registrar:
                Rfc= CABL840215RF4
            En el caso de una persona moral se debe registrar:
                Rfc= PAL7202161U0*/
            comprobante.Emisor = new ComprobanteEmisor();

            /*Se puede registrar el nombre, denominación o razón social del emisor del
            comprobante.

            Ejemplo:
                En el caso de una persona física se debe registrar:
                    Nombre = Marton Aleejandro Sanzi Fierror
                En el caso de una persona moral se debe registrar:
                    Nombre = La Palma Aei0 S A de C V
            */
            comprobante.Emisor.Rfc = createComprobanteParams.PayrollCompanyConfiguration.RFC;

            /*Razón social del Emisor (Empleador)*/
            comprobante.Emisor.Nombre = createComprobanteParams.PayrollCompanyConfiguration.SocialReason;

            /*Se debe especificar la clave del régimen fiscal del contribuyente emisor bajo
            el cual se está emitiendo el comprobante.
            Las claves de los diversos regímenes se encuentran incluidas en el catálogo
            c_RegimenFiscal publicado en el Portal del SAT.
            Ejemplo: En el caso de que el emisor sea una persona moral inscrita en el
            Régimen General de Ley de Personas Morales, debe registrar lo siguiente:
            RegimenFiscal= 601
            Aplica para tipo persona
            c_RegimenFiscal Descripción                                     Física Moral
            601             General de Ley Personas Morales                   No    Si
            603             Personas Morales con Fines no Lucrativos          No    Si
            621             Incorporación Fiscal                              Si    No
            */

            fiscalRegimesDictionary.TryGetValue(((int)createComprobanteParams.PayrollCompanyConfiguration.FiscalRegime).ToString(), out c_RegimenFiscal c_Regimen);
            comprobante.Emisor.RegimenFiscal = c_Regimen;

            //RECEPTOR
            /*Nodo: Receptor En este nodo se debe expresar la información del contribuyente receptor del
            comprobante (trabajador asalariado o asimilado a salarios). */
            comprobante.Receptor = new ComprobanteReceptor();

            /*Se debe registrar la Clave en el Registro Federal de Contribuyentes del
            receptor (persona física) del comprobante.
            La clave en el RFC debe estar contenida en la lista de RFC(I_RFC) inscritos no
            cancelados en el SAT.
            Debe ser de una persona física. La clave en el RFC debe ser correcta y
            corresponder a una persona efectivamente registrada en el SAT –esto se
            validará por el SAT o proveedor de certificación de CFDI-, por esto es muy
            importante validar las claves en el RFC de los trabajadores previamente a la
            generación del CFDI, ver la introducción del documento en dónde hay una
            liga directa a la herramienta SAT de validación.
            Nota: En caso de que el trabajador ya haya fallecido, se deberá registrar en
            este campo el RFC genérico XAXX010101000, debiendo registrar la CURP del
            trabajador fallecido en el campo “Curp” del Nodo: Receptor del
            Complemento de Nómina.
            Ejemplo:
                Persona física
                    Rfc= FIMA420127R44*/
            comprobante.Receptor.Rfc = createComprobanteParams.Overdraft.HistoricEmployee.Employee.RFC;

            /*Se debe registrar el nombre del contribuyente receptor del comprobante
            (trabajador asalariado o asimilado a salarios).
            Ejemplo:
            Persona física, se debe registrar:
            Nombre = Rafaeli Camposorio Ruízo
            */
            comprobante.Receptor.Nombre = createComprobanteParams.Overdraft.HistoricEmployee.Employee.FullName;

            /*Este campo no debe existir.*/
            comprobante.Receptor.ResidenciaFiscalSpecified = false;

            /*Este campo no debe existir*/
            comprobante.Receptor.NumRegIdTrib = null;

            /*UsoCFDI Se debe registrar la clave “P01” (Por definir) del catálogo c_UsoCFDI
            publicado en el Portal del SAT*/
            comprobante.Receptor.UsoCFDI = c_UsoCFDI.P01;

            //CONCEPTOS
            /*Nodo:
            Conceptos
            En este nodo se debe expresar el concepto descrito en el comprobante.
            Nodo: Concepto En este nodo se debe expresar la información detallada del servicio descrito
            en el comprobante. Se debe registrar la información de un solo concepto.
            */
            comprobante.Conceptos = new ComprobanteConcepto[1];

            /*ClaveProdServ Se debe registrar el valor “84111505”.
            Ejemplo:
                ClaveProdServ= 84111505
            */
            comprobante.Conceptos[0] = new ComprobanteConcepto();
            comprobante.Conceptos[0].ClaveProdServ = "84111505";

            //Este campo no debe existir
            comprobante.Conceptos[0].NoIdentificacion = null;

            /*Cantidad Se debe registrar el valor “1”.
            Ejemplo:
                Cantidad= 1 */
            comprobante.Conceptos[0].Cantidad = 1;

            //Se debe registrar el valor “ACT”.
            comprobante.Conceptos[0].ClaveUnidad = "ACT";

            //Este campo no debe existir.
            comprobante.Conceptos[0].Unidad = null;

            /*Descripcion Se debe registrar el valor “Pago de nómina”, este valor se debe registrar así,
            indistintamente de si trata de un trabajador asalariado o de un asimilado a
            salarios, toda vez que la información específica que denota si el comprobante
            corresponde a un asalariado o asimilado a salarios se precisa dentro del
            complemento de nómina en los campos TipoContrato y TipoRegimen. */
            comprobante.Conceptos[0].Descripcion = "Pago de nómina";

            //ValorUnitario Se debe registrar la suma de los campos TotalPercepciones más
            //TotalOtrosPagos del Complemento Nómina.

            comprobante.Conceptos[0].ValorUnitario = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalSalaryPayments + createComprobanteParams.OverdraftResults.TotalOtherPayments);

            /*Se debe registrar la suma de los campos TotalPercepciones más
            TotalOtrosPagos del Complemento Nómina.*/
            comprobante.Conceptos[0].Importe = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalSalaryPayments + createComprobanteParams.OverdraftResults.TotalOtherPayments);

            //Se debe registrar el valor del campoTotalDeducciones
            comprobante.Conceptos[0].Descuento = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalDeductionPayments);
            comprobante.Conceptos[0].DescuentoSpecified = true;

            //NOMINA
            /*Nodo:Nomina Complemento para el Comprobante Fiscal Digital por Internet (CFDI) para
            expresar la información que ampara conceptos de ingresos por salarios, la
            prestación de un servicio personal subordinado o conceptos asimilados a
            salarios.
            Este nodo se debe registrar como un nodo hijo del nodo Complemento en el
            CFDI.
            Siempre debe existir montos en los campos TotalPercepciones o
            TotalOtrosPagos o en ambos campos.
            El valor del campo Total del CFDI al que se le incorpora el complemento de
            nómina no puede ser negativo.*/

            var nomina = new Nomina();

            //Version Debe tener el valor "1.2" 
            nomina.Version = PAYROLL_VERSION;

            //TipoNomina 
            /*Este dato lo integra el sistema que utiliza el contribuyente para la emisión del
                comprobante fiscal.
                TipoNomina Se debe registrar la clave con la que se identifica el tipo de nómina.
                Las claves de los tipos de nóminas se encuentran incluidos en el catálogo
                c_TipoNomina publicado en el Portal del SAT.
                Ejemplo:
                    TipoNomina= O
                c_TipoNomina Descripción
                    O Nómina ordinaria
                E Nómina extraordinaria
                El tipo de nómina puede ser:
                Ordinaria o Extraordinaria, ésta clasificación la realiza el patrón al emitir el
                comprobante, comúnmente se suele clasificar como ordinaria a la nómina que
                paga conceptos de manera periódica y, por ende, a la que le corresponde una
                periodicidad determinada, por ejemplo: Diaria, Semanal, Catorcenal, Quincenal,
                Mensual, Bimestral, Decenal o incluso por unidad de obra, comisión o precio
                alzado.
                Como extraordinaria se clasifica a aquella nómina que incluye conceptos que no
                son objeto de pago de manera periódica o habitual, por ejemplo, pagos por
                separación, aguinaldos o bonos. */

            overdraftTypeDictionary.TryGetValue(createComprobanteParams.Overdraft.OverdraftType, out c_TipoNomina c_tipoNomina);
            nomina.TipoNomina = c_tipoNomina;

            //FechaPago
            /*Se debe registrar la fecha en que efectivamente el empleador realizó el pago
            (erogación) de la nómina al trabajador.
            Se expresa en la forma aaaa-mm-dd de acuerdo con la especificación ISO 8601.
            Ejemplo:
                FechaPago= 2017-05-15
            La fecha que se debe considerar conforme a la forma en que se realice el pago al trabajador es la siguiente:
            Forma de pago Descripción
               Efectivo Es la fecha en que el empleador realizó el pago (erogación) en efectivo al trabajador.
               Cheque Es la fecha de emisión del cheque que se genera para el pago por el empleador al trabajador.
               Transferencia electrónica de fondos
            Es la fecha en que el empleador ordenó a la institución de crédito realizar la transacción del pago
            a la tarjeta bancaria del trabajador.

            Fundamento Legal: Artículo 99 fracción III de la Ley del Impuesto sobre la
            Renta y regla 2.7.5.1. de la Resolución Miscelánea Fiscal vigente.*/
            //nomina.FechaPago = createComprobanteParams.PayrollStampingDetail.PaymentDate;
            nomina.FechaPago = createComprobanteParams.Overdraft.PeriodDetail.FinalDate;

            //FechaInicialPago
            /*Se debe registrar la fecha inicial del período de pago, debe de ser menor o igual
            a la FechaFinalPago.
            Para el caso de nóminas extraordinarias, se puede señalar como
            FechaInicialPago y FechaFinalPago la misma, es decir la del día en que se realice
            el pago al trabajador. 
            Se expresa en la forma aaaa-mm-dd de acuerdo con la especificación ISO 8601.
            Ejemplo:
                FechaInicialPago=2017-07-01
            
            Fundamento Legal: Artículo 99, fracción III de la Ley del Impuesto sobre la Renta*/
            var fechaInicialPago = createComprobanteParams.Overdraft.PeriodDetail.InitialDate;
            if (
                createComprobanteParams.Overdraft.HistoricEmployee.EntryDate > createComprobanteParams.Overdraft.PeriodDetail.InitialDate
                && createComprobanteParams.Overdraft.HistoricEmployee.EntryDate <= createComprobanteParams.Overdraft.PeriodDetail.FinalDate)
            {
                fechaInicialPago = createComprobanteParams.Overdraft.HistoricEmployee.EntryDate;
            }
            nomina.FechaInicialPago = fechaInicialPago;

            /*Se debe registrar la fecha final del período de pago, debe ser mayor o igual a la
            FechaInicialPago.
            Para el caso de nóminas extraordinarias como aquella en la que se paga la PTU,
            el aguinaldo, indemnización o pagos como resultado de la ejecución de un laudo,
            se puede señalar como FechaInicialPago y FechaFinalPago, la misma fecha, es
            decir del día en que se realice el pago al trabajador.
            Se expresa en la forma aaaa-mm-dd de acuerdo con la especificación ISO 8601.
            Ejemplo:
                FechaFinalPago= 2017-04-15

            Fundamento Legal: Artículo 99, fracción III de Ley del Impuesto sobre la Renta
            */
            DateTime? fechaFinalPago = createComprobanteParams.Overdraft.PeriodDetail.FinalDate;
            if (createComprobanteParams.Overdraft.HistoricEmployee.UnregisteredDate <= createComprobanteParams.Overdraft.PeriodDetail.FinalDate
                && createComprobanteParams.Overdraft.HistoricEmployee.UnregisteredDate >= createComprobanteParams.Overdraft.PeriodDetail.InitialDate)
            {
                fechaFinalPago = createComprobanteParams.Overdraft.HistoricEmployee.UnregisteredDate;
            }
            nomina.FechaFinalPago = fechaFinalPago.Value;

            /*Se debe registrar el número de días y/o la fracción de días pagados al trabajador.
            El valor debe ser mayor que cero, se pueden registrar hasta 36, 160 días y no se
            incluyen los ceros a la izquierda.
            Para el número de días pagados también se deben registrar en los casos en que
            se realicen pagos por ejemplo por PTU, indemnización o pagos como resultado
            de la ejecución de un laudo.
            Para el caso de los días pagados en los supuestos en dónde no se cuente con la
            posibilidad de incluir el detalle de los días que ampara el pago, se debe registrar
            el valor “1”.
            En el caso de fracción, se registran hasta tres decimales.
            Ejemplo: Se pagan 40 días por concepto de indemnización.
                NumDiasPagados= 40.000
            Ejemplo: Se pagan 5.5 días de aguinaldo.
                NumDiasPagados= 5.500
            
            Fundamento Legal: Artículo 99 fracción III de la Ley del Impuesto sobre la Renta.*/
            if (createComprobanteParams.OverdraftResults.WorkingDays > 0)
            {
                nomina.NumDiasPagados = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.WorkingDays, 3);
            }
            else
            {
                /* Para el caso de los días pagados en los supuestos en dónde no se cuente con la
                posibilidad de incluir el detalle de los días que ampara el pago, se debe registrar
                el valor “1”.*/
                nomina.NumDiasPagados = 1;
            }
            /*Es la suma de todas las percepciones que corresponden al trabajador, (Suma de
            los campos TotalSueldos más TotalSeparacionIndemnizacion más
            TotalJubilacionPensionRetiro) del nodo Percepciones.
            En el comprobante se pueden registrar percepciones y/u otros pagos. En el caso
            de que solo se registren otros pagos, este campo no debe existir dado que no hay
            datos de percepciones.
            Ejemplo:
                TotalPercepciones= 15000.00
            Nota: Es importante revisar que el TotalPercepciones coincida con la suma de
            TotalSueldos más TotalSeparacionIndemnizacion más
            TotalJubilcacionPensionRetiro de todas las percepciones que considere el
            comprobante.
            Fundamento Legal: Artículos 93 y 94 de la Ley del Impuesto sobre la Renta.*/
            nomina.TotalPercepciones = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalSalaryPayments);
            nomina.TotalPercepcionesSpecified = true;
            /*Es la suma de todas las deducciones (descuentos) aplicables al trabajador,
            incluyendo el Total Impuestos Retenidos (ISR), es decir, la suma de los campos
            TotalOtrasDeducciones más TotalImpuestosRetenidos, del Nodo Deducciones.
            En el comprobante pueden existir o no deducciones, en el caso de que no
            existan, este campo no debe existir.
            Ejemplo:
            TotalDeducciones= 4000.00
            Nota: Es importante revisar que el TotalDeducciones coincida con la suma de
            todas las deducciones (descuentos) que considere el comprobante.
            Fundamento Legal: Artículos 97, 110 y 132 de la Ley Federal del Trabajo.*/
            if (createComprobanteParams.OverdraftResults.TotalDeductionPayments > 0)
            {
                nomina.TotalDeducciones = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalDeductionPayments);
                nomina.TotalDeduccionesSpecified = true;
            }
            else
            {
                nomina.TotalDeduccionesSpecified = false;
            }

            /*Fundamento Legal: Artículos 97, 110 y 132 de la Ley Federal del Trabajo.
            TotalOtrosPagos Es la suma de los importes de la sección de “Otros Pagos” realizados al
            trabajador como son:
             El reintegro de ISR pagado en exceso (siempre que no haya sido
            enterado al SAT).
             Subsidio para el empleo (efectivamente entregado al trabajador).
             Viáticos (entregados al trabajador).
             Aplicación de saldo a favor por compensación anual.
             Reintegro de ISR retenido en exceso de ejercicio anterior (siempre que
            no haya sido enterado al SAT).
             Otros.
            Los citados pagos se registran como datos informativos y no se suman a las
            percepciones obtenidas por el trabajador, ya que se trata de pagos que no son
            ingresos acumulables para éste.             
            En caso de no existir información en la sección de OtrosPagos este campo no
            debe existir.
            Ejemplo:
            TotalOtrosPagos = 3000.00
            
            Fundamento Legal: Artículos 28, fracción V y 97 de la Ley del Impuesto sobre la
            Renta y Decreto del subsidio para el empleo, publicado en el Diario Oficial de la
            Federación el 11 de diciembre de 2013*/
            nomina.TotalOtrosPagos = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalOtherPayments);
            nomina.TotalOtrosPagosSpecified = true;

            //Nomina - Emisor
            nomina.Emisor = new NominaEmisor();

            /*Curp Se puede registrar la CURP del empleador (emisor) del comprobante de nómina
            cuando se trate de una persona física.
            En el caso de personas morales, éstas no cuentan con CURP, por lo tanto, no se
            debe registrar información en este campo.
            Ejemplo:
            Curp= CABL840215MDFSRS01
            */
            //Si es persona física es obligatorio el CURP
            if (createComprobanteParams.PayrollCompanyConfiguration.RFC.Length == 13)
            {
                nomina.Emisor.Curp = createComprobanteParams.PayrollCompanyConfiguration.CURP;
            }

            /*Se puede incorporar el registro patronal, clave de ramo - pagaduría o la que le
            asigne la institución de seguridad social al patrón. Se debe ingresar cuando se
            cuente con él o se esté obligado conforme a otras disposiciones aplicables.
            Puede conformarse desde 1 hasta 20 caracteres.
            Ejemplo:
                RegistroPatronal= B5510768108
            Por excepción, este dato no aplica cuando el empleador realice el pago a
            contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados
            en los artículos 12 y 13 de la Ley del Seguro Social, o bien no cuente con un registro
            asignado en términos de las disposiciones aplicables.
            Este dato debe existir cuando en el campo TipoContrato se haya registrado
            alguna de las siguientes claves: 
            01(Contrato de trabajo por tiempo indeterminado), 
            02 Contrato de trabajo para obra determinada, 
            03 (Contrato de trabajo por tiempo determinado), 
            04 (Contrato de trabajo por temporada),
            05(Contrato de trabajo sujeto a prueba), 
            06(Contrato de trabajo con capacitación inicial), 
            07(Modalidad de contratación por pago de hora laborada), 
            08 (Modalidad de trabajo por comisión laboral).

            Si en este campo se captura un Registro Patronal, también deberán registrarse
            obligatoriamente los siguientes campos: NumSeguridadSocial,
            FechaInicioRelLaboral, Antigüedad, RiesgoPuesto, y SalarioDiarioIntegrado, en
            caso contrario, no debe existir ninguno de los campos anteriormente señalados.
            Fundamento Legal: Artículo 15 Ley del Seguro Social.
            */

            if (!String.IsNullOrEmpty(createComprobanteParams.Overdraft.HistoricEmployee.EmployerRegistrationCode))
            {
                if (createComprobanteParams.OverdraftResults.TotalSeparationCompensation > 0 &&
                createComprobanteParams.Overdraft.OverdraftType == OverdraftType.CompensationSettlement)
                {
                    nomina.Emisor.RegistroPatronal = null;
                }
                else
                {
                    nomina.Emisor.RegistroPatronal = createComprobanteParams.Overdraft.HistoricEmployee.EmployerRegistrationCode;
                }
            }
            else
            {
                //Es obligatorio el proporcional un registro patronal para estos tipos de contrato
                if ((int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType == 1 ||
                    (int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType == 2 ||
                    (int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType == 3 ||
                    (int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType == 4 ||
                    (int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType == 5 ||
                    (int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType == 6 ||
                    (int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType == 7 ||
                    (int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType == 8
                    )
                {
                    var description = EnumHelper<ContractType>.GetDisplayDescription(createComprobanteParams.Overdraft.HistoricEmployee.ContractType);

                    throw new CotorraException(103, "103",
                        $"\nEs necesario proporcionar un registro patronal, ya que para el empleado " +
                        $"<strong>'{createComprobanteParams.Overdraft.HistoricEmployee.FullName}'</strong> su tipo de contrato es " +
                        $"<strong>'{description}'</strong> y por tanto es obligatorio", null);
                }
            }

            /*RfcPatronOrigen Se puede registrar la clave en el RFC de la persona que fungió como patrón
            cuando el pago al trabajador o ex trabajador se realice a través de un tercero
            como vehículo o herramienta de pago como es el caso de pagos de fondos de
            jubilación o pensión, pagados a través de fideicomisos, este campo puede ser
            utilizado por aquellos contribuyentes que deban cumplir obligaciones por
            cuenta de sus integrantes.
            El proveedor de certificación validará que en los sistemas del SAT exista el RFC
            del patrón origen como RFC inscrito y no cancelado.
            En el caso de instituciones fiduciarias que realicen pagos derivados de planes de
            pensiones y jubilaciones manejados a través de un fideicomiso mismo que 
            
            administran por instrucciones del fideicomitente (expatrón), deben señalar la
            clave en el RFC del expatrón.
            Este dato no aplica cuando el pago lo realice directamente el patrón o el pagador
            del ingreso asimilado, apareciendo el mismo como emisor del comprobante.
            Tampoco aplica para el caso de subcontratación o de tercerización de nómina.
            Ejemplo:
                RfcPatronOrigen= PAL7202161U0
            */
            if (!String.IsNullOrEmpty(createComprobanteParams.PayrollStampingDetail.RFCOriginEmployer))
            {
                nomina.Emisor.RfcPatronOrigen = createComprobanteParams.PayrollStampingDetail.RFCOriginEmployer;
            }

            /*Este nodo sólo aplica para las entidades federativas, municipios, así como sus respectivos 
             * organismos autónomos y entidades paraestatales y paramunicipales.  
            El proveedor de certificación validará que en los sistemas del SAT exista clave en el RFC del emisor 
            como RFC inscrito y no cancelado. En caso contrario este campo no debe existir. 
            c_Origen Recurso Descripción 
                IP Ingresos propios:   Son los ingresos pagados por Entidades  federativas, municipios o demarcaciones territoriales del Distrito Federal, 
                organismos autónomos y entidades paraestatales y paramunicipales con cargo a sus participaciones u otros ingresos locales. 
                IF Ingresos federales: Son los ingresos pagados por Entidades  federativas, municipios o demarcaciones territoriales del 
                Distrito Federal, organismos autónomos y entidades paraestatales y paramunicipales con recursos federales, distintos a las participaciones. 
                IM Ingresos mixtos:    Son los ingresos pagados por Entidades federativas, municipios o demarcaciones territoriales del Distrito Federal, 
                organismos autónomos y entidades paraestatales y paramunicipales con cargo a sus participaciones u otros ingresos locales 
                y con recursos federales distintos a las participaciones.  
             */
            if (createComprobanteParams.PayrollStampingDetail.SNCFEntity != null && createComprobanteParams.PayrollStampingDetail.SNCFEntity != default(SNCFEntity))
            {
                nomina.Emisor.EntidadSNCF = new NominaEmisorEntidadSNCF();
                nomina.Emisor.EntidadSNCF.MontoRecursoPropio = createComprobanteParams.PayrollStampingDetail.SNCFEntity.AmountOwnResource;
                nomina.Emisor.EntidadSNCF.MontoRecursoPropioSpecified = true;

                originResourceDictionary.TryGetValue(createComprobanteParams.PayrollStampingDetail.SNCFEntity.SNCFOriginResourceType, out c_OrigenRecurso c_OrigenRecurso);
                nomina.Emisor.EntidadSNCF.OrigenRecurso = c_OrigenRecurso;
            }

            //RECEPTOR
            nomina.Receptor = new NominaReceptor();
            /*
            En este nodo se debe expresar la información del contribuyente receptor (trabajador asalariado o asimilado a salarios) del comprobante. 
            Curp Se debe registrar la CURP del trabajador asalariado o asimilado a sueldos (receptor) del comprobante de nómina. 
            Las personas morales no pueden ser receptoras de un comprobante fiscal por concepto de nómina y por ende no son trabajadores, 
            ni cuentan con CURP. 
            En caso de trabajadores extranjeros que no cuenten con clave CURP, se deberán registrar las siguientes claves según corresponda, 
            XEXX010101HNEXXXA4 (Hombre) y XEXX010101MNEXXXA8 (Mujer).              */
            nomina.Receptor.Curp = createComprobanteParams.Overdraft.HistoricEmployee.Employee.CURP;

            /*Se puede registrar el número de seguridad social del trabajador otorgado por el instituto de seguridad social al cual se encuentra afiliado. Se debe ingresar cuando se esté obligado conforme a las disposiciones aplicables. 
             Puede conformarse desde 1 hasta 15 caracteres. 
             En relación con las diversas instituciones de seguridad social, es importante considerar lo contenido en el artículo 123, Apartado A y B de la Constitución Política de los Estados Unidos Mexicanos o bien la disposición legal específica aplicable o que en su caso corresponda a las demás instituciones respecto de este dato. 
             Ejemplo: NumSeguridadSocial= 123456789 
 
            Por excepción, este dato no aplica cuando el empleador realice el pago a contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados en los artículos 12 y 13 de la Ley del Seguro Social, o bien no cuente con un registro asignado en términos de las disposiciones aplicables. 
 
            Fundamento Legal: Artículos 15 Ley del Seguro Social y 9 de la Ley del ISSSTE.  
             */
            nomina.Receptor.NumSeguridadSocial = createComprobanteParams.Overdraft.HistoricEmployee.Employee.NSS;

            /*Se puede registrar la fecha de inicio de la relación laboral entre el empleador y el empleado. Se deben señalar los datos de la relación laboral y patrón vigente, es decir, contrato vigente. 
            Se debe registrar cuando se esté obligado conforme a las disposiciones aplicables. 
            Se expresa en la forma aaaa-mm-dd de acuerdo con la especificación ISO 8601. 
 
            Ejemplo:  
            FechaInicioRelLaboral= 2017-01-01 

            Por excepción, este dato no aplica cuando el empleador realice el pago a contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados en los artículos 12 y 13 de la Ley del Seguro Social, o bien no tenga la obligación de registrar este dato en términos de las disposiciones aplicables. 
             El valor de este dato deberá ser menor o igual que el campo FechaFinalPago. */
            nomina.Receptor.FechaInicioRelLaboral = dateTimeUtil.FixDateTime(createComprobanteParams.Overdraft.HistoricEmployee.EntryDate);
            nomina.Receptor.FechaInicioRelLaboralSpecified = true;

            /*Por excepción, este dato no aplica cuando el empleador realice el pago a contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados en los artículos 12 y 13 de la Ley del Seguro Social, o bien no tenga la obligación de registrar este dato en términos de las disposiciones aplicables. 
             El valor de este dato deberá ser menor o igual que el campo FechaFinalPago. 
 
            Antigüedad Se puede registrar el número de semanas o el periodo de años, meses y días (año calendario) en que el empleado ha mantenido relación laboral con el empleador. Se debe registrar cuando se esté obligado conforme a las disposiciones aplicables.  
             El valor de este campo deber ser menor o igual que el tiempo transcurrido entre la fecha de inicio de relación laboral y la fecha final de pago. 
             Por excepción, este dato no aplica cuando el empleador realice el pago a contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados en los artículos 12 y 13 de la Ley del Seguro Social, o bien no tenga la obligación de registrar este dato en términos de las disposiciones aplicables. 
 
            Ejemplo: Si un trabajador tiene una antigüedad de 10 años, 8 meses, 15 días, se debe registrar de la siguiente manera:  
            Antigüedad= P10Y8M15D 
 
            Ejemplo: Si un trabajador tiene una antigüedad de 0 años, 0 meses 20 días, se debe registrar de la siguiente manera:  
            Antigüedad= P20D 
 
            Ejemplo: 
            Si un trabajador tiene una antigüedad de 110 semanas, se debe registrar de la siguiente manera: 
             Antigüedad= P110W 
 
            Es importante mencionar que el registro se realiza conforme al año calendario. 
             Ejemplo: Si un trabajador tiene una antigüedad de un mes (febrero 2016).  
            Antigüedad=P29D 
 
            Si un trabajador tiene una antigüedad de un mes (febrero 2017).  
            Antigüedad=P28D 
 
            Fundamento Legal: Artículos 150 y 172 del Reglamento de la Ley del Impuesto sobre la Renta. */
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(nomina.Receptor.FechaInicioRelLaboral, createComprobanteParams.Overdraft.PeriodDetail.FinalDate);

            var antiquity = String.Empty;
            antiquity = $"P{calculateDateDifference.CalculateWeeks()}W";

            nomina.Receptor.Antigüedad = antiquity;

            /*
             TipoContrato Se debe registrar la clave del tipo de contrato laboral que tiene el trabajador con su empleador, en virtud del cual el trabajador se compromete a prestar sus servicios a cambio de una remuneración.  
             Las claves de los distintos tipos de contrato se encuentran incluidas en el catálogo c_TipoContrato publicado en el Portal del SAT. 
            Ejemplo:  
 
            TipoContrato= 01 
             c_TipoContrato Descripción 
            01 Contrato de trabajo por tiempo indeterminado. 

            Catálogo de tipos de contrato
                c_TipoContrato	Descripción
                01	Contrato de trabajo por tiempo indeterminado
                02	Contrato de trabajo para obra determinada
                03	Contrato de trabajo por tiempo determinado
                04	Contrato de trabajo por temporada
                05	Contrato de trabajo sujeto a prueba
                06	Contrato de trabajo con capacitación inicial
                07	Modalidad de contratación por pago de hora laborada
                08	Modalidad de trabajo por comisión laboral
                09	Modalidades de contratación donde no existe relación de trabajo
                10	Jubilación, pensión, retiro.
                99	Otro contrato
 
            Fundamento Legal: Título Segundo Relaciones Individuales de Trabajo, Capítulo II, Duración de las relaciones de Trabajo3 y artículo 35 de la Ley Federal del Trabajo. 
             */
            if (createComprobanteParams.OverdraftResults.TotalSeparationCompensation > 0 &&
                 createComprobanteParams.Overdraft.OverdraftType == OverdraftType.CompensationSettlement)
            {
                nomina.Receptor.TipoContrato = c_TipoContrato.Item99;
            }
            else
            {
                contractTypeDictionary.TryGetValue(createComprobanteParams.Overdraft.HistoricEmployee.ContractType, out c_TipoContrato c_TipoContrato);
                nomina.Receptor.TipoContrato = c_TipoContrato;
            }

            /*Sindicalizado Se debe registrar el valor “Sí”, únicamente cuando el trabajador este asociado a
            un sindicato dentro de la organización en la cual presta sus servicios.
            Se debe registrar el valor “No” cuando el empleador realice el pago a
            contribuyentes asimilados a salarios, o a asalariados no sindicalizados.
            */
            employeeTrustLevelDictionary.TryGetValue(createComprobanteParams.Overdraft.HistoricEmployee.EmployeeTrustLevel, out NominaReceptorSindicalizado nominaReceptorSindicalizado);
            nomina.Receptor.Sindicalizado = nominaReceptorSindicalizado;
            nomina.Receptor.SindicalizadoSpecified = true;
            /*
             TipoJornada Se puede registrar la clave correspondiente al tipo de jornada que cubre el
            trabajador durante el desempeño de las actividades encomendadas por su
            empleador. Se debe registrar cuando se esté obligado conforme a las
            disposiciones aplicables. 
            Servicio de Administración Tributaria │ Av. Hidalgo, núm. 77, col. Guerrero, delegación Cuauhtémoc, Ciudad de México,
            c. p. 06300 │ MarcaSAT: 627 22 728 desde la Ciudad de México │documento disponible en www.sat.gob.mx

            Las distintas claves de tipos de jornada se encuentran incluidas en el catálogo
            c_TipoJornada publicado en el Portal del SAT.
            Ejemplo:
            TipoJornada= 01
            c_TipoJornada Descripción
            01 Diurna
            Por excepción, este dato no aplica cuando el empleador realice el pago a
            contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados
            en los artículos 12 y 13 de la Ley del Seguro Social, o bien no tenga la obligación
            de registrar este dato en términos de las disposiciones aplicables.
            Fundamento Legal: Articulo 60 y 61 de la Ley Federal del Trabajo y 123, Apartado
            B), Fracción I de Ia Constitución Política de los Estados Unidos Mexicanos.
            c_TipoJornada	Descripción
                01	Diurna
                02	Nocturna
                03	Mixta
                04	Por hora
                05	Reducida
                06	Continuada
                07	Partida
                08	Por turnos
                99	OtraJornada
             */
            shiftWorkingDayTypeDictionary.TryGetValue(createComprobanteParams.Overdraft.HistoricEmployee.ShiftWorkingDayType, out c_TipoJornada c_tipoJornada);
            nomina.Receptor.TipoJornada = c_tipoJornada;
            nomina.Receptor.TipoJornadaSpecified = true;

            /*
             TipoRegimen Se debe registrar la clave del régimen por la cual el empleador tiene contratado
            al trabajador.
            Los distintos tipos de régimen se encuentran incluidos en el catálogo
            c_TipoRégimen publicado en el Portal del SAT.
            Ejemplo:
            TipoRegimen = 02
            c_TipoRegimen Descripción
            02              Sueldos
            03              Jubilados
            04              Pensionados
            09              Asimilados Honorarios
            13              Indemnización o Separación
            Si el campo TipoContrato tiene una clave entre los valores 01 y 08 del catálogo
            TipoContrato publicado en el Portal del SAT, entonces este campo deber ser 02,
            03 o 04.
            Si el campo TipoContrato tiene un valor 09 o superior, entonces este atributo
            debe contener algún valor del 05 hasta el 99.

            Los pagos realizados por indemnizaciones o separaciones deberán identificarse
            con la clave tipo regimen 13 (Indemnización o Separación), esto con la finalidad
            de distinguir correctamente este tipo de pago de aquellos pagos ordinarios de
            salarios.
            Fundamento Legal: Artículo 94 de la Ley del Impuesto sobre la Renta. */
            employeeRegimeTypeDictionary.TryGetValue(createComprobanteParams.Overdraft.HistoricEmployee.RegimeType, out c_TipoRegimen c_tipoRegimen);
            if (createComprobanteParams.OverdraftResults.TotalSeparationCompensation > 0 &&
                createComprobanteParams.Overdraft.OverdraftType == OverdraftType.CompensationSettlement)
            {
                //Separación / Indemnización
                c_tipoRegimen = c_TipoRegimen.Item13;
                nomina.Receptor.TipoRegimen = c_tipoRegimen;
            }
            else
            {
                nomina.Receptor.TipoRegimen = c_tipoRegimen;
            }

            /*NumEmpleado Se debe registrar el número interno que le asigna el empleador a cada uno de
            sus empleados para su pronta identificación, puede conformarse desde 1 hasta
            15 caracteres.
            Ejemplo:
            NumEmpleado= 120  */
            nomina.Receptor.NumEmpleado = createComprobanteParams.Overdraft.HistoricEmployee.Code;

            /*Departamento Se puede registrar el nombre del departamento o área a la que pertenece el
            trabajador a la que esta asignado, es decir, en donde desarrolla sus funciones. 
          
            En caso de laborar en distintos departamentos se registrará aquel en que haya
            desarrollado su labor por más tiempo en el período que ampara el comprobante,
            en caso de no ser posible determinar esto, se registrará el último departamento
            en que laboró en el período que ampara el comprobante.
            Ejemplo:
            Departamento= Cobranza */
            nomina.Receptor.Departamento = createComprobanteParams.Overdraft.HistoricEmployee.DepartmentDescription;

            /*Puesto Se puede registrar el nombre del puesto asignado al empleado o el nombre de
            la actividad que realiza.
            En caso de que durante el período que ampara el comprobante el trabajador
            haya cambiado de puesto se deberá consignar el último puesto ocupado.
            Ejemplo:
            Puesto= Velador */
            nomina.Receptor.Puesto = createComprobanteParams.Overdraft.HistoricEmployee.JobPositionDescription;

            /*RiesgoPuesto Se puede registrar la clave conforme a la clase en que esta inscrito el empleador,
            de acuerdo con las actividades que desempeñan sus trabajadores, según lo
            previsto en el artículo 196 del Reglamento en Materia de Afiliación Clasificación
            de Empresas, Recaudación y Fiscalización, o conforme con la Normatividad del
            Instituto de Seguridad Social del trabajador.
            Se debe registrar cuando se esté obligado conforme a las disposiciones
            aplicables.
            Las claves de las distintas clases de riesgos de puestos, se encuentran incluidas
            en el catálogo c_RiesgoPuesto publicado en el Portal del SAT.
            En caso de trabajadores que no se encuentren afiliados al IMSS, en este campo
            se deberá registrar la clave 99 “No aplica” del catálogo c_RiesgoPuesto.
                Ejemplo:
                RiesgoPuesto= 1
                c_RiesgoPuesto	Descripcion
                1	Clase I
                2	Clase II
                3	Clase III
                4	Clase IV
                5	Clase V
                99  No aplica
            Por excepción, este dato no aplica cuando el empleador realice el pago a
            contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados
            en los artículos 12 y 13 de la Ley del Seguro Social, o bien no tenga la obligación
            de registrar este dato en términos de las disposiciones aplicables.
            Fundamento Legal: Artículo 473 de la Ley Federal del Trabajo, 18 y 196 del
            Reglamento de la Ley del Seguro Social en Materia de Afiliación, Clasificación de
            Empresas, Recaudación y Fiscalización.
            */
            jobPositionRiskTypeDictionary.TryGetValue(createComprobanteParams.Overdraft.HistoricEmployee.JobPositionRiskType, out c_RiesgoPuesto c_riesgoPuesto);
            nomina.Receptor.RiesgoPuesto = c_riesgoPuesto;
            nomina.Receptor.RiesgoPuestoSpecified = true;

            /*
             PeriodicidadPago Se debe registrar la clave de periodicidad de pago en que se realiza el pago del
            salario al empleado o trabajador asimilado.
            En el caso de que en un mismo comprobante se incluya nómina ordinaria y un
            concepto extraordinario, por ejemplo para la última quincena se realiza el pago
            del sueldo más el finiquito, con periodicidad de pago como ordinaria, entonces
            en campo PeriodicidadPago se debe ingresar la clave “04” quincenal
            correspondiente al pago de nómina ordinaria.
            Las claves de periodicidad de pago se encuentran incluidas en el catálogo
            c_PeriodicidadPago publicado en el Portal del SAT.
            Ejemplo:
            PeriodicidadPago= 04

           c_PeriodicidadPago	Descripcion
            01	Diario
            02	Semanal
            03	Catorcenal
            04	Quincenal
            05	Mensual
            06	Bimestral
            07	Unidad_obra
            08	Comisión
            09	Precio_alzado
            99	OtraPeriodicidad
            Cuando se clasifique la nómina como ordinaria, pero en ella se incluyan claves de
            percepciones como pagos por separación o aguinaldo, se debe registrar como
            PeriodicidadPago alguna de las siguientes claves: 01(Diario), 02(Semanal),
            03(Catorcenal), 04(Quincenal), 05(Mensual), 06(Bimestral), 07(Unidad_obra),
            08(Comisión) 09(Precio_alzado) o 10 (Decenal), la que corresponda.
            Cuando el tipo de nómina sea ordinaria, la clave de periodicidad de pago debe
            ser distinta de la clave 99 (Otra Periodiciad) y si el tipo de nómina es
            extraordinaria, se debe registrar la clave 99 (Otra Periodicidad).
            Fundamento Legal: Artículo 25, fracción VII y 88 de la Ley Federal del Trabajo.
             */

            paymentPeriodicityDictionary.TryGetValue(createComprobanteParams.Overdraft.HistoricEmployee.PaymentPeriodicity, out c_PeriodicidadPago c_periodicidadPago);
            nomina.Receptor.PeriodicidadPago = c_periodicidadPago;

            /*
             Banco Se puede registrar la clave del banco en donde el empleador realiza el depósito
            de la nómina al trabajador o asimilado a salarios.
            Las claves de los distintos bancos se encuentran incluidas en el catálogo c_Banco
            publicado en el Portal del SAT.
            Ejemplo:
            Banco= 002
            c_Banco Descripción Nombre o razón social
            002 BANAMEX
            Banco Nacional de México, S.A., Institución de
            Banca Múltiple, Grupo Financiero Banamex
            Fundamento Legal: Artículo 101 de la Ley Federal del Trabajo.
             */
            if (null != createComprobanteParams.Overdraft.HistoricEmployee.BankCode)
            {
                var nameEnum = createComprobanteParams.Overdraft.HistoricEmployee.BankCode.ToString().PadLeft(3, '0');
                Enum.TryParse($"Item{nameEnum}", out c_Banco c_banco);
                nomina.Receptor.Banco = c_banco;
            }

            /*CuentaBancaria Se puede registrar el número de cuenta bancaria (11 posiciones), número de
            teléfono celular (10 posiciones), número de tarjeta de crédito, débito o de
            servicios (15 o 16 posiciones), la CLABE (18 posiciones), o número de monedero
            electrónico, en donde el empleador realiza el depósito de la nómina al trabajador.
            Ejemplo:
            CuentaBancaria = 002215911558451272
            Si el valor de este campo contiene una cuenta CLABE (18 posiciones), no debe
            existir el campo Banco, este dato será objeto de validación por el SAT o el
            proveedor de certificación de CFDI, se debe confirmar que el dígito de control es
            correcto.
            Si el valor de este campo contiene una cuenta de tarjeta de débito (16 posiciones)
            o una cuenta bancaria (11 posiciones) o un número de teléfono celular (10
            posiciones) debe de existir siempre el campo Banco.
            Fundamento Legal: Artículo 101 de la Ley Federal del Trabajo.
            */

            if (!String.IsNullOrEmpty(createComprobanteParams.Overdraft.HistoricEmployee.BankAccount))
            {
                if (createComprobanteParams.Overdraft.HistoricEmployee.BankAccount.Length == 18)
                {
                    nomina.Receptor.BancoSpecified = false;
                }
                else
                {
                    if (createComprobanteParams.Overdraft.HistoricEmployee.BankCode == null)
                    {
                        throw new CotorraException(20001, "20001", "El valor de la cuenta bancaria contiene una cuenta de tarjeta de débito (16 posiciones) o una " +
                            "cuenta bancaria(11 posiciones) o un número de teléfono celular(10 posiciones) debe de existir siempre el campo Banco." +
                            " Fundamento Legal: Artículo 101 de la Ley Federal del Trabajo.", null, null);
                    }

                    nomina.Receptor.BancoSpecified = true;
                }

                nomina.Receptor.CuentaBancaria = createComprobanteParams.Overdraft.HistoricEmployee.BankAccount;
            }
            else
            {
                //VALIDAR ESTA REGLA
                //throw new CotorraException(20002, "20002", $"Es necesario capturar la cuenta bancaria del empleado {overdraft.HistoricEmployee.Code}: {overdraft.HistoricEmployee.FullName}",
                //    null, null);
            }

            /*SalarioBaseCotApor Se puede registrar el importe de la retribución otorgada al trabajador, que se
            integra por los pagos hechos en efectivo por cuota diaria, gratificaciones,
            percepciones, alimentación, habitación, primas, comisiones, prestaciones en
            especie y cualquiera otra cantidad o prestación que se entregue al trabajador por
            su trabajo, sin considerar los conceptos que se excluyen de conformidad con el
            Artículo 27 de la Ley del Seguro Social, o la integración de los pagos conforme la
            normatividad del Instituto de Seguridad Social del trabajador. (Se emplea para
            pagar las cuotas y aportaciones de Seguridad Social).
            Se debe registrar cuando se esté obligado conforme a las disposiciones
            aplicables.
           
            Ejemplo:
            SalarioBaseCotApor= 490.22
            Por excepción, este dato no aplica cuando el empleador realice el pago a
            contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados
            en los artículos 12 y 13 de la Ley del Seguro Social, o bien no tenga la obligación
            de registrar este dato en términos de las disposiciones aplicables.
            Fundamento Legal: Artículo 27 de la Ley del Seguro Social.    */

            //TODO: No se si es el SBC correcto            
            //SBC = SBC Parte Fija + SBC Variable
            //TODO: 5 al 11 no   del 1 al 8 si
            //86.88 * 25 = 2172 a partir de feb 2020
            //Tipo de contrato asimilados no aplica (etc)
            if ((int)createComprobanteParams.Overdraft.HistoricEmployee.ContractType < 9)
            {
                nomina.Receptor.SalarioBaseCotApor = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.Overdraft.HistoricEmployee.SBCMax25UMA);
                nomina.Receptor.SalarioBaseCotAporSpecified = true;
            }
            else
            {
                nomina.Receptor.SalarioBaseCotAporSpecified = false;
            }
            /*SalarioDiarioIntegrado Se puede registrar el importe del salario que se integra con los pagos hechos en
            efectivo por cuota diaria, gratificaciones, percepciones, habitación, primas,
            comisiones, prestaciones en especie y cualquier otra cantidad o prestación que
            se entregue al trabajador por su trabajo, de conformidad con el Art. 84 de la Ley
            Federal del Trabajo. (Se utiliza para el cálculo de las indemnizaciones).
            Si se trata de relaciones laborales no sujetas a la Ley Federal del Trabajo, aquí se
            asentará el salario que sirva de base de cotización para el cálculo de
            indemnizaciones.
            Se debe registrar cuando se esté obligado conforme a las disposiciones
            aplicables.
            Ejemplo:
            SalarioDiarioIntegrado= 146.47
            Por excepción, este dato no aplica cuando el empleador realice el pago a
            contribuyentes asimilados a salarios, no se sitúe en los supuestos contemplados
            en los artículos 12 y 13 de la Ley del Seguro Social, o bien no tenga la obligación
            de registrar este dato en términos de las disposiciones aplicables.
            Fundamento Legal: Artículo 84 de la Ley Federal del Trabajo*/
            nomina.Receptor.SalarioDiarioIntegrado = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.Overdraft.HistoricEmployee.DailySalary);
            nomina.Receptor.SalarioDiarioIntegradoSpecified = true;

            /*ClaveEntFed Se debe registrar la clave de la entidad federativa en donde el trabajador prestó
            sus servicios al empleador.
            Si el trabajador prestó servicio en distintas entidades federativas durante el
            período que ampara el comprobante, se deberá incluir la clave de aquella
            entidad en dónde prestó la mayor parte del servicio. En caso de no ser posible
            identificar la entidad en que prestó la mayor cantidad del servicio, se podrá poner
            la clave de la última entidad en que los prestó.
            Las claves de las distintas entidades federativas se encuentran incluidas en el
            catálogo c_Estado publicado en el Portal del SAT.
            Ejemplo:
            ClaveEntFed= AGU
            c_Estado c_Pais Nombre del
            estado
            AGU MEX Aguascalientes*/

            var federalEntity = "Jalisco";
            if (!String.IsNullOrEmpty(createComprobanteParams.Overdraft.HistoricEmployee.EmployerRegistrationFederalEntity))
            {
                federalEntity = createComprobanteParams.Overdraft.HistoricEmployee.EmployerRegistrationFederalEntity;
            }
            else if (!String.IsNullOrEmpty(createComprobanteParams.PayrollCompanyConfiguration.Address?.FederalEntity))
            {
                federalEntity = createComprobanteParams.PayrollCompanyConfiguration.Address?.FederalEntity;
            }

            if (statesDictionary.TryGetValue(federalEntity, out c_Estado estado))
            {
                nomina.Receptor.ClaveEntFed = estado;
            }
            else
            {
                throw new CotorraException(60003, "60003", $"El estado especificado del empleado {createComprobanteParams.Overdraft.HistoricEmployee.FullName} no se encontró en el catálogo de estados del SAT.", null);
            }

            //PERCEPCIONES
            //TODO:Nodo:SubContratacion 
            /*Nodo:Percepciones     En este nodo se pueden expresar las percepciones aplicables. 
            La suma de los importes de los campos TotalSueldos más TotalSeparacionIndemnizacion más TotalJubilacionPensionRetiro 
            debe ser igual a la suma de los importes de los campos TotalGravado más TotalExento. 
            Al clasificar percepciones o deducciones como gravados o exentos, se debe tener cuidado en sólo clasificar como gravados 
            los conceptos que así lo sean en términos de las disposiciones legales aplicables. 
            Los premios por puntualidad no son ingresos exentos, son totalmente gravados. 

            TotalSueldos Es el total de las percepciones brutas (gravadas y exentas) por sueldos y salarios y conceptos asimilados a salarios. 
             Este dato debe ser igual a la suma de los campos ImporteGravado e ImporteExento donde la clave expresada en el campo 
             TipoPercepcion sea distinta de 
                "022" (Prima por Antigüedad), 
                "023" (Pagos por separación), 
                "025" (Indemnizaciones), 
                "039" (Jubilaciones, pensiones o haberes de retiro en una exhibición) y 
                "044" (Jubilaciones, pensiones o haberes de retiro en parcialidades). 
 
            Ejemplo:  TotalSueldos= 28000.00 
 
            Nota: Es importante revisar que el importe señalado en TotalSueldos sea efectivamente la suma de todas las percepciones 
            gravadas y exentas consideradas sueldos y salarios y conceptos asimilados a salarios que incluya el comprobante. 
             Fundamento Legal: Artículo 93, 94 y 95 de la Ley del Impuesto sobre la Renta
             */
            nomina.Percepciones = new NominaPercepciones();
            nomina.Percepciones.TotalSueldos = createComprobanteParams.RoundUtil
                .RoundValue(createComprobanteParams.OverdraftResults.TotalSalaryTotals);
            nomina.Percepciones.TotalSueldosSpecified = true;

            //Total Exento
            nomina.Percepciones.TotalExento = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalExempt);

            /*TotalSeparacionIndemnizacion Es la suma del importe exento y gravado de las claves tipo percepción “022” Prima por Antigüedad, “023” Pagos por separación y “025” Indemnizaciones. 
            Estas claves se encuentran incluidas en el catálogo c_TipoPercepción publicado en el Portal del SAT.  
 
            Ejemplo:  
                TotalSeparacionIndemnizacion= 60000.00 
 
            Nota: Es importante revisar que el importe señalado en TotalSeparacionIndemnizacion sea efectivamente la suma de 
            todas las percepciones gravadas y exentas consideradas en este atributo  que incluya el comprobante. 

            Fundamento Legal: Artículo 93, 94 y 95 de la Ley del Impuesto sobre la Renta, 172 del Reglamento de la Ley del Impuesto sobre la Renta. 
            */
            if (createComprobanteParams.OverdraftResults.TotalSeparationCompensation > 0)
            {
                nomina.Percepciones.TotalSeparacionIndemnizacion = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalSeparationCompensation);
                nomina.Percepciones.TotalSeparacionIndemnizacionSpecified = true;
            }

            /*
             TotalJubilacionPensionRetiro Es suma del importe exento y gravado de las claves tipo percepción “039” Jubilaciones, 
             pensiones o haberes de retiro en una exhibición y “044” Jubilaciones, pensiones o haberes de retiro en parcialidades. 
             Estas claves se encuentran incluidas en el catálogo c_TipoPercepción publicado en el Portal del SAT.  
 
            Ejemplo: 
             TotalJubilacionPensionRetiro= 30000.00 
             Nota: Es importante revisar que el importe señalado TotalJubilacionPensionRetiro en sea efectivamente la suma de todas las percepciones gravadas y exentas consideradas en este atributo que incluya el comprobante. 
 
            Fundamento Legal: Artículo 93, 94 y 95 de la Ley del Impuesto sobre la Renta, 171 y  173 del Reglamento de la Ley del Impuesto sobre la Renta. 
             */
            if (createComprobanteParams.OverdraftResults.TotalRetirementPensionWithdrawal > 0)
            {
                nomina.Percepciones.TotalJubilacionPensionRetiro = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalRetirementPensionWithdrawal);
                nomina.Percepciones.TotalJubilacionPensionRetiroSpecified = true;
            }

            /*
             * TotalGravado Es la suma de todas las percepciones gravadas que se relacionan en el comprobante. 
            Ejemplo: Si un trabajador obtuvo percepciones por concepto de sueldos y comisiones (por llegar a su meta de venta), 
            en este campo se debe considerar el total de las percepciones gravadas. 
 
            TotalGravado= 3180.51 
             TipoPercepcion= 001 Concepto= Sueldos y salarios  
            ImporteGravado=3030.51 

            TipoPercepcion= 028 
            Concepto= Comisiones 
            ImporteGravado=              150.00 
            Total Gravado=    3180.51                                                                                                             
 
            Fundamento Legal: Artículo 94 y 95 de la Ley del Impuesto sobre la Renta.  
             */

            nomina.Percepciones.TotalGravado = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalTaxed);

            //PERCEPCIONES -> PERCEPCION
            //En este nodo se debe expresar la información detallada de cada percepción. 
            var overdraftDetailSalaryPayment = createComprobanteParams.Overdraft.OverdraftDetails.Where(p => p.ConceptPayment.ConceptType == ConceptType.SalaryPayment).ToList();

            nomina.Percepciones.Percepcion = new NominaPercepcionesPercepcion[overdraftDetailSalaryPayment.Count];
            for (int i = 0; i < overdraftDetailSalaryPayment.Count; i++)
            {
                //Tipos de percepciones
                /*
                 c_TipoPercepcion	Descripcion
                    001	Sueldos, Salarios  Rayas y Jornales
                    002	Gratificación Anual (Aguinaldo)
                    003	Participación de los Trabajadores en las Utilidades PTU
                    004	Reembolso de Gastos Médicos Dentales y Hospitalarios
                    005	Fondo de Ahorro
                    006	Caja de ahorro
                    009	Contribuciones a Cargo del Trabajador Pagadas por el Patrón
                    010	Premios por puntualidad
                    011	Prima de Seguro de vida
                    012	Seguro de Gastos Médicos Mayores
                    013	Cuotas Sindicales Pagadas por el Patrón
                    014	Subsidios por incapacidad
                    015	Becas para trabajadores y/o hijos
                    016	Otros
                    019	Horas extra
                    020	Prima dominical
                    021	Prima vacacional
                    022	Prima por antigüedad
                    023	Pagos por separación
                    024	Seguro de retiro
                    025	Indemnizaciones
                    026	Reembolso por funeral
                    027	Cuotas de seguridad social pagadas por el patrón
                    028	Comisiones
                    029	Vales de despensa
                    030	Vales de restaurante
                    031	Vales de gasolina
                    032	Vales de ropa
                    033	Ayuda para renta
                    034	Ayuda para artículos escolares
                    035	Ayuda para anteojos
                    036	Ayuda para transporte
                    037	Ayuda para gastos de funeral
                    038	Otros ingresos por salarios
                    039	Jubilaciones, pensiones o haberes de retiro
                    044	Jubilaciones, pensiones o haberes de retiro en parcialidades
                    045	Ingresos en acciones o títulos valor que representan bienes
                     046	Ingresos asimilados a salarios
                 */

                //"NOM195","Message":"La suma de los importes de los atributos ImporteGravado e ImporteExento no es mayor que cero."
                if (createComprobanteParams.RoundUtil.RoundValue(overdraftDetailSalaryPayment[i].Taxed) + createComprobanteParams.RoundUtil.RoundValue(overdraftDetailSalaryPayment[i].Exempt) <= 0)
                {
                    continue;
                }

                nomina.Percepciones.Percepcion[i] = new NominaPercepcionesPercepcion();

                var salaryPaymentType = overdraftDetailSalaryPayment[i].ConceptPayment.SATGroupCode.Replace("P-", String.Empty).PadLeft(3, '0');
                Enum.TryParse($"Item{salaryPaymentType}", out c_TipoPercepcion c_percepcion);
                nomina.Percepciones.Percepcion[i].TipoPercepcion = c_percepcion;

                /*Clave Se debe registrar la clave de control interno que asigna el patrón a cada  percepción de nómina propia de su contabilidad, 
                 * puede conformarse desde 3 hasta 15 caracteres. 
                 Ejemplo:  
                 Clave= 00500 
 
                c_TipoPercepcion conforme al catálogo del SAT. 
                Descripción señalada conforme al catálogo del SAT. 
                Clave de percepción de nómina asignada internamente por el empleador en su contabilidad  correspondiente a la 
                percepción 001 del catálogo del SAT:  
                001 
                Sueldos, Salarios  Rayas y Jornales  
                00500 */
                nomina.Percepciones.Percepcion[i].Clave = overdraftDetailSalaryPayment[i].ConceptPayment.Code.ToString().PadLeft(3, '0');

                /*Concepto  Se debe registrar la descripción de cada uno de los conceptos de percepción. 
 
                Se ingresa el nombre o descripción específica que dé el patrón de cada uno de los  conceptos de percepción pagado 
                al trabador que corresponda, esta descripción puede o no coincidir con la descripción del catálogo c_ TipoPercepción publicado en el Portal del SAT. 
 
 
                Ejemplo: Concepto= Sueldos, Salarios Rayas y Jornales c_TipoPercepcion Descripción 
                001 Sueldos, Salarios Rayas y Jornales 
                029 Vales de despensa*/
                nomina.Percepciones.Percepcion[i].Concepto = overdraftDetailSalaryPayment[i].ConceptPayment.Description;

                /*ImporteGravado  Se debe registrar el importe gravado por cada concepto de percepción pagada al trabajador de conformidad 
                 * con las disposiciones fiscales aplicables. 
                 El importe gravado debe ser mayor a cero. 

                Ejemplo:  
                 ImporteGravado= 2598.80 
                 Fundamento Legal: Artículo 94 y 95 de la Ley del Impuesto sobre la Renta. */
                nomina.Percepciones.Percepcion[i].ImporteGravado = createComprobanteParams.RoundUtil.RoundValue(overdraftDetailSalaryPayment[i].Taxed);

                /*ImporteExento Se debe registrar el importe exento por cada concepto de percepción pagada al trabajador de conformidad 
                 * con las disposiciones fiscales aplicables. 
                 El importe exento  debe ser mayor a cero. 
                Ejemplo:  
 
                ImporteExento= 1301 
                Fundamento Legal: Artículo 93 de la Ley del Impuesto sobre la Renta. */
                nomina.Percepciones.Percepcion[i].ImporteExento = createComprobanteParams.RoundUtil.RoundValue(overdraftDetailSalaryPayment[i].Exempt);

                /*Nodo:AccionesOTitulos */

                /*Nodo Horas extras*/
                /*Nodo:HorasExtra En este nodo se pueden expresar las horas extras aplicables. En el caso de haber registrado 
                 * la clave de percepción “019” (Horas Extras) contenida en el catálogo c_TipoPercepción se debe registrar la información de éste nodo. 
 
                Dias Se debe registrar el número de días en que el trabajador laboró horas extra adicionales a su jornada normal de trabajo. 
                 */
                // “019” (Horas Extras) 
                //TODO: Calcular horas extras
                if (c_percepcion == c_TipoPercepcion.Item019)
                {
                    var calculateHE = CalculateHE(createComprobanteParams);
                    if (calculateHE.HETripleDays <= 0)
                    {
                        nomina.Percepciones.Percepcion[i].HorasExtra = new NominaPercepcionesPercepcionHorasExtra[1];

                        /*Se debe registrar el número de días en que el trabajador laboró horas extra adicionales a su jornada normal de trabajo. 
                          Ejemplo: 
                           Días= 1  
                           Fundamento Legal: Artículo 65, 66, 67 y 68 de la Ley Federal del Trabajo. */
                        var days = createComprobanteParams.Incidents.Where(p => p.IncidentType.Code == "HE").GroupBy(p => p.Date.Date);
                        nomina.Percepciones.Percepcion[i].HorasExtra[0] = new NominaPercepcionesPercepcionHorasExtra();
                        nomina.Percepciones.Percepcion[i].HorasExtra[0].Dias = days.Count();

                        /*TipoHoras Se debe registrar la clave del tipo de horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                        Las diferentes claves de tipos de horas extra se encuentran incluidos en el catálogo c_TipoHoras publicado en el Portal del SAT.  
                         Ejemplo:  
                         TipoHoras= 03  
                         c_TipoHoras Descripción 
                        01 Dobles 
                        02 Triples 
                        03 Simples 
                        Fundamento Legal: Artículo 65, 66, 67, 68 de la Ley Federal del Trabajo. */
                        //Todas las horas extras como dobles                  
                        var c_tipohoras = c_TipoHoras.Item01;
                        nomina.Percepciones.Percepcion[i].HorasExtra[0].TipoHoras = c_tipohoras;

                        /*HorasExtra  
                        Se debe registrar el número de horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                         Se deben considerar el número de horas extra completas y en caso de tener fracciones se deben redondear.  
                         Ejemplo:  HorasExtra= 3 

                         Fundamento Legal: Artículo 65, 66, 67 y 68 de la Ley Federal del Trabajo.  */
                        nomina.Percepciones.Percepcion[i].HorasExtra[0].HorasExtra = Convert.ToInt32(overdraftDetailSalaryPayment[i].Value);

                        /*
                         ImportePagado Se debe registrar el importe pagado por las horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                         Ejemplo: 
                        ImportePagado= 300.00 
                         Fundamento Legal: Artículo 93 fracción I de la Ley del Impuesto sobre la Renta, 67 de la Ley Federal del Trabajo. 
                         */
                        nomina.Percepciones.Percepcion[i].HorasExtra[0].ImportePagado = createComprobanteParams.RoundUtil.RoundValue(overdraftDetailSalaryPayment[i].Amount);
                    }
                    else
                    {
                        nomina.Percepciones.Percepcion[i].HorasExtra = new NominaPercepcionesPercepcionHorasExtra[2];

                        //DOUBLES //---------------------------------------------------------------
                        /*Se debe registrar el número de días en que el trabajador laboró horas extra adicionales a su jornada normal de trabajo. 
                          Ejemplo: 
                           Días= 1  
                           Fundamento Legal: Artículo 65, 66, 67 y 68 de la Ley Federal del Trabajo. */
                        nomina.Percepciones.Percepcion[i].HorasExtra[0] = new NominaPercepcionesPercepcionHorasExtra();
                        nomina.Percepciones.Percepcion[i].HorasExtra[0].Dias = calculateHE.HEDoubleDays;

                        /*TipoHoras Se debe registrar la clave del tipo de horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                        Las diferentes claves de tipos de horas extra se encuentran incluidos en el catálogo c_TipoHoras publicado en el Portal del SAT.  
                         Ejemplo:  
                         TipoHoras= 03  
                         c_TipoHoras Descripción 
                        01 Dobles 
                        02 Triples 
                        03 Simples 
                        Fundamento Legal: Artículo 65, 66, 67, 68 de la Ley Federal del Trabajo. */
                        //Todas las horas extras como dobles                  
                        var c_tipohoras = c_TipoHoras.Item01;
                        nomina.Percepciones.Percepcion[i].HorasExtra[0].TipoHoras = c_tipohoras;

                        /*HorasExtra  
                        Se debe registrar el número de horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                         Se deben considerar el número de horas extra completas y en caso de tener fracciones se deben redondear.  
                         Ejemplo:  HorasExtra= 3 

                         Fundamento Legal: Artículo 65, 66, 67 y 68 de la Ley Federal del Trabajo.  */
                        nomina.Percepciones.Percepcion[i].HorasExtra[0].HorasExtra = (int)calculateHE.HEDoubleHours;

                        /*
                         ImportePagado Se debe registrar el importe pagado por las horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                         Ejemplo: 
                        ImportePagado= 300.00 
                         Fundamento Legal: Artículo 93 fracción I de la Ley del Impuesto sobre la Renta, 67 de la Ley Federal del Trabajo. 
                         */
                        nomina.Percepciones.Percepcion[i].HorasExtra[0].ImportePagado = createComprobanteParams.RoundUtil.RoundValue(Convert.ToDecimal(calculateHE.HEDoublesAmount));

                        //Triples //---------------------------------
                        /*Se debe registrar el número de días en que el trabajador laboró horas extra adicionales a su jornada normal de trabajo. 
                         Ejemplo: 
                          Días= 1  
                          Fundamento Legal: Artículo 65, 66, 67 y 68 de la Ley Federal del Trabajo. */
                        nomina.Percepciones.Percepcion[i].HorasExtra[1] = new NominaPercepcionesPercepcionHorasExtra();
                        nomina.Percepciones.Percepcion[i].HorasExtra[1].Dias = calculateHE.HETripleDays;

                        /*TipoHoras Se debe registrar la clave del tipo de horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                        Las diferentes claves de tipos de horas extra se encuentran incluidos en el catálogo c_TipoHoras publicado en el Portal del SAT.  
                         Ejemplo:  
                         TipoHoras= 03  
                         c_TipoHoras Descripción 
                        01 Dobles 
                        02 Triples 
                        03 Simples 
                        Fundamento Legal: Artículo 65, 66, 67, 68 de la Ley Federal del Trabajo. */
                        //Todas las horas extras como dobles                  
                        var c_tipohoras2 = c_TipoHoras.Item02;
                        nomina.Percepciones.Percepcion[i].HorasExtra[1].TipoHoras = c_tipohoras2;

                        /*HorasExtra  
                        Se debe registrar el número de horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                         Se deben considerar el número de horas extra completas y en caso de tener fracciones se deben redondear.  
                         Ejemplo:  HorasExtra= 3 

                         Fundamento Legal: Artículo 65, 66, 67 y 68 de la Ley Federal del Trabajo.  */
                        nomina.Percepciones.Percepcion[i].HorasExtra[1].HorasExtra = (int)calculateHE.HETripleHours;

                        /*
                         ImportePagado Se debe registrar el importe pagado por las horas extra que laboró el trabajador adicionales a su jornada normal de trabajo. 
                         Ejemplo: 
                        ImportePagado= 300.00 
                         Fundamento Legal: Artículo 93 fracción I de la Ley del Impuesto sobre la Renta, 67 de la Ley Federal del Trabajo. 
                         */
                        nomina.Percepciones.Percepcion[i].HorasExtra[1].ImportePagado = createComprobanteParams.RoundUtil.RoundValue(Convert.ToDecimal(calculateHE.HETriplesAmount));
                    }


                }
            }


            /*Nodo:JubilacionPensionRetiro En este nodo se puede expresar la 
             * información detallada de pagos por 
             * jubilación, pensiones o haberes de retiro. */

            /*Nodo:SeparacionIndemnizacion */
            if (createComprobanteParams.OverdraftResults.TotalSeparationCompensation > 0)
            {
                var lastMonthSalary = 0.0M;
                var accumulatedIncome = 0.0M;

                //si el tipo de salario es fijo o mixto se toda el salario diario * 30, caso contrario el variable x 30
                if (createComprobanteParams.Overdraft.HistoricEmployee.ContributionBase == BaseQuotation.Variable)
                {
                    lastMonthSalary = createComprobanteParams.Overdraft.HistoricEmployee.SBCVariablePart * 30;
                }
                else
                {
                    lastMonthSalary = createComprobanteParams.Overdraft.HistoricEmployee.DailySalary * 30;
                }

                //Se toma el valor mínimo entre el total de gravado del finiquito vs el ultimosalario mensual ordinario
                if (lastMonthSalary > createComprobanteParams.OverdraftResults.TotalTaxedSettlement)
                {
                    accumulatedIncome = createComprobanteParams.OverdraftResults.TotalTaxedSettlement;
                }
                else
                {
                    accumulatedIncome = lastMonthSalary;
                }
                //El valor no acumulado es el restante del total menos el acumulado
                var nonAccumulatedIncome = createComprobanteParams.OverdraftResults.TotalSeparationCompensation - accumulatedIncome;

                //Años de servicio redondeo considerando el día de ingreso
                var totalDaysWorked = createComprobanteParams.Overdraft.PeriodDetail.PaymentDays;
                int yearsOfService = Convert.ToInt32(Math.Round(totalDaysWorked / 365, 0));

                nomina.Percepciones.SeparacionIndemnizacion = new NominaPercepcionesSeparacionIndemnizacion();
                nomina.Percepciones.SeparacionIndemnizacion.IngresoAcumulable = createComprobanteParams.RoundUtil.RoundValue(accumulatedIncome);
                nomina.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable = createComprobanteParams.RoundUtil.RoundValue(nonAccumulatedIncome);
                nomina.Percepciones.SeparacionIndemnizacion.TotalPagado = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalSeparationCompensation);
                nomina.Percepciones.SeparacionIndemnizacion.NumAñosServicio = yearsOfService;
                nomina.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd = createComprobanteParams.RoundUtil.RoundValue(lastMonthSalary);
            }

            //NODO: DEDUCCIONES En este nodo se deben expresar las deducciones aplicables. 

            //Si hay deducciones poner el elemento y si no no
            if (createComprobanteParams.OverdraftResults.TotalDeductionPayments > 0
                || createComprobanteParams.OverdraftResults.TotalOtherDeductions > 0)
            {
                nomina.Deducciones = new NominaDeducciones();
                /*TotalOtrasDeducciones 
                Se puede registrar el total de las deducciones (descuentos) aplicables al trabajador, sin considerar la clave de tipo deducción “002”(ISR).  

                Ejemplo: 
                 TotalOtrasDeducciones=15200.00*/
                nomina.Deducciones.TotalOtrasDeducciones = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalOtherDeductions);
                nomina.Deducciones.TotalOtrasDeduccionesSpecified = true;

                /*TotalImpuestosRetenidos Es la suma del impuesto sobre la renta retenido, es decir, donde la clave de tipo de deducción sea “002” (ISR). 
                 En caso de no existir la clave tipo deducción “002” (ISR), no se deberá registrar ningún dato en este campo. 

                Ejemplo:  
                 TotalImpuestosRetenidos=2500.00 

                Nota: Es importante revisar que no debe existir un importe mayor a cero en el ISR retenido (tipodeduccion 002) cuando el trabajador solo 
                percibió ingresos exentos para el periodo que corresponda al comprobante*/

                if (createComprobanteParams.OverdraftResults.TotalDeductionPaymentsISR > 0)
                {
                    nomina.Deducciones.TotalImpuestosRetenidos = createComprobanteParams.RoundUtil.RoundValue(createComprobanteParams.OverdraftResults.TotalDeductionPaymentsISR);
                    nomina.Deducciones.TotalImpuestosRetenidosSpecified = true;
                }
                else
                {
                    nomina.Deducciones.TotalImpuestosRetenidosSpecified = false;
                }

                //NODO: Deducción
                //Exclusión de Ajuste al neto y Subsidio al empleo
                /**/
                /**/
                var overdraftDetailDeductionPayment = createComprobanteParams.Overdraft.OverdraftDetails
                    .Where(p => p.ConceptPayment.ConceptType == ConceptType.DeductionPayment &&
                    p.ConceptPayment.Print && !p.ConceptPayment.Kind &&
                    !p.ConceptPayment.SATGroupCode.Contains("OP") && p.ConceptPayment.Code != 99).ToList();

                nomina.Deducciones.Deduccion = new NominaDeduccionesDeduccion[overdraftDetailDeductionPayment.Count];
                for (int i = 0; i < overdraftDetailDeductionPayment.Count; i++)
                {
                    //no puede haber ninguna deducción en 0
                    if (overdraftDetailDeductionPayment[i].Amount <= 0)
                    {
                        continue;
                    }

                    /*Se debe registrar la clave agrupadora que clasifica cada una de las deducciones (descuentos) del trabajador. 
                        Las diferentes claves de tipos de deducciones se encuentran incluidas en el catálogo c_TipoDeduccion publicado en el Portal del SAT.  

                        Ejemplo: Si a un trabajador se le descuenta de su sueldo un monto de $100.00 por concepto de seguridad social, se debe registrar de la siguiente forma: 

                         TipoDeduccion= 001 
                         c_TipoDeducción Descripción 
                        001 Seguridad social 
                        019 Cuotas sindicales 

                        Se puede registrar otro tipo de deducciones no consideradas en el citado catálogo, usando la clave tipo deducción “004” (Otros). 

                         Si se registró la clave "006"(Descuento por incapacidad) se debe expresar la información contenida en el nodo Incapacidad. 

                        La clave “101” (ISR Retenido de ejercicio anterior), se podrá utilizar para retenciones a cargo, establecidas en el artículo 97 de la LISR. 

                        Cuando se hayan  tenido errores en la utilización de las claves “065” (Ajuste en jubilaciones, pensiones o haberes de retiro en una sola exhibición exento), “066” (Ajuste en jubilaciones, pensiones o haberes de retiro en una sola exhibición gravado), “069” (Ajuste en jubilaciones, pensiones o haberes de retiro en parcialidades exento) y “070”(Ajuste en jubilaciones, pensiones o haberes de retiro en parcialidades gravado), se deberán  cancelar los comprobanes emitidos y volver a expedir con la información correcta. 
                         */

                    nomina.Deducciones.Deduccion[i] = new NominaDeduccionesDeduccion();

                    var salaryDeductionType = overdraftDetailDeductionPayment[i].ConceptPayment.SATGroupCode
                        .Replace("D-", String.Empty).ToString().PadLeft(3, '0');
                    Enum.TryParse($"Item{salaryDeductionType}", out c_TipoDeduccion c_tipoDeduccion);
                    nomina.Deducciones.Deduccion[i].TipoDeduccion = c_tipoDeduccion;

                    /*Se debe registrar la clave de control interno que asigna el patrón a cada deducción (descuento) de nómina propia de su contabilidad, puede conformarse desde 3 hasta 15 caracteres. 
                     Ejemplo:  
                        Clave= 00301 

                            c_TipoDeduccion conforme al catálogo del SAT. 
                            Descripción señalada conforme al catálogo del SAT. 
                            Clave de deducción de nómina asignada internamente por el empleador en su contabilidad  correspondiente a la deducción 001 del catálogo del SAT:  
                            001 Seguridad Social 

                            00301  */
                    nomina.Deducciones.Deduccion[i].Clave = overdraftDetailDeductionPayment[i].ConceptPayment.Code.ToString().PadLeft(3, '0');

                    /*Se debe registrar la descripción de cada uno de los conceptos de deducción. 

                        Se ingresa el nombre o descripción específica que dé el patrón de cada uno de los  conceptos de deducción (descuentos) realizados al trabador que corresponda, esta descripción puede o no coincidir con la descripción del catálogo tipo deducción. 

                         Ejemplo: Si a un trabajador se le realiza un descuento por concepto de seguridad social, se debe registrar en este campo la descripción de la deducción (descuento). 
                         Concepto= Seguridad social                        c_TipoDeducción Descripción 
                            001 Seguridad social 
                            019 Cuotas sindicales 

                        Nota: Es importante considerar que aunque la descripción no coincida textualmente con la descripción del catálogo c_Tipo deducción 
                        publicado en el Portal del SAT, se debe cuidar que el concepto utilizado si tenga relación y sea concordante con la descripción 
                        de dicho catálogo de la clave que corresponda.*/
                    nomina.Deducciones.Deduccion[i].Concepto = overdraftDetailDeductionPayment[i].ConceptPayment.Description;

                    /*Importe Se debe registrar el importe de un concepto de deducción (descuento) y debe ser mayor que cero. 
                    En el caso de que se agreguen uno o más nodos Incapacidad, la suma de los campos Importe Monetario de estos nodos, debe ser igual al monto de este campo. 

                    Ejemplo:  
                     Importe= 200.00 */
                    nomina.Deducciones.Deduccion[i].Importe = createComprobanteParams.RoundUtil.RoundValue(overdraftDetailDeductionPayment[i].Amount);
                }
            }

            /*Nodo:OtrosPagos En este nodo se debe expresar otros pagos aplicables. */
            //Nodo:OtroPago

            //Ajuste al neto
            //Subs al empleo 
            var overdraftDetailOtherPayments = new OverdraftManager().GetOtherPayments(createComprobanteParams.Overdraft).DistinctBy(p => p.ConceptPaymentID).ToList();

            //Fix para cuando no se manda el subsidio al empleo - solo si es regimen 2
            var subsidio = false;
            if (!overdraftDetailOtherPayments.Any(p => p.ConceptPayment.Code == 39)
                && c_tipoRegimen == c_TipoRegimen.Item02)
            {
                overdraftDetailOtherPayments.Add(new OverdraftDetail()
                {
                    Amount = (0.00M).ToCustomRound(createComprobanteParams.RoundUtil),
                    Value = (0.00M).ToCustomRound(createComprobanteParams.RoundUtil),
                    ConceptPayment = new ConceptPayment()
                    {
                        ConceptType = ConceptType.DeductionPayment,
                        Code = 39,
                        Description = "Subs al Empleo mes",
                        Name = "Subs al Empleo mes",
                        SATGroupCode = "OP-002"
                    }
                });
                subsidio = true;
            }
            else if (c_tipoRegimen == c_TipoRegimen.Item02)
            {
                subsidio = true;
            }

            var nominasOtrosPagos = new List<NominaOtroPago>();

            for (int i = 0; i < overdraftDetailOtherPayments.Count; i++)
            {
                var nominaOtroPago = new NominaOtroPago();

                //si es ajuste al neto pero no tiene importe no hay que ponerlo
                //si aplica subsidio por el tipo de regimen pero tiene un atributo 007 u 008 no lo reporta
                if (overdraftDetailOtherPayments[i].Amount <= 0 && overdraftDetailOtherPayments[i].ConceptPayment.Code != 39)
                {
                    continue;
                }

                //4 es Codigo SAT otras deducciones
                if (overdraftDetailOtherPayments[i].ConceptPayment.SATGroupCode.ToString() == "D-004")
                {
                    //Otros pagos SAT 
                    c_TipoOtroPago c_tipoOtroPago = c_TipoOtroPago.Item999;
                    nominaOtroPago.TipoOtroPago = c_tipoOtroPago;
                }
                else
                {
                    var nameEnum = overdraftDetailOtherPayments[i].ConceptPayment.SATGroupCode
                        .Replace("OP-", String.Empty)
                        .Replace("P-", String.Empty)
                        .Replace("D-", String.Empty)
                        .Replace("OP-", String.Empty)
                        .PadLeft(3, '0');
                    Enum.TryParse($"Item{nameEnum}", out c_TipoOtroPago c_tipoOtroPago);
                    nominaOtroPago.TipoOtroPago = c_tipoOtroPago;
                }

                nominaOtroPago.Clave = overdraftDetailOtherPayments[i].ConceptPayment.Code.ToString().PadLeft(3, '0');
                nominaOtroPago.Importe = createComprobanteParams.RoundUtil.RoundValue(overdraftDetailOtherPayments[i].Amount);
                nominaOtroPago.Concepto = overdraftDetailOtherPayments[i].ConceptPayment.Description;

                //Subsidio al empleo
                if (overdraftDetailOtherPayments[i].ConceptPayment.SATGroupCode == "OP-002")
                {
                    nominaOtroPago.SubsidioAlEmpleo = new NominaOtroPagoSubsidioAlEmpleo();
                    nominaOtroPago.SubsidioAlEmpleo.SubsidioCausado = createComprobanteParams.RoundUtil.RoundValue(overdraftDetailOtherPayments[i].Amount);
                }

                nominasOtrosPagos.Add(nominaOtroPago);
            }
            //Otros pagos
            if (nominasOtrosPagos.Any())
            {
                nomina.OtrosPagos = nominasOtrosPagos.ToArray();
            }
            else
            {
                nomina.TotalOtrosPagosSpecified = false;
            }

            /*Nodo:Incapacidades En este nodo se puede expresar la información de las 
             * incapacidades. 
                Nodo:Incapacidad En este nodo se debe expresar la información 
                    de las incapacidades. 

                DiasIncapacidad:  
                    Se debe registrar el número de días enteros que el trabajador se incapacitó en el periodo. 
                 Ejemplo: 
                 DiasIncapacidad= 7 
 
                TipoIncapacidad: 
                    Se debe registrar la clave del tipo de incapacidad que le fue otorgada al trabajador. 
                Las diferentes claves de tipos de incapacidad se encuentran incluidas en el catálogo c_TipoIncapacidad publicado en el Portal del SAT. 
 
                Ejemplo: 
                Si una trabajadora se encuentra con incapacidad por maternidad, 
                    en este campo se deberá registrar la clave “03”. 
                 TipoIncapacidad= 03 
 
                c_TipoIncapacidad Descripción 
                03 Maternidad 
 
                ImporteMonetario  Se puede registrar el monto del importe monetario de la incapacidad. 
                 Ejemplo:  
                 ImporteMonetario= 1100.00 
 
                Fundamento Legal: Artículo 58 de la Ley del Seguro Social. */
            if (createComprobanteParams.Inhabilities.Any(p => p.EmployeeID == createComprobanteParams.Overdraft.EmployeeID))
            {
                List<NominaIncapacidad> nominaIncapacidades = new List<NominaIncapacidad>();
                createComprobanteParams.Inhabilities
                    .Where(p => p.EmployeeID == createComprobanteParams.Overdraft.EmployeeID)
                    .ForEach(p =>
                {
                    inhabilitTypeDictionary.TryGetValue(p.CategoryInsurance,
                        out c_TipoIncapacidad tipoIncapacidad);

                    //Días donde intersectan las incapacidades y los días del periodo
                    var daysOfInhabilityOnPeriod = new DateTimeUtil().InclusiveDays(
                        p.InitialDate, p.FinalDate,
                        createComprobanteParams.Overdraft.PeriodDetail.InitialDate,
                        createComprobanteParams.Overdraft.PeriodDetail.FinalDate);
                    if (daysOfInhabilityOnPeriod > 0)
                    {
                        //Buscar concepto P-014 (Incapacidades pagadas por la empresa)
                        var sumP014 = createComprobanteParams.Overdraft.OverdraftDetails
                                .Where(q => q.ConceptPayment.SATGroupCode == "P-014")
                                .Sum(q => q.Taxed + q.Exempt);

                        var nominaIncapacidad = new NominaIncapacidad
                        {
                            DiasIncapacidad = daysOfInhabilityOnPeriod,
                            TipoIncapacidad = tipoIncapacidad
                        };

                        if (sumP014 > 0)
                        {
                            nominaIncapacidad.ImporteMonetario = createComprobanteParams.RoundUtil.RoundValue(sumP014, 2);
                            nominaIncapacidad.ImporteMonetarioSpecified = true;
                        }
                        else
                        {
                            nominaIncapacidad.ImporteMonetarioSpecified = false;
                        }
                        nominaIncapacidades.Add(nominaIncapacidad);
                    }
                });
                if (nominaIncapacidades.Any())
                {
                    nomina.Incapacidades = nominaIncapacidades.ToArray();
                }
            }

            //*-********************************------------------------------------------
            //add complemento nomina
            comprobante.Complemento = new ComprobanteComplemento[1];
            comprobante.Complemento[0] = new ComprobanteComplemento();
            comprobante.Complemento[0].Any = new XmlElement[1] { GetElement(CreateXmlNom(nomina)) };

            return comprobante;

        }


        /// <summary>
        /// Stamp Document
        /// </summary>
        /// <param name="signDocumentResult"></param>
        /// <returns></returns>
        public override async Task<SignDocumentResult<ICFDINomProvider>> StampDocumetAsync(SignDocumentResult<ICFDINomProvider> signDocumentResult)
        {
            var xml = base.CreateXml<ICFDINomProvider>(signDocumentResult.CFDI);
            IPACProvider pACProvider = FactoryPACProvider.CreateInstanceFromConfig();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            signDocumentResult = await pACProvider.StampDocumetAsync(signDocumentResult, _fiscalStampingVersion, xml);
            stopwatch.Stop();
            var message = $"El tiempo transcurrido en timbrar fue de {stopwatch.Elapsed.ToString()} con el proveedor {pACProvider.GetType().ToString()}";
            AppTrace.Information(message);

            if (!signDocumentResult.WithErrors && !String.IsNullOrEmpty(signDocumentResult.TFD))
            {
                //Get TFD Object
                var tfd = SerializerXml.DeserializeObject<TimbreFiscalDigital>($"{signDocumentResult.TFD}");

                //Add TFD to CFDI
                var complementos = (signDocumentResult.CFDI as Comprobante).Complemento.ToList();
                var listElementos = complementos[0].Any.ToList();
                listElementos.Add(GetElement(CreateXmlTFD(tfd)));
                complementos[0].Any = listElementos.ToArray();
                (signDocumentResult.CFDI as Comprobante).Complemento = complementos.ToArray();

                //Set UUID to return
                signDocumentResult.UUID = Guid.Parse(tfd.UUID);
            }
            else
            {
                AppTrace.Error(signDocumentResult.Details);
            }

            return signDocumentResult;
        }
    }
}

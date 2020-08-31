using Cotorra.Core.Managers.Calculation.Functions;
using Cotorra.Core.Managers.Calculation.Functions.Catalogs;
using Cotorra.Core.Managers.Calculation.Functions.Catalogs.Deduction;
using Cotorra.Core.Managers.Calculation.Functions.Catalogs.Piecework;
using Cotorra.Core.Managers.Calculation.Functions.ExtraHours;
using Cotorra.Core.Managers.Calculation.Functions.FONACOT;
using Cotorra.Core.Managers.Calculation.Functions.IMSS;
using Cotorra.Core.Managers.Calculation.Functions.Incidents;
using Cotorra.Core.Managers.Calculation.Functions.INFONAVIT;
using Cotorra.Core.Managers.Calculation.Functions.PDO;
using Cotorra.Core.Managers.Calculation.Functions.Perceptions;
using Cotorra.Core.Managers.Calculation.Functions.Programatic;
using Cotorra.Core.Managers.Calculation.Functions.Settlement;
using Cotorra.Core.Managers.Calculation.Functions.SeventhDays;
using Cotorra.Core.Managers.Calculation.Functions.Values;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.Calculation
{
    public class FormulaManager : IFormulaManager
    {
        #region "Attributes"
        private readonly Dictionary<string, string> reserverdWords;
        private List<PrimitiveElement> _primitiveElements;
        #endregion

        #region "Properties"
        public FunctionParams FunctionParamsProperty { get; set; }
        #endregion

        #region "Constructor"
        public FormulaManager(FunctionParams functionParams)
        {
            reserverdWords = new Dictionary<string, string>();
            reserverdWords.Add("Invalidez y Vida", "Invalidez_y_Vida");
            reserverdWords.Add("Cesantia y Vejez", "Cesantia_y_Vejez");
            reserverdWords.Add("Enf. y Mat. Patron", "Enf_y_Mat_Patron");
            reserverdWords.Add("2% Fondo retiro SAR (8)", "Dos_Porciento_Fondo_retiro_SAR_8");
            reserverdWords.Add("2% Impuesto estatal", "Dos_Porciento_Impuesto_estatal");
            reserverdWords.Add("Riesgo de trabajo (9)", "Riesgo_de_trabajo_9");
            reserverdWords.Add("1% Educación empresa", "Uno_Porciento_Educacion_empresa");
            reserverdWords.Add("I.M.S.S. empresa", "IMSS_empresa");
            reserverdWords.Add("Infonavit empresa", "Infonavit_empresa");
            reserverdWords.Add("Guarderia I.M.S.S. (7)", "Guarderia_IMSS_7");
            reserverdWords.Add("Séptimo día", "Septimo_dia");
            reserverdWords.Add("Horas extras", "Horas_extras");
            reserverdWords.Add("I.S.R. (mes)", "ISR_mes");
            reserverdWords.Add("Subs al Empleo (mes)", "Subs_al_Empleo_mes");
            reserverdWords.Add("Ajuste al neto", "Ajuste_al_neto");
            reserverdWords.Add("ISR de la Tarifa", "ISR_de_la_Tarifa");
            FunctionParamsProperty = functionParams;

            //Initialize primite elements, functions, variables, etc.
            Initializate();
        }
        #endregion

        private string replaceIfItsReservedWord(string complexWord)
        {
            if (complexWord != null)
            {
                var resultWord = complexWord;
                var keys = reserverdWords.Keys.AsParallel().ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    if (complexWord.Contains(keys[i]))
                    {
                        reserverdWords.TryGetValue(keys[i], out string value);
                        resultWord = resultWord.Replace(keys[i], value);
                    }
                }
                return resultWord;
            }
            return String.Empty;
        }

        #region "Prepare Functions and Arguments"
        private void PrepareArgumentWithFormulaFunction(string argumentName, string formula,
            ref List<PrimitiveElement> primitiveElements)
        {
            Function genericFuncion = new Function($"{FixFormula(argumentName)}Function", FixFormula(formula),
                primitiveElements.ToArray());
            primitiveElements.Add(new Argument(FixFormula(argumentName), true, genericFuncion));
        }

        private void PrepareFunction<T>(string functionName,
            ref List<PrimitiveElement> primitiveElements) where T : FunctionExtension
        {
            var instance = Activator.CreateInstance(typeof(T), FunctionParamsProperty);
            primitiveElements.Add(new Function(FixFormula(functionName), instance as FunctionExtension));
        }

        private void PrepareArgument<T>(string argumentName,
            ref List<PrimitiveElement> primitiveElements) where T : FunctionExtension
        {
            var type = typeof(T);
            var instance = Activator.CreateInstance(type, FunctionParamsProperty);
            primitiveElements.Add(new Argument(FixFormula(argumentName), true, (instance as FunctionExtension)));
        }
        #endregion

        private void ProgramaticFunctions(ref List<PrimitiveElement> primitiveElements)
        {
            //Functions Programatic
            PrepareFunction<MINFunction>("MIN", ref primitiveElements);
            PrepareFunction<MAXFunction>("MAX", ref primitiveElements);
            PrepareFunction<FRACFunction>("FRAC", ref primitiveElements);
            PrepareFunction<RoundToFunction>("ROUNDTO", ref primitiveElements);
            PrepareFunction<INTFunction>("INT", ref primitiveElements);
        }

        /// <summary>
        /// Static functions
        /// </summary>
        /// <param name="functionParams"></param>
        private void Functions(ref List<PrimitiveElement> primitiveElements)
        {
            //Functions
            PrepareFunction<HorasPorTurnoFunction>("HorasPorTurno", ref primitiveElements);
            PrepareFunction<DiasDescansoVacPeriodoCompletoFunction>("DiasDescansoVacPeriodoCompleto", ref primitiveElements);
            PrepareFunction<SalDiarioAntFunction>("SalCuotaDiariaAnt", ref primitiveElements);
            PrepareFunction<IncidenciaSinDerechoASueldoFunction>("IncidenciaSinDerechoASueldo", ref primitiveElements);
            PrepareFunction<TipoProcesoFunction>("TipoProceso", ref primitiveElements);
            PrepareFunction<SeptimosvacPerCompletoFunction>("SeptimosvacPerCompleto", ref primitiveElements);
            PrepareFunction<DiasLFTSalarioVigenteFunction>("DiasLFTSalarioVigente", ref primitiveElements);
            PrepareFunction<DiasLFTSalarioAnteriorFunction>("DiasLFTSalarioAnterior", ref primitiveElements);
            PrepareFunction<DiasLFTSinSalarioVigenteFunction>("DiasLFTSinSalarioVigente", ref primitiveElements);
            PrepareFunction<DiasLFTSinSalarioAnteriorFunction>("DiasLFTSinSalarioAnterior", ref primitiveElements);
            PrepareFunction<DiasLFTSeptimosAnteriorFunction>("DiasLFTSeptimosAnterior", ref primitiveElements);
            PrepareFunction<DiasLFTSeptimosVigenteFunction>("DiasLFTSeptimosVigente", ref primitiveElements);
            PrepareFunction<PeriodoDeVacacionesFunction>("PeriodoEnVacaciones", ref primitiveElements);
            PrepareFunction<SalariosMinimos_ZonaA>("SalariosMinimos.Zona_A", ref primitiveElements);
            PrepareFunction<SalariosMinimos_ZonaB>("SalariosMinimos.Zona_B", ref primitiveElements);
            PrepareFunction<SalariosMinimos_ZonaC>("SalariosMinimos.Zona_C", ref primitiveElements);
            PrepareFunction<UMA_ValorUMAFunction>("UMA.Valor_UMA", ref primitiveElements);
            PrepareFunction<FactorDescINFONAVITFunction>("FactorDescINFONAVIT.Factor_Descuento", ref primitiveElements);
            PrepareFunction<DiasIMSSAusenciasVigenteFunction>("vDiasAus", ref primitiveElements);
            PrepareFunction<DiasIMSSIncapacidadesVigenteFunction>("vDiasInc", ref primitiveElements);
            PrepareFunction<DiasDescansoVacFunction>("DiasDescansoTarjeta", ref primitiveElements);
            PrepareFunction<DiasTranscurridosMesFunction>("DiasTranscurridosMes", ref primitiveElements);

            //ISR
            PrepareFunction<TVigISRMensual_Limite_inferiorFunction>("TVigISRMensual.Limite_inferior", ref primitiveElements);
            PrepareFunction<TVigISRMensual_PorcentajeFunction>("TVigISRMensual.Porcentaje", ref primitiveElements);
            PrepareFunction<TVigISRMensual_Cuota_fijaFunction>("TVigISRMensual.Cuota_fija", ref primitiveElements);

            //Subsidio al Empleo
            PrepareFunction<TVigSubEmpMensual_Subs_al_empleoFunction>("TVigSubEmpMensual.Subs_al_empleo", ref primitiveElements);

            //Topes SGDF
            PrepareFunction<TopesSGDF_EG_Especie_GastosMedicos_1Function>("TopesSGDF.EG_Especie_GastosMedicos_1", ref primitiveElements);
            PrepareFunction<TopesSGDF_EG_Especie_Fija_2Function>("TopesSGDF.EG_Especie_Fija_2", ref primitiveElements);
            PrepareFunction<TopesSGDF_EG_Especie_mas_3SMDF_3Function>("TopesSGDF.EG_Especie_mas_3SMDF_3", ref primitiveElements);
            PrepareFunction<TopesSGDF_EG_Prestaciones_en_Dinero_4>("TopesSGDF.EG_Prestaciones_en_Dinero_4", ref primitiveElements);
            PrepareFunction<TopesSGDF_Invalidez_y_vida_5Function>("TopesSGDF.Invalidez_y_vida_5", ref primitiveElements);
            PrepareFunction<TopesSGDF_Cesantia_y_vejez_6Function>("TopesSGDF.Cesantia_y_vejez_6", ref primitiveElements);
            PrepareFunction<TopesSGDF_Guarderias_7Function>("TopesSGDF.Guarderias_7", ref primitiveElements);
            PrepareFunction<TopesSGDF_Retiro_8Function>("TopesSGDF.Retiro_8", ref primitiveElements);
            PrepareFunction<TopesSGDF_RiesgodeTrabajo_9Function>("TopesSGDF.RiesgodeTrabajo_9", ref primitiveElements);

            //Tabla IMSSPatron
            PrepareFunction<TablaIMSSPatron_EG_Especie_GastosMedicos_1Function>("TablaIMSSPatron.EG_Especie_GastosMedicos_1", ref primitiveElements);
            PrepareFunction<TablaIMSSPatron_EG_Especie_Fija_2Function>("TablaIMSSPatron.EG_Especie_Fija_2", ref primitiveElements);
            PrepareFunction<TablaIMSSPatron_EG_Especie_mas_3SMDF_3Function>("TablaIMSSPatron.EG_Especie_mas_3SMDF_3", ref primitiveElements);
            PrepareFunction<TablaIMSSPatron_EG_Prestaciones_en_Dinero_4Function>("TablaIMSSPatron.EG_Prestaciones_en_Dinero_4", ref primitiveElements);
            PrepareFunction<TablaIMSSPatron_Invalidez_y_vida_5Function>("TablaIMSSPatron.Invalidez_y_vida_5", ref primitiveElements);
            PrepareFunction<TablaIMSSPatron_Cesantia_y_vejez_6Function>("TablaIMSSPatron.Cesantia_y_vejez_6", ref primitiveElements);
            PrepareFunction<TablaIMSSPatron_Guarderias_7Function>("TablaIMSSPatron.Guarderias_7", ref primitiveElements);
            PrepareFunction<TablaIMSSPatron_Retiro_8Function>("TablaIMSSPatron.Retiro_8", ref primitiveElements);

            //Tabla IMSSTrabajador
            PrepareFunction<TablaIMSSTrabajador_EG_Especie_GastosMedicos_1Function>("TablaIMSSTrabajador.EG_Especie_GastosMedicos_1", ref primitiveElements);
            PrepareFunction<TablaIMSSTrabajador_EG_Especie_Fija_2Function>("TablaIMSSTrabajador.EG_Especie_Fija_2", ref primitiveElements);
            PrepareFunction<TablaIMSSTrabajador_EG_Especie_mas_3SMDF_3Function>("TablaIMSSTrabajador.EG_Especie_mas_3SMDF_3", ref primitiveElements);
            PrepareFunction<TablaIMSSTrabajador_EG_Prestaciones_en_Dinero_4Function>("TablaIMSSTrabajador.EG_Prestaciones_en_Dinero_4", ref primitiveElements);
            PrepareFunction<TablaIMSSTrabajador_Invalidez_y_vida_5Function>("TablaIMSSTrabajador.Invalidez_y_vida_5", ref primitiveElements);
            PrepareFunction<TablaIMSSTrabajador_Cesantia_y_vejez_6Function>("TablaIMSSTrabajador.Cesantia_y_vejez_6", ref primitiveElements);
            PrepareFunction<TablaIMSSTrabajador_Guarderias_7Function>("TablaIMSSTrabajador.Guarderias_7", ref primitiveElements);
            PrepareFunction<TablaIMSSTrabajador_Retiro_8Function>("TablaIMSSTrabajador.Retiro_8", ref primitiveElements);

            //IMSS
            PrepareFunction<TotalPercepcionesFunction>("TotalPercepciones", ref primitiveElements);
            PrepareFunction<TipoEstadoEmpleadoPeriodoFunction>("TipoEstadoEmpleadoPeriodo", ref primitiveElements);
            PrepareFunction<DiasIMSSAnteriorFunction>("DiasIMSSAnterior", ref primitiveElements);
            PrepareFunction<DiasIMSSVigenteFunction>("DiasIMSSVigente", ref primitiveElements);
            PrepareFunction<DiasIMSSAusenciasAnteriorFunction>("DiasIMSSAusenciasAnterior", ref primitiveElements);
            PrepareFunction<DiasIMSSAusenciasVigenteFunction>("DiasIMSSAusenciasVigente", ref primitiveElements);
            PrepareFunction<DiasIMSSIncapacidadesAnteriorFunction>("DiasIMSSIncapacidadesAnterior", ref primitiveElements);
            PrepareFunction<DiasIMSSIncapacidadesVigenteFunction>("DiasIMSSIncapacidadesVigente", ref primitiveElements);
            PrepareFunction<SBCAnteriorFunction>("SBCAnterior", ref primitiveElements);
            PrepareFunction<SBCVigenteFunction>("SBCVigente", ref primitiveElements);
            PrepareFunction<SalCuotaDiariaIMSSAntFunction>("SalCuotaDiariaIMSSAnt", ref primitiveElements);
            PrepareFunction<SalCuotaDiariaIMSSVigFunction>("SalCuotaDiariaIMSSVig", ref primitiveElements);
            PrepareFunction<Riesgo_trabajoFunction>("Riesgo_trabajo", ref primitiveElements);

            //Vacations 
            PrepareFunction<DiasDeVacacionesFunction>("DiasDeVacaciones", ref primitiveElements);
            PrepareFunction<DiasDeVacacionesPeriodoFunction>("DiasDeVacacionesPeriodo", ref primitiveElements);
            PrepareFunction<DiasDescansoVacFunction>("DiasDescansoVac", ref primitiveElements);
            PrepareFunction<Vac_Sept_Primer_PeriodoFunction>("Vac_Sept_Primer_Periodo", ref primitiveElements);
            PrepareFunction<Vac_Ult_Periodo_CompletoFunction>("Vac_Ult_Periodo_Completo", ref primitiveElements);
            PrepareFunction<VPagoVacacionesFunction>("VPagoVacaciones", ref primitiveElements);

            //Finiquito
            PrepareFunction<Finiquito_CASISR86Function>("Finiquito.CASISR86", ref primitiveElements);
            PrepareFunction<Finiquito_CASUSMOFunction>("Finiquito.CASUSMO", ref primitiveElements);
            PrepareFunction<Finiquito_Indem20Function>("Finiquito.Indem20", ref primitiveElements);
            PrepareFunction<Finiquito_Indem90Function>("Finiquito.Indem90", ref primitiveElements);
            PrepareFunction<Finiquito_PrimaAntigFunction>("Finiquito.PrimaAntig", ref primitiveElements);

            //Configuration
            PrepareFunction<DescontarIncidenciasFunction>("DescontarIncidencias", ref primitiveElements);
            PrepareFunction<OpcionDiasPeriodoFunction>("OpcionDiasPeriodo", ref primitiveElements);
            PrepareFunction<Ajusta_SubsCausado_MenosFunction>("Ajusta_SubsCausado_Menos", ref primitiveElements);
            PrepareFunction<Ajusta_ISRRetenido_MasFunction>("Ajusta_ISRRetenido_Mas", ref primitiveElements);
            PrepareFunction<Ajusta_SubsEntregado_MenosFunction>("Ajusta_SubsEntregado_Menos", ref primitiveElements);
            PrepareFunction<VAjusta_Netear_SubsEntregado_Function>("Ajusta_Netear_SubsEntregado", ref primitiveElements);

            //Fonacot
            PrepareFunction<RetencionFONACOTPeriodoFunction>("RetencionFONACOTPeriodo", ref primitiveElements);

            //Employee
            PrepareFunction<TipoRegimenEmpleadoFunction>("TipoRegimenEmpleado", ref primitiveElements);

            //Extra Hours
            PrepareFunction<HE_ExentasVigenteFunction>("HE_ExentasVigente", ref primitiveElements);
            PrepareFunction<HE_ExentasAnteriorFunction>("HE_ExentasAnterior", ref primitiveElements);
            PrepareFunction<HE_topesXsemanaFunction>("HE_topesXsemana", ref primitiveElements);

            //Destajo
            PrepareFunction<ImporteDestajoFunction>("ImporteDestajo", ref primitiveElements);

            //Incidents
            PrepareFunction<DFT_topesXsemanaFunction>("DFT_topesXsemana", ref primitiveElements);

            //Settlements
            PrepareFunction<AntiguedadEmpleadoFunction>("AntiguedadEmpleado", ref primitiveElements);

            //Septimo Dia
            PrepareFunction<DiasLFTSinSeptimoVigenteFunction>("DiasLFTSinSeptimoVigente", ref primitiveElements);
            PrepareFunction<DiasLFTSinSeptimoAnteriorFunction>("DiasLFTSinSeptimoAnterior", ref primitiveElements);

            //Infonavit 
            PrepareFunction<DiasInfonavitPorcentajeAnteriorFunction>("DiasInfonavitPorcentajeAnterior", ref primitiveElements);
            PrepareFunction<DiasInfonavitPorcentajeVigenteFunction>("DiasInfonavitPorcentajeVigente", ref primitiveElements);
            PrepareFunction<DiasInfonavitAmortizacionFunction>("DiasInfonavitAmortizacion", ref primitiveElements);
            PrepareFunction<DiasInfonavitAmortizacionFunction>("DiasInfonavitPesosAmortizacion", ref primitiveElements);
            PrepareFunction<DiasBimestreCalendarioFunction>("DiasBimestreCalendario", ref primitiveElements);
            PrepareFunction<AplicaPagoSeguroFunction>("AplicaPagoSeguro", ref primitiveElements);
            PrepareFunction<TINFONAVITSegViviendaCuotaFunction>("TINFONAVITSegVivienda.Cuota", ref primitiveElements);

            //Percepciones Generico
            PrepareFunction<PercepcionGenericaTotalFunction>("Percepcion.Total", ref primitiveElements);
        }

        /// <summary>
        /// Catalogs
        /// </summary>
        /// <param name="functionParams"></param>
        private void Catalogs(ref List<PrimitiveElement> primitiveElements)
        {
            //Arguments from Catalogs
            PrepareArgument<EmployeeSalaryZoneFunction>("Empleado[Zona salario]", ref primitiveElements);
            PrepareArgument<EmployeeDailySalaryFunction>("Empleado[Salario diario]", ref primitiveElements);
            PrepareArgument<EmployeeIMSSSBCTypeFunction>("Empleado[IMSS tipo SBC]", ref primitiveElements);
            PrepareArgument<EmployeeSBCVariableFunction>("Empleado[SBC variable]", ref primitiveElements);
            PrepareArgument<EmployeeLFTFinFunction>("Empleado[Salario LFT para finiquito]", ref primitiveElements);
            PrepareArgument<EmployeeDischargeDateFunction>("Empleado[Fecha Baja]", ref primitiveElements);
            PrepareArgument<EmployeeEntryDateFunction>("Empleado[Fecha Alta]", ref primitiveElements);
            PrepareArgument<EmployeeReEntryDateFunction>("Empleado[Fecha Reingreso]", ref primitiveElements);
            PrepareArgument<IncidenciaVigenteFunction>("IncidenciaVigente[Retardos]", ref primitiveElements);
            PrepareArgument<IncidenciaAnteriorFunction>("IncidenciaAnterior[Retardos]", ref primitiveElements);
            PrepareArgument<PeriodoFechaFinFunction>("Periodo[Fecha fin]", ref primitiveElements);
            PrepareArgument<PeriodoFechaFinMesFunction>("Periodo[Fin mes]", ref primitiveElements);
            PrepareArgument<PeriodoFechaInicioFunction>("Periodo[Fecha inicio]", ref primitiveElements);
            PrepareArgument<PeriodoNumeroSeptimosFunction>("Periodo[Número séptimos]", ref primitiveElements);
            PrepareArgument<PeriodoDiasPagoFunction>("Periodo[Días pago]", ref primitiveElements);

            //Acumulados
            PrepareArgument<AcumuladoISRBaseGravadaArt142Function>("Acumulado[ISR Base Gravada  Art142]", ref primitiveElements);
            PrepareArgument<AcumuladoISRLiquidaciongravadoFunction>("Acumulado[ISR Liquidacion gravado]", ref primitiveElements);
            PrepareArgument<AcumuladoMesISPTSubsEmpleoFunction>("AcumuladoMes[ISPT antes de Subs al Empleo]", ref primitiveElements);
            PrepareArgument<AcumuladoMesISRDELFunction>("AcumuladoMes[ISR DEL]", ref primitiveElements);
            PrepareArgument<AcumuladoMesSubsEmpleoAcreditadoFunction>("AcumuladoMes[Subs al Empleo  Acreditado]", ref primitiveElements);
            PrepareArgument<AcumuladoMesISRBaseGravadaFunction>("AcumuladoMes[ISR Base Gravada]", ref primitiveElements);
            PrepareArgument<AcumuladoISRBaseGravadaFunction>("Acumulado[ISR Base Gravada]", ref primitiveElements);
            PrepareArgument<AcumuladoMesISRPercEspecialesGravFunction>("AcumuladoMes[ISR Perc.especiales grav.]", ref primitiveElements);
            PrepareArgument<AcumuladoISRPercEspecialesGravFunction>("Acumulado[ISR Perc.especiales grav.]", ref primitiveElements);
            PrepareArgument<AcumuladoMesSubsEmpleoEntregadoFunction>("AcumuladoMes[Subsidio al empleo Entregado]", ref primitiveElements);
            PrepareArgument<AcumuladoMesISRRetenidoMesFunction>("AcumuladoMes[ISR retenido mes]", ref primitiveElements);
            PrepareArgument<AcumuladoISRTotalPercepcionesFunction>("Acumulado[ISR Total de percepciones]", ref primitiveElements);
            PrepareArgument<AcumuladoAnualISRPrimaVacExentaFunction>("AcumuladoAnual[ISR Prima vac. exenta]", ref primitiveElements);
            PrepareArgument<AcumuladoISRPrimaVacExentaFunction>("Acumulado[ISR Prima vac. exenta]", ref primitiveElements);
            PrepareArgument<AcumuladoAnualISRGratificacionExentaFunction>("AcumuladoAnual[ISR Gratificación exenta]", ref primitiveElements);
            PrepareArgument<AcumuladoISRLiquidacionExentoFunction>("Acumulado[ISR Liquidacion exento]", ref primitiveElements);

            //Percepciones
            PrepareArgument<PercepcionSueldoTotalFunction>("Percepción[Sueldo][Total]", ref primitiveElements);
            PrepareArgument<PercepcionPremiosEficienciaValorFunction>("Percepción[Premios eficiencia][Valor]", ref primitiveElements);
            PrepareArgument<PercepcionPremiosEficienciaTotalFunction>("Percepción[Premios eficiencia][Total]", ref primitiveElements);
            PrepareArgument<PercepcionRetroactivoTotalFunction>("Percepción[Retroactivo][Total]", ref primitiveElements);
            PrepareArgument<PercepcionAjusteSueldosTotalFunction>("Percepción[Ajuste en sueldos][Total]", ref primitiveElements);
            PrepareArgument<PercepcionVacacionesTiempoTotalFunction>("Percepción[Vacaciones a tiempo][Total]", ref primitiveElements);
            PrepareArgument<PercepcionVacacionesTiempoDiasFunction>("Percepción[Vacaciones a tiempo][Dias]", ref primitiveElements);
            PrepareArgument<PercepcionDespensaValorFunction>("Percepción[Despensa][Valor]", ref primitiveElements);
            PrepareArgument<PercepcionDespensaTotalFunction>("Percepción[Despensa][Total]", ref primitiveElements);
            PrepareArgument<PercepcionHorasExtrasTotalFunction>("Percepción[Horas extras][Total]", ref primitiveElements);
            PrepareArgument<PercepcionHorasExtrasISRBaseGravadaFunction>("Percepción[Horas extras][Gravado ISR]", ref primitiveElements);
            PrepareArgument<PercepcionHorasExtrasIMSSBaseGravadaFunction>("Percepción[Horas extras][Gravado IMSS]", ref primitiveElements);
            PrepareArgument<PercepcionSeptimodiaTotalFunction>("Percepción[Séptimo día][Total]", ref primitiveElements);
            PrepareArgument<PercepcionDestajosTotalFunction>("Percepción[Destajos][Total]", ref primitiveElements);
            PrepareArgument<PercepcionComisionesTotalFunction>("Percepción[Comisiones][Total]", ref primitiveElements);
            PrepareArgument<PercepcionIncentivoProductividadTotalFunction>("Percepción[Incentivo productividad][Total]", ref primitiveElements);
            PrepareArgument<PercepcionIncentivosDemorasTotalFunction>("Percepción[Incentivos demoras][Total]", ref primitiveElements);
            PrepareArgument<PercepcionIncapacidadPagadaEmpresaTotalFunction>("Percepción[Incapacidad pagada empresa][Total]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaDominicalDiasFunction>("Percepción[Prima dominical][Dias de Prima]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaDominicalTotalFunction>("Percepción[Prima dominical][Total]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaDominicalGravadoISRFunction>("Percepción[Prima dominical][Gravado ISR]", ref primitiveElements);
            PrepareArgument<PercepcionDiaFestivoTotalFunction>("Percepción[Día festivo descanso][Total]", ref primitiveElements);
            PrepareArgument<PercepcionDiaFestivoGravadoISRFunction>("Percepción[Día festivo descanso][Gravado ISR]", ref primitiveElements);
            PrepareArgument<PercepcionGratificationTotalFunction>("Percepción[Gratificación][Total]", ref primitiveElements);
            PrepareArgument<PercepcionCompensacionTotalFunction>("Percepción[Compensación][Total]", ref primitiveElements);
            PrepareArgument<PercepcionBonoPuntualidadTotalFunction>("Percepción[Bono puntualidad][Total]", ref primitiveElements);
            PrepareArgument<PercepcionBonoPuntualidadGravadoIMSSFunction>("Percepción[Bono puntualidad][Gravado IMSS]", ref primitiveElements);
            PrepareArgument<PercepcionAnticipoSueldosTotalFunction>("Percepción[Anticipo de sueldos][Total]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaVacacionesTiempoDiasPrimaFunction>("Percepción[Prima de vacaciones a tiempo][Dias de Prima]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaVacacionesTiempoTotalFunction>("Percepción[Prima de vacaciones a tiempo][Total]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaVacacionesTiempoGravadoISRFunction>("Percepción[Prima de vacaciones a tiempo][Gravado ISR]", ref primitiveElements);
            PrepareArgument<PercepcionVacacionesReportadasDiasFunction>("Percepción[Vacaciones reportadas $][Dias]", ref primitiveElements);
            PrepareArgument<PercepcionVacacionesReportadasTotalFunction>("Percepción[Vacaciones reportadas $][Total]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaVacacionesReportadasDiasFunction>("Percepción[Prima de vacaciones reportada $][Dias]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaVacacionesReportadasTotalFunction>("Percepción[Prima de vacaciones reportada $][Total]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaVacacionesReportadasGravadoISRFunction>("Percepción[Prima de vacaciones reportada $][Gravado ISR]", ref primitiveElements);
            PrepareArgument<PercepcionDiasVacacionesDiasFunction>("Percepción[Días de vacaciones][Dias]", ref primitiveElements);
            PrepareArgument<PercepcionDiasVacacionesTotalFunction>("Percepción[Días de vacaciones][Total]", ref primitiveElements);
            PrepareArgument<PercepcionAguinaldoValorFunction>("Percepción[Aguinaldo][Valor]", ref primitiveElements);
            PrepareArgument<PercepcionAguinaldoTotalFunction>("Percepción[Aguinaldo][Total]", ref primitiveElements);
            PrepareArgument<PercepcionRepartoUtilidadesTotalFunction>("Percepción[Reparto de utilidades][Total]", ref primitiveElements);
            PrepareArgument<PercepcionIndemnizacionTotalFunction>("Percepción[Indemnización][Total]", ref primitiveElements);
            PrepareArgument<PercepcionSeparacionUnicaTotalFunction>("Percepción[Separación Unica][Total]", ref primitiveElements);
            PrepareArgument<PercepcionPrimaAntiguedadTotalFunction>("Percepción[Prima de antiguedad][Total]", ref primitiveElements);
            PrepareArgument<PercepcionFondoAhorroEmpresaValorFunction>("Percepción[Fondo ahorro empresa][Valor]", ref primitiveElements);
            PrepareArgument<PercepcionFondoAhorroEmpresaTotalFunction>("Percepción[Fondo ahorro empresa][Total]", ref primitiveElements);
            PrepareArgument<PercepcionDespensaGravadoIMSSFunction>("Percepción[Despensa][Gravado IMSS]", ref primitiveElements);
            PrepareArgument<PercepcionDeporteCulturaValorFunction>("Percepción[Deporte y cultura][Valor]", ref primitiveElements);
            PrepareArgument<PercepcionDeporteCulturaTotalFunction>("Percepción[Deporte y cultura][Total]", ref primitiveElements);
            PrepareArgument<PercepcionAnticipoVacacionesTotalFunction>("Percepción[Anticipo vacaciones Percepción][Total]", ref primitiveElements);
            PrepareArgument<PercepcionDestajoSueldoTotalFunction>("Percepción[Destajo sueldo][Total]", ref primitiveElements);
            PrepareArgument<PercepcionComisionSueldoTotalFunction>("Percepción[Comisión sueldo][Total]", ref primitiveElements);

            //Deduction
            PrepareArgument<DeduccionCuotaSindicalPorcentajeFunction>("Deducción[Cuota sindical][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionCajadeAhorroPorcentajeFunction>("Deducción[Caja de ahorro][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPrestamoCajadeAhorroPorcentajeFunction>("Deducción[Préstamo caja de ahorro][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionInteresesPtoAhorroPorcentajeFunction>("Deducción[Intereses Ptmo ahorro][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPrestamoInfonavitPorcentajeFunction>("Deducción[Préstamo Infonavit][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionFonacotRevolventePorcentajeFunction>("Deducción[Fonacot revolvente][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPrestamoEmpresaPorcentajeFunction>("Deducción[Préstamo empresa][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionFondoAhorroPorcentajeFunction>("Deducción[Fondo de ahorro][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionGeneralDiasFunction>("Deducción[Deduccion general][Dias]", ref primitiveElements);
            PrepareArgument<DeduccionPrestamoFondoAhorroValorFunction>("Deducción[Préstamo fondo de ahorro][Valor]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoCajaAhorro2PorcentajeFunction>("Deducción[Ptmo caja de ahorro2][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoCajaAhorro3PorcentajeFunction>("Deducción[Ptmo caja de ahorro3][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoCajaAhorro4PorcentajeFunction>("Deducción[Ptmo caja de ahorro4][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoEmpresa2PorcentajeFunction>("Deducción[Ptmo empresa2][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoEmpresa3PorcentajeFunction>("Deducción[Ptmo empresa3][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoEmpresa4PorcentajeFunction>("Deducción[Ptmo empresa4][Porcentaje]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoFondoAhorro2ValorFunction>("Deducción[Ptmo fondo de ahorro2][Valor]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoFondoAhorro3ValorFunction>("Deducción[Ptmo fondo de ahorro3][Valor]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoFondoAhorro4ValorFunction>("Deducción[Ptmo fondo de ahorro4][Valor]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoInfonavitVsmValorFunction>("Deducción[Préstamo Infonavit vsm][Valor]", ref primitiveElements);
            PrepareArgument<DeduccionPtmoInfonavitCfValorFunction>("Deducción[Préstamo Infonavit cf][Valor]", ref primitiveElements);

            //Extra Hours
            PrepareArgument<IncidenciaHEFunction>("Incidencia[Horas extras]", ref primitiveElements);
            PrepareArgument<VSalarioHEAntFunction>("VSalarioHEAnt", ref primitiveElements);
            PrepareArgument<VSalarioHEVigFunction>("VSalarioHEVig", ref primitiveElements);
            PrepareArgument<VSalarioExentoIMSSXhoraFunction>("VSalarioExentoIMSSXhora", ref primitiveElements);

            //Incidents
            PrepareArgument<IncidenciaVigenteIncapacidadPagadaEmpresaFunction>("IncidenciaVigente[Incapacidad pagada por la empresa]", ref primitiveElements);
            PrepareArgument<IncidenciaAnteriorIncapacidadPagadaEmpresaFunction>("IncidenciaAnterior[Incapacidad pagada por la empresa]", ref primitiveElements);
            PrepareArgument<IncidenciaVigenteDiaFestivoTrabajadoFunction>("IncidenciaVigente[Día festivo trabajado]", ref primitiveElements);
            PrepareArgument<IncidenciaAnteriorDiaFestivoTrabajadoFunction>("IncidenciaAnterior[Día festivo trabajado]", ref primitiveElements);

            //Formulas Valores
            PrepareArgument<VacacionesATiempoValorFunction>("VacacionesATiempoValor", ref primitiveElements);
            PrepareArgument<PrimaVacacionesATiempoValorFunction>("PrimaVacacionesATiempoValor", ref primitiveElements);

            //Infonavit
            PrepareArgument<MontoDescINFONAVITFunction>("MontoDescINFONAVIT", ref primitiveElements);
            
        }

        /// <summary>
        /// Sin categoria   
        /// </summary>
        /// <param name="functionParams"></param>
        private void WithoutCategory(ref List<PrimitiveElement> primitiveElements)
        {
            //Composed arguments with formulas
            PrepareArgumentWithFormulaFunction("UMA", "UMA.Valor_UMA{Periodo[Fecha fin]}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSalarioPrestaciones", "iif(Empleado[IMSS tipo SBC] = 86 , Empleado[SBC variable], Empleado[Salario diario])", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("SalarioMinimoDF", "SalariosMinimos.Zona_A{Periodo[Fecha fin]}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("SalarioMinimo", "IIF(Empleado[Zona salario] = 'A' , SalariosMinimos.Zona_A{Periodo[Fecha fin]}, IIF(Empleado[Zona salario] = 'B' , SalariosMinimos.Zona_B{Periodo[Fecha fin]}, SalariosMinimos.Zona_C{Periodo[Fecha fin]}))", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasdePeriodo", "Periodo[Fecha fin] - Periodo[Fecha inicio] + 1", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasFraccion", "iif(FRAC(Periodo[Días pago]) > 0.01 , FRAC(Periodo[Días pago]), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasLFTSeptimosAnterior", "DiasLFTSeptimosAnterior()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasLFTSeptimosVigente", "DiasLFTSeptimosVigente()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasLFTSalarioAnterior", "DiasLFTSalarioAnterior()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasLFTSinSalarioXAlta", "(Periodo[Días pago] + Periodo[Número séptimos]) -(VDiasLFTSalarioAnterior + DiasLFTSalarioVigente() + vDiasFraccion)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasLFTSalarioVigente", "iif(FRAC(Periodo[Días pago]) > 0.01 , MAX((Periodo[Días pago] + Periodo[Número séptimos] - VDiasLFTSalarioAnterior - VDiasLFTSinSalarioXAlta), 0), DiasLFTSalarioVigente())", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasDerechoSueldoAnterior", "MAX((VDiasLFTSalarioAnterior - VDiasLFTSeptimosAnterior - DiasLFTSinSalarioAnterior()), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasDerechoSueldoVigente", "MAX((VDiasLFTSalarioVigente - VDiasLFTSeptimosVigente - DiasLFTSinSalarioVigente()), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VPeriodoDeVacaciones", "PeriodoEnVacaciones()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSalDiarioVigente", "Empleado[Salario diario]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSalDiarioAnt", "SalCuotaDiariaAnt()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VHorasRetVig", "IncidenciaSinDerechoASueldo(IncidenciaVigente[Retardos])", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VHorasRetAnt", "IncidenciaSinDerechoASueldo(IncidenciaAnterior[Retardos])", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSalarioXhoraVig", "Empleado[Salario diario] / HorasPorTurno()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSalarioXhoraAnt", "SalCuotaDiariaAnt() / HorasPorTurno()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VImpRetardos", "(VHorasRetVig * VSalarioXhoraVig) + (VHorasRetAnt * VSalarioXhoraAnt)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VdiasDerechoSueldoFin", "IIF(VDiasdePeriodo = 7 , 0 , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VdiasDerechoSueldoFin2", "IIF(VDiasDerechoSueldoVigente = 6 , 0 , VdiasDerechoSueldoFin)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VPago_SueldoFin", "VSalDiarioVigente * VdiasDerechoSueldoFin2 * IIF(TipoProceso() = 70 , 1 , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VEstadoEmpleadoPeriodo", "TipoEstadoEmpleadoPeriodo()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vTotalPercepciones", "TotalPercepciones()", ref primitiveElements);
        }

        /// <summary>
        /// IMSS
        /// </summary>
        private void GeneralsIMSS(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("SalarioMinimoZonadelEmpleado", "IIF(Empleado[Zona salario] = 'A' , SalariosMinimos.Zona_A{Periodo[Fecha fin]}, IIF(Empleado[Zona salario] = 'B' , SalariosMinimos.Zona_B{Periodo[Fecha fin]}, SalariosMinimos.Zona_C{Periodo[Fecha fin]}))", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasIMSSAnterior", "DiasIMSSAnterior(0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasIMSSVigente", "DiasIMSSVigente(0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAusentismoIMSSAnterior", "DiasIMSSAusenciasAnterior()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAusentismoIMSSVigente", "DiasIMSSAusenciasVigente()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VIncapacidadesIMSSAnterior", "DiasIMSSIncapacidadesAnterior()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VIncapacidadesIMSSVigente", "DiasIMSSIncapacidadesVigente()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSBCAnterior", "SBCAnterior()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSBCVigente", "SBCVigente()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSalCuotaDiariaIMSSAnt", "SalCuotaDiariaIMSSAnt()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSalCuotaDiariaIMSSVig", "SalCuotaDiariaIMSSVig()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VIncidencias", "VAusentismoIMSSAnterior + VAusentismoIMSSVigente + VIncapacidadesIMSSAnterior + VIncapacidadesIMSSVigente", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("FactorDescINFONAVIT", "FactorDescINFONAVIT.Factor_Descuento{Periodo[Fecha fin]}", ref primitiveElements);
        }

        /// <summary>
        /// Vacations
        /// </summary>
        private void Vacations(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("VDiasVacaciones", "DiasDeVacaciones()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasVacacionesPeriodo", "DiasDeVacacionesPeriodo()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasXVacaciones", "VDiasVacaciones - VDiasVacacionesPeriodo", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasXFinVacaciones", "(DiasDescansoVac() - Vac_Sept_Primer_Periodo()) *( - 1)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VParteProporcional7oDiaAnt", "MIN(MAX((VDiasLFTSalarioAnterior - VDiasLFTSeptimosAnterior - DiasLFTSinSeptimoAnterior() - VHorasRetAnt / HorasPorTurno()), 0) * Periodo[Número séptimos] / Periodo[Días pago], Periodo[Número séptimos])", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VParteProporcional7oDiaVig", "MIN(MAX((VDiasLFTSalarioVigente - VDiasLFTSeptimosVigente - DiasLFTSinSeptimoVigente() - VHorasRetVig / HorasPorTurno()), 0) * Periodo[Número séptimos] / Periodo[Días pago], Periodo[Número séptimos])", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSeptimos_Periodo_Completo", "SeptimosvacPerCompleto() * VSalDiarioVigente", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSeptimos_Dias_Vac", "DiasDescansoVac() * VSalDiarioVigente", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSeptimos_Periodo_Sueldo", "VSalDiarioVigente * Periodo[Número séptimos]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSeptimos_Definidos_Periodo", "Periodo[Número séptimos]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSeptimos_Ult_Periodo", "iif(VSeptimos_Definidos_Periodo , IIF(Vac_Ult_Periodo_Completo() = DiasDescansoVac(), 0 , VSeptimos_Periodo_Sueldo), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSeptimos_Dias", "IIF(Periodo[Número séptimos] = 0 , 0 , IIF(VPeriodoDeVacaciones , MAX(VSeptimos_Periodo_Completo , 0), MAX((VParteProporcional7oDiaAnt * VSalDiarioAnt) +(VParteProporcional7oDiaVig * VSalDiarioVigente) +(VSeptimos_Periodo_Completo), 0)))", ref primitiveElements);
        }

        private void EG1Calculation(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("DiasAnteriorEnfermedadGeneral", "VDiasIMSSAnterior - VIncapacidadesIMSSAnterior", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("DiasVigenteEnfermedadGeneral", "VDiasIMSSVigente - VIncapacidadesIMSSVigente + VDiasXVacaciones - VDiasXFinVacaciones", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("TopeEnfGeneral", "UMA * TopesSGDF.EG_Especie_GastosMedicos_1{Periodo[Fecha fin]}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaEGAnterior", "IIF(VSBCAnterior >= TopeEnfGeneral , TopeEnfGeneral , VSBCAnterior)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaEGVigente", "IIF(VSBCVigente >= TopeEnfGeneral , TopeEnfGeneral , VSBCVigente)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatEG1PreviaAnt", "(BaseDiariaEGAnterior * DiasAnteriorEnfermedadGeneral) *(TablaIMSSPatron.EG_Especie_GastosMedicos_1{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatEG1PreviaVig", "(BaseDiariaEGVigente * DiasVigenteEnfermedadGeneral) *(TablaIMSSPatron.EG_Especie_GastosMedicos_1{Periodo[Fecha fin]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroEG1PreviaAnt", "(BaseDiariaEGAnterior * DiasAnteriorEnfermedadGeneral) *(TablaIMSSTrabajador.EG_Especie_GastosMedicos_1{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroEG1PreviaVig", "(BaseDiariaEGVigente * DiasVigenteEnfermedadGeneral) *(TablaIMSSTrabajador.EG_Especie_GastosMedicos_1{Periodo[Fecha fin]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatronEG1", "ROUNDTO((IIF(VSalCuotaDiariaIMSSVig <= SalarioMinimo , CuotaPatEG1PreviaVig + CuotaObreroEG1PreviaVig , CuotaPatEG1PreviaVig) + IIF(VSalCuotaDiariaIMSSAnt <= SalarioMinimo , CuotaPatEG1PreviaAnt + CuotaObreroEG1PreviaAnt , CuotaPatEG1PreviaAnt)), 2)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroEG1", "ROUNDTO((IIF(VSalCuotaDiariaIMSSAnt <= SalarioMinimo , 0 , CuotaObreroEG1PreviaAnt) + IIF(VSalCuotaDiariaIMSSVig <= SalarioMinimo , 0 , CuotaObreroEG1PreviaVig)), 2)", ref primitiveElements);
        }

        private void EG2Calculation(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("CuotaPatronEG2", "ROUNDTO(((DiasAnteriorEnfermedadGeneral + DiasVigenteEnfermedadGeneral) * UMA *(TablaIMSSPatron.EG_Especie_Fija_2{Periodo[Fecha fin]} / 100)), 2)", ref primitiveElements);
        }

        private void EG3Calculation(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("BaseDiariaEG3SMGDFAnterior", "IIF(BaseDiariaEGAnterior > 3 * UMA , BaseDiariaEGAnterior -(3 * UMA), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaEG3SMGDFVigente", "IIF(BaseDiariaEGVigente > 3 * UMA , BaseDiariaEGVigente -(3 * UMA), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatronEG3SMGDF", "ROUNDTO((BaseDiariaEG3SMGDFAnterior * DiasAnteriorEnfermedadGeneral *(TablaIMSSPatron.EG_Especie_mas_3SMDF_3{Periodo[Fecha inicio]} / 100) + BaseDiariaEG3SMGDFVigente * DiasVigenteEnfermedadGeneral *(TablaIMSSPatron.EG_Especie_mas_3SMDF_3{Periodo[Fecha fin]} / 100)), 2)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroEG3SMGDF", "ROUNDTO((BaseDiariaEG3SMGDFAnterior * DiasAnteriorEnfermedadGeneral *(TablaIMSSTrabajador.EG_Especie_mas_3SMDF_3{Periodo[Fecha inicio]} / 100) + BaseDiariaEG3SMGDFVigente * DiasVigenteEnfermedadGeneral *(TablaIMSSTrabajador.EG_Especie_mas_3SMDF_3{Periodo[Fecha fin]} / 100)), 2)", ref primitiveElements);
        }

        private void EG4Calculation(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("CuotaPatEG4PreviaAnt", "(BaseDiariaEGAnterior * DiasAnteriorEnfermedadGeneral) *(TablaIMSSPatron.EG_Prestaciones_en_Dinero_4{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatEG4PreviaVig", "(BaseDiariaEGVigente * DiasVigenteEnfermedadGeneral) *(TablaIMSSPatron.EG_Prestaciones_en_Dinero_4{Periodo[Fecha fin]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroEG4PreviaAnt", "(BaseDiariaEGAnterior * DiasAnteriorEnfermedadGeneral) *(TablaIMSSTrabajador.EG_Prestaciones_en_Dinero_4{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroEG4PreviaVig", "(BaseDiariaEGVigente * DiasVigenteEnfermedadGeneral) *(TablaIMSSTrabajador.EG_Prestaciones_en_Dinero_4{Periodo[Fecha fin]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatronEG4", "ROUNDTO((IIF(VSalCuotaDiariaIMSSVig <= SalarioMinimo , CuotaPatEG4PreviaVig + CuotaObreroEG4PreviaVig , CuotaPatEG4PreviaVig) + IIF(VSalCuotaDiariaIMSSAnt <= SalarioMinimo , CuotaPatEG4PreviaAnt + CuotaObreroEG4PreviaAnt , CuotaPatEG4PreviaAnt)), 2)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroEG4", "ROUNDTO((IIF(VSalCuotaDiariaIMSSVig <= SalarioMinimo , 0 , CuotaObreroEG4PreviaVig) + IIF(VSalCuotaDiariaIMSSAnt <= SalarioMinimo , 0 , CuotaObreroEG4PreviaAnt)), 2)", ref primitiveElements);
        }

        private void Table5Calculation(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("DiasAnteriorInvalidezyVida", "VDiasIMSSAnterior - VIncapacidadesIMSSAnterior - VAusentismoIMSSAnterior", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("DiasVigenteInvalidezyVida", "VDiasIMSSVigente - VIncapacidadesIMSSVigente - VAusentismoIMSSVigente + VDiasXVacaciones - VDiasXFinVacaciones", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("TopeInvalidezyVida", "UMA * TopesSGDF.Invalidez_y_vida_5{Periodo[Fecha fin]}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaInvalidezyVidaAnt", "IIF(VSBCAnterior >= TopeInvalidezyVida , TopeInvalidezyVida , VSBCAnterior)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaInvalidezyVidaVigente", "IIF(VSBCVigente >= TopeInvalidezyVida , TopeInvalidezyVida , VSBCVigente)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatInvalidezyVidaPreAnt", "DiasAnteriorInvalidezyVida * BaseDiariaInvalidezyVidaAnt *(TablaIMSSPatron.Invalidez_y_vida_5{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatInvalidezyVidaPreVig", "DiasVigenteInvalidezyVida * BaseDiariaInvalidezyVidaVigente *(TablaIMSSPatron.Invalidez_y_vida_5{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroInvalidezYVidaPreAnt", "DiasAnteriorInvalidezyVida * BaseDiariaInvalidezyVidaAnt *(TablaIMSSTrabajador.Invalidez_y_vida_5{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroInvalidezYVidaPreVig", "DiasVigenteInvalidezyVida * BaseDiariaInvalidezyVidaVigente *(TablaIMSSTrabajador.Invalidez_y_vida_5{Periodo[Fecha fin]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatronInvalidezyVida5", "ROUNDTO((IIF(VSalCuotaDiariaIMSSAnt <= SalarioMinimo , CuotaPatInvalidezyVidaPreAnt + CuotaObreroInvalidezYVidaPreAnt , CuotaPatInvalidezyVidaPreAnt) + IIF(VSalCuotaDiariaIMSSVig <= SalarioMinimo , CuotaPatInvalidezyVidaPreVig + CuotaObreroInvalidezYVidaPreVig , CuotaPatInvalidezyVidaPreVig)), 2)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroInvalidezYVida5", "ROUNDTO((IIF(VSalCuotaDiariaIMSSAnt <= SalarioMinimo , 0 , CuotaObreroInvalidezYVidaPreAnt) + IIF(VSalCuotaDiariaIMSSVig <= SalarioMinimo , 0 , CuotaObreroInvalidezYVidaPreVig)), 2)", ref primitiveElements);
        }

        private void Table6Calculation(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("CuotaPatCesantiaPreviaAnt", "DiasAnteriorInvalidezyVida * BaseDiariaInvalidezyVidaAnt *(TablaIMSSPatron.Cesantia_y_vejez_6{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatCesantiaPreviaVig", "DiasVigenteInvalidezyVida * BaseDiariaInvalidezyVidaVigente *(TablaIMSSPatron.Cesantia_y_vejez_6{Periodo[Fecha fin]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroCesantiaPreviaAnt", "DiasAnteriorInvalidezyVida * BaseDiariaInvalidezyVidaAnt *(TablaIMSSTrabajador.Cesantia_y_vejez_6{Periodo[Fecha inicio]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroCesantiaPreviaVig", "DiasVigenteInvalidezyVida * BaseDiariaInvalidezyVidaVigente *(TablaIMSSTrabajador.Cesantia_y_vejez_6{Periodo[Fecha fin]} / 100)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaPatronCesantia6", "ROUNDTO((IIF(VSalCuotaDiariaIMSSAnt <= SalarioMinimo , CuotaPatCesantiaPreviaAnt + CuotaObreroCesantiaPreviaAnt , CuotaPatCesantiaPreviaAnt) + IIF(VSalCuotaDiariaIMSSVig <= SalarioMinimo , CuotaPatCesantiaPreviaVig + CuotaObreroCesantiaPreviaVig , CuotaPatCesantiaPreviaVig)), 2)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaObreroCesantia6", "ROUNDTO((IIF(VSalCuotaDiariaIMSSAnt <= SalarioMinimo , 0 , CuotaObreroCesantiaPreviaAnt) + iif(VSalCuotaDiariaIMSSVig <= SalarioMinimo , 0 , CuotaObreroCesantiaPreviaVig)), 2)", ref primitiveElements);
        }

        private void Table7Calculation(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("DiasAnteriorGuarderias", "VDiasIMSSAnterior - VIncapacidadesIMSSAnterior - VAusentismoIMSSAnterior", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("DiasVigenteGuarderias", "VDiasIMSSVigente - VIncapacidadesIMSSVigente - VAusentismoIMSSVigente + VDiasXVacaciones - VDiasXFinVacaciones", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("TopeGuarderias", "UMA * TopesSGDF.Guarderias_7{Periodo[Fecha fin]}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaGuarderiasAnterior", "IIF(VSBCAnterior > TopeGuarderias , TopeGuarderias , VSBCAnterior)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaGuarderiasVigente", "IIF(VSBCVigente > TopeGuarderias , TopeGuarderias , VSBCVigente)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaGuarderias7", "BaseDiariaGuarderiasAnterior * DiasAnteriorGuarderias *(TablaIMSSPatron.Guarderias_7{Periodo[Fecha inicio]} / 100) + BaseDiariaGuarderiasVigente * DiasVigenteGuarderias *(TablaIMSSPatron.Guarderias_7{Periodo[Fecha fin]} / 100)", ref primitiveElements);
        }

        private void Table8Calculation(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("DiasAnteriorRetiro", "VDiasIMSSAnterior - VAusentismoIMSSAnterior", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("DiasVigenteRetiro", "VDiasIMSSVigente - VAusentismoIMSSVigente + VDiasXVacaciones - VDiasXFinVacaciones", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("TopeRetiro", "UMA * TopesSGDF.Guarderias_7{Periodo[Fecha fin]}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaRetiroAnterior", "IIF(VSBCAnterior > TopeRetiro , TopeRetiro , VSBCAnterior)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaRetiroVigente", "IIF(VSBCVigente > TopeRetiro , TopeRetiro , VSBCVigente)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaRetiro8", "DiasAnteriorRetiro * BaseDiariaRetiroAnterior *(TablaIMSSPatron.Retiro_8{Periodo[Fecha inicio]} / 100) + DiasVigenteRetiro * BaseDiariaRetiroVigente *(TablaIMSSPatron.Retiro_8{Periodo[Fecha fin]} / 100)", ref primitiveElements);
        }

        private void IMSSWorkRisk(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("DiasAnteriorRT", "VDiasIMSSAnterior - VIncapacidadesIMSSAnterior - VAusentismoIMSSAnterior", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("DiasVigenteRT", "VDiasIMSSVigente - VIncapacidadesIMSSVigente - VAusentismoIMSSVigente + VDiasXVacaciones - VDiasXFinVacaciones", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("TopeRT", "TopeRetiro", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaRT9", "BaseDiariaGuarderiasAnterior * DiasAnteriorRT *(Riesgo_trabajo()) + BaseDiariaGuarderiasVigente * DiasVigenteRT *(Riesgo_trabajo())", ref primitiveElements);
        }

        private void ISPT_Art142(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("VArt142_BaseGravada", "Acumulado[ISR Base Gravada  Art142]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Vart142_OpcionElegida", "VArt142_BaseGravada", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_Fracc_I", "(VArt142_BaseGravada / 365) * 30.4", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_CuotaDiaria", "IIF(Empleado[IMSS tipo SBC] = 86 , IIF(Empleado[Salario LFT para finiquito] > 0.01 , Empleado[Salario LFT para finiquito], Empleado[SBC variable]), Empleado[Salario diario])", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_Aplica_Subs", "Finiquito.CASISR86{1}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_1BISPT_USMO", "VArt142_CuotaDiaria * 30.0", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_2IM", "(VArt142_1BISPT_USMO - TVigISRMensual.Limite_inferior{VArt142_1BISPT_USMO}) * TVigISRMensual.Porcentaje{VArt142_1BISPT_USMO} / 100", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_3CF", "TVigISRMensual.Cuota_fija{VArt142_1BISPT_USMO}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_4ISPT", "VArt142_2IM + VArt142_3CF", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Vart142_5ISPToSE", "(VArt142_4ISPT - TVigSubEmpMensual.Subs_al_empleo{VArt142_1BISPT_USMO} * VArt142_Aplica_Subs)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Vart142_1EspBISPT_USMO", "VArt142_1BISPT_USMO + VArt142_Fracc_I", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_2EspIM", "(Vart142_1EspBISPT_USMO - TVigISRMensual.Limite_inferior{Vart142_1EspBISPT_USMO}) * TVigISRMensual.Porcentaje{Vart142_1EspBISPT_USMO} / 100", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_3EspCF", "TVigISRMensual.Cuota_fija{Vart142_1EspBISPT_USMO}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_4EspISPT", "VArt142_2EspIM + VArt142_3EspCF", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_5EspISPToSE", "(VArt142_4EspISPT - TVigSubEmpMensual.Subs_al_empleo{Vart142_1EspBISPT_USMO} * VArt142_Aplica_Subs)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_Fracc_III", "VArt142_5EspISPToSE - max(Vart142_5ISPToSE , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VArt142_Fracc_V", "IIF(VArt142_BaseGravada <= 0 , 0 , VArt142_Fracc_III / VArt142_Fracc_I)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Vart142_Fracc_IV", "VArt142_Fracc_V * Vart142_OpcionElegida", ref primitiveElements);
        }

        private void ISR_Settlement(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("VAplica_SE_a_USMO_Finiquitos", "Finiquito.CASUSMO{1}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_USMO", "IIF(Empleado[IMSS tipo SBC] = 86 , Empleado[Salario LFT para finiquito], Empleado[Salario diario])", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_1BISPT", "VFin_USMO * 30", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_2IM", "(VFin_1BISPT - TVigISRMensual.Limite_inferior{VFin_1BISPT}) * TVigISRMensual.Porcentaje{VFin_1BISPT} / 100", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_3CF", "TVigISRMensual.Cuota_fija{VFin_1BISPT}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_4ISPT", "VFin_2IM + VFin_3CF", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_5ISPT", "MAX((VFin_4ISPT - VAplica_SE_a_USMO_Finiquitos * TVigSubEmpMensual.Subs_al_empleo{VFin_1BISPT}), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_6TRI", "VFin_5ISPT / VFin_1BISPT", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_Gravable_Finiquito", "Acumulado[ISR Liquidacion gravado]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFin_ISPT_Definitivo_de_Finiquitos", "VFin_Gravable_Finiquito * VFin_6TRI", ref primitiveElements);
        }

        private void ISPT_Generals(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("vDiasVacYaPagadas", "IIF(VDiasXVacaciones < 0 , - VDiasXVacaciones , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDescontarIncidencias", "DescontarIncidencias()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vOpcionDiasPeriodo", "OpcionDiasPeriodo()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_Correspondido_M", "AcumuladoMes[ISPT antes de Subs al Empleo]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_Retenido_M", "AcumuladoMes[ISR DEL]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vSE_Correspondido_M", "AcumuladoMes[Subs al Empleo  Acreditado]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vBaseTotal_M", "AcumuladoMes[ISR Base Gravada] + AcumuladoMes[ISR Perc.especiales grav.]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vBaseNormal_P", "Acumulado[ISR Base Gravada]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vBaseEspecial_P", "Acumulado[ISR Perc.especiales grav.]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vBaseTotal_P", "vBaseNormal_P + vBaseEspecial_P", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasDePago_P", "Periodo[Días pago] + Periodo[Número séptimos]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasDespuesBaja", "IIF((Empleado[Fecha Baja] > MAX(Empleado[Fecha Alta], Empleado[Fecha Reingreso])) Y ((Empleado[Fecha Baja] >= Periodo[Fecha inicio]) Y (Empleado[Fecha Baja] <= Periodo[Fecha fin])), Periodo[Fecha fin] - Empleado[Fecha Baja], 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasAntesIngreso", "IIF((Periodo[Fecha inicio] > MAX(Empleado[Fecha Alta], Empleado[Fecha Reingreso])) O (Periodo[Fecha fin] < MAX(Empleado[Fecha Alta], Empleado[Fecha Reingreso])), 0 , MAX(Empleado[Fecha Alta], Empleado[Fecha Reingreso]) - Periodo[Fecha inicio])", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasNoConsiderar", "IIF(vDescontarIncidencias = 1 , IIF(vOpcionDiasPeriodo = 2 , vDiasAntesIngreso + vDiasDespuesBaja + vDiasAus() + vDiasInc(), vDiasAus() + vDiasInc()), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasTarifa_M", "30.4", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasTarifaISPT", "vDiasDePago_P +(VDiasXVacaciones + DiasDescansoTarjeta())", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasTarifaISR", "vDiasTarifaISPT - IIF(vOpcionDiasPeriodo = 2 , vDiasNoConsiderar , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasTarifaSE", "vDiasTarifaISPT - vDiasNoConsiderar", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vBaseISPT", "vBaseTotal_P / vDiasTarifaISR * 30.4", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vBaseSE", "vBaseTotal_P / vDiasTarifaSE * 30.4", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjusta_SubsCausado_Menos", "Ajusta_SubsCausado_Menos()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjusta_ISRRetenido_Mas", "Ajusta_ISRRetenido_Mas()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjusta_SubsEntregado_Menos", "Ajusta_SubsEntregado_Menos()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjusta_Netear_SubsEntregado", "Ajusta_Netear_SubsEntregado()", ref primitiveElements);
        }

        private void ISPT_Monthly(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("vLimiteInferior", "TVigISRMensual.Limite_inferior{vBaseISPT}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vCuotaFija", "TVigISRMensual.Cuota_fija{vBaseISPT}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vPorcentaje", "TVigISRMensual.Porcentaje{vBaseISPT} / 100", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_TarifaMes", "((vBaseISPT - vLimiteInferior) * vPorcentaje) + vCuotaFija", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vSE_TarifaMes", "TVigSubEmpMensual.Subs_al_empleo{vBaseSE}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_CORRESPONDE_P", "vISPT_TarifaMes / vDiasTarifa_M * vDiasTarifaISR", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vSE_CORRESPONDE_P", "vSE_TarifaMes / vDiasTarifa_M * vDiasTarifaSE", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_RETENER_P", "IIF(vBaseTotal_P <=(SalarioMinimo * vDiasTarifaISR), 0 , IIF(vISPT_CORRESPONDE_P > vSE_CORRESPONDE_P , vISPT_CORRESPONDE_P - vSE_CORRESPONDE_P , 0))", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("ISR de la Tarifa", "vISPT_RETENER_P", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vSE_ENTREGAR_P", "IIF(vSE_CORRESPONDE_P > vISPT_CORRESPONDE_P , vSE_CORRESPONDE_P - vISPT_CORRESPONDE_P , 0)", ref primitiveElements);
        }

        private void ISPT_Monthlylized(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("vNoCalcular", "IIF(vBaseTotal_P <= 0 , 1 , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vBaseNormalDiaria", "vBaseNormal_P / vDiasTarifaISPT", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiasCompletarBase", "vDiasTarifa_M -(DiasTranscurridosMes() - vDiasDePago_P) - vDiasVacYaPagadas", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vBaseNormalPorRecibir", "vBaseNormalDiaria * vDiasCompletarBase", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v01_BaseMensual_SinEsp", "IIF(vNoCalcular = 1 , 0 , vBaseTotal_M + vBaseNormalPorRecibir)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v02_BaseMensual_ConEsp", "v01_BaseMensual_SinEsp + vBaseEspecial_P", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v01_LimiteInferior", "TVigISRMensual.Limite_inferior{v01_BaseMensual_SinEsp}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v01_CuotaFija", "TVigISRMensual.Cuota_fija{v01_BaseMensual_SinEsp}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v01_Porcentaje", "TVigISRMensual.Porcentaje{v01_BaseMensual_SinEsp} / 100", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v01_ISPT_TarifaMes", "((v01_BaseMensual_SinEsp - v01_LimiteInferior) * v01_Porcentaje) + v01_CuotaFija", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v01_SE_TarifaMes", "TVigSubEmpMensual.Subs_al_empleo{v01_BaseMensual_SinEsp}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v02_LimiteInferior", "TVigISRMensual.Limite_inferior{v02_BaseMensual_ConEsp}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v02_CuotaFija", "TVigISRMensual.Cuota_fija{v02_BaseMensual_ConEsp}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v02_Porcentaje", "TVigISRMensual.Porcentaje{v02_BaseMensual_ConEsp} / 100", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v02_ISPT_TarifaMes", "((v02_BaseMensual_ConEsp - v02_LimiteInferior) * v02_Porcentaje) + v02_CuotaFija", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("v02_SE_TarifaMes", "TVigSubEmpMensual.Subs_al_empleo{v02_BaseMensual_ConEsp}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiferencial_ISPT", "v02_ISPT_TarifaMes - v01_ISPT_TarifaMes", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vDiferencial_SE", "v02_SE_TarifaMes - v01_SE_TarifaMes", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_Corresponde_M", "IIF(vNoCalcular = 1 , 0 , v01_ISPT_TarifaMes - vISPT_Correspondido_M)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vSE_Corresponde_M", "IIF(vNoCalcular = 1 , 0 , v01_SE_TarifaMes + vSE_Correspondido_M)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_CorrespondeParcial", "vISPT_Corresponde_M / vDiasCompletarBase * vDiasTarifaISPT", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vSE_CorrespondeParcial", "vSE_Corresponde_M / vDiasCompletarBase * vDiasTarifaISPT", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_CORRESPONDEmz_P", "vISPT_CorrespondeParcial + vDiferencial_ISPT", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vSE_CORRESPONDEmz_P", "vSE_CorrespondeParcial + vDiferencial_SE", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vISPT_RETENERmz_P", "IIF(vISPT_CORRESPONDEmz_P > vSE_CORRESPONDEmz_P , vISPT_CORRESPONDEmz_P - vSE_CORRESPONDEmz_P , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vSE_ENTREGARmz_P", "IIF(vSE_CORRESPONDEmz_P > vISPT_CORRESPONDEmz_P , vSE_CORRESPONDEmz_P - vISPT_CORRESPONDEmz_P , 0)", ref primitiveElements);
        }

        private void LoanesCredits(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("vRetencionFONACOTPeriodo", "RetencionFONACOTPeriodo()", ref primitiveElements);
        }

        private void MonthFixedSubsidy(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("VAjuste1_LimiteInferior", "TVigISRMensual.Limite_inferior{vBaseISPT}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste2_Sub_CuotaFija", "TVigISRMensual.Cuota_fija{vBaseISPT}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste3_Porcentaje", "TVigISRMensual.Porcentaje{vBaseISPT} / 100", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste4_ISRDirecto", "((vBaseISPT - VAjuste1_LimiteInferior) * VAjuste3_Porcentaje) + VAjuste2_Sub_CuotaFija", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste5_SubsidioCausado", "TVigSubEmpMensual.Subs_al_empleo{vBaseSE}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste6_ISRDirectoCorresp", "VAjuste4_ISRDirecto / vDiasTarifa_M * vDiasTarifaISR", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste7_SubsidioCausadoCorresp", "VAjuste5_SubsidioCausado / vDiasTarifa_M * vDiasTarifaSE", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste8_BaseGravadaMes", "vBaseTotal_M + vBaseTotal_P", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste9_Subs_Causado_Mes", "IIF(Periodo[Fin mes] = 0 , 0 , TVigSubEmpMensual.Subs_al_empleo{VAjuste8_BaseGravadaMes})", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste10_SubsCausaCorrespDef", "IIF(TipoRegimenEmpleado() = 2 , IIF(Periodo[Fin mes] = 1 , IIF((AcumuladoMes[Subs al Empleo  Acreditado] * - 1) > VAjuste9_Subs_Causado_Mes , 0 , MIN(VAjuste7_SubsidioCausadoCorresp , VAjuste9_Subs_Causado_Mes -(AcumuladoMes[Subs al Empleo  Acreditado] * - 1))), VAjuste7_SubsidioCausadoCorresp), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste11_ISR_RETENER", "IIF(vBaseTotal_P <=(SalarioMinimo * vDiasTarifaISR), 0 , MAX(VAjuste6_ISRDirectoCorresp - VAjuste10_SubsCausaCorrespDef , 0))", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste12_SUBSIDIO_ENTREGAR", "MAX(VAjuste10_SubsCausaCorrespDef - VAjuste6_ISRDirectoCorresp , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste13_LimiteInferiorMes", "TVigISRMensual.Limite_inferior{VAjuste8_BaseGravadaMes}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste14_CuotaFijaMes", "TVigISRMensual.Cuota_fija{VAjuste8_BaseGravadaMes}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste15_PorcentajeMes", "TVigISRMensual.Porcentaje{VAjuste8_BaseGravadaMes} / 100", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste16_ISRDirectoMes", "IIF(Periodo[Fin mes] = 0 , 0 ,((VAjuste8_BaseGravadaMes - VAjuste13_LimiteInferiorMes) * VAjuste15_PorcentajeMes) + VAjuste14_CuotaFijaMes)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste17_SubsidioCausadoMes", "IIF(TipoRegimenEmpleado() = 2 , IIF(Periodo[Fin mes] = 0 , 0 , TVigSubEmpMensual.Subs_al_empleo{VAjuste8_BaseGravadaMes}), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste18_ISR_RETENER_MES", "IIF(VAjuste8_BaseGravadaMes <=(SalarioMinimo * DiasTranscurridosMes()), 0 , MAX(VAjuste16_ISRDirectoMes - VAjuste17_SubsidioCausadoMes , 0))", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste19_SUBSIDIO_ENTREGAR_MES", "MAX(VAjuste17_SubsidioCausadoMes - VAjuste16_ISRDirectoMes , 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste20_SUBSIDIO_ENTREGAR_DEF", "IIF(Periodo[Fin mes] = 1 , MIN(MAX(0 , VAjuste12_SUBSIDIO_ENTREGAR), MAX(0 , VAjuste19_SUBSIDIO_ENTREGAR_MES -(AcumuladoMes[Subsidio al empleo Entregado] * - 1))), VAjuste12_SUBSIDIO_ENTREGAR)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste21_AcumuladoMes_SubsidioCausado", "(AcumuladoMes[Subs al Empleo  Acreditado] * - 1) + VAjuste10_SubsCausaCorrespDef", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste22_AcumuladoMes_ISRRetenido", "AcumuladoMes[ISR retenido mes] + VAjuste11_ISR_RETENER", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste23_AcumuladoMes_SubsidioEntregado", "(AcumuladoMes[Subsidio al empleo Entregado] * - 1) + VAjuste20_SUBSIDIO_ENTREGAR_DEF", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste24_SubsCausado_deMas", "IIF(Periodo[Fin mes] = 1 , MAX(0 , VAjuste21_AcumuladoMes_SubsidioCausado - VAjuste17_SubsidioCausadoMes), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste25_SubsCausado_deMenos", "IIF(Periodo[Fin mes] = 1 ,(MIN(0 , VAjuste21_AcumuladoMes_SubsidioCausado - VAjuste17_SubsidioCausadoMes) * VAjusta_SubsCausado_Menos), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste26_SubsEntregado_deMas", "IIF(Periodo[Fin mes] = 1 , MAX(0 , VAjuste23_AcumuladoMes_SubsidioEntregado - VAjuste19_SUBSIDIO_ENTREGAR_MES), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste27_SubsEntregado_no_correspondia", "IIF(VAjusta_Netear_SubsEntregado = 0 , MIN(VAjuste24_SubsCausado_deMas , VAjuste26_SubsEntregado_deMas), IIF(VAjuste24_SubsCausado_deMas < 0.01 , 0 , VAjuste26_SubsEntregado_deMas))", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste28_SubsEntregado_deMenos", "IIF(Periodo[Fin mes] = 1 ,(MIN(0 , VAjuste23_AcumuladoMes_SubsidioEntregado - VAjuste19_SUBSIDIO_ENTREGAR_MES) * VAjusta_SubsEntregado_Menos), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste29_ISR_ajuste_mensual", "IIF(Periodo[Fin mes] = 1 , MAX(0 , VAjuste24_SubsCausado_deMas - VAjuste26_SubsEntregado_deMas), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste30_ISR_ajustado_por_Subsidio", "VAjuste29_ISR_ajuste_mensual", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste31_ISR_Retenido_deMenos", "IIF(Periodo[Fin mes] = 1 , MIN(VAjuste22_AcumuladoMes_ISRRetenido - VAjuste18_ISR_RETENER_MES , 0), 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VAjuste32_ISR_Retenido_deMas", "IIF(Periodo[Fin mes] = 1 ,(MAX(VAjuste22_AcumuladoMes_ISRRetenido + VAjuste29_ISR_ajuste_mensual - VAjuste18_ISR_RETENER_MES , 0) * VAjusta_ISRRetenido_Mas), 0)", ref primitiveElements);
        }

        private void INFONAVIT(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("DiasINFONAVITAnterior", "VDiasIMSSAnterior - VAusentismoIMSSAnterior", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("DiasINFONAVITVigente", "VDiasIMSSVigente - VAusentismoIMSSVigente + VDiasXVacaciones - VDiasXFinVacaciones", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("TopeINFONAVIT", "TopesSGDF.Cesantia_y_vejez_6{Periodo[Fecha fin]} * UMA", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaINFONAVITAnterior", "IIF(VSBCAnterior > TopeINFONAVIT , TopeINFONAVIT , VSBCAnterior)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("BaseDiariaINFONAVITVigente", "IIF(VSBCVigente > TopeINFONAVIT , TopeINFONAVIT , VSBCVigente)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("CuotaINFONAVIT", "DiasINFONAVITAnterior * BaseDiariaINFONAVITAnterior * 0.05 + DiasINFONAVITVigente * BaseDiariaINFONAVITVigente * 0.05", ref primitiveElements);
        }

        private void Misc(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("Invalidez_y_Vida", "IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones() = 0 , 0 , CuotaPatronInvalidezyVida5)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Cesantia_y_Vejez", "IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones() = 0 , 0 , CuotaPatronCesantia6)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Enf_y_Mat_Patron", "IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones() = 0 , 0 , CuotaPatronEG1 + CuotaPatronEG2 + CuotaPatronEG3SMGDF + CuotaPatronEG4)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Dos_Porciento_Fondo_retiro_SAR_8", "IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones() = 0), 0 , CuotaRetiro8)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Dos_Porciento_Impuesto_estatal", "Acumulado[ISR Total de percepciones] * 0.02", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Riesgo_de_trabajo_9", "IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones() = 0), 0 , CuotaRT9)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Uno_Porciento_Educacion_empresa", "0", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("IMSS_empresa", "IIF(VPeriodoDeVacaciones = 1 Y VPagoVacaciones() = 0 , 0 , CuotaPatronEG1 + CuotaPatronEG2 + CuotaPatronEG3SMGDF + CuotaPatronEG4 + CuotaPatronInvalidezyVida5 + CuotaPatronCesantia6)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Infonavit_empresa", "IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones() = 0), 0 , CuotaINFONAVIT)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Guarderia_IMSS_7", "IIF((VPeriodoDeVacaciones = 1 * VPagoVacaciones() = 0), 0 , CuotaGuarderias7)", ref primitiveElements);
        }

        private void Perceptions(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("Sueldo", "IIF(VPeriodoDeVacaciones , 0 ,(VDiasDerechoSueldoAnterior * VSalDiarioAnt) +(VDiasDerechoSueldoVigente * VSalDiarioVigente) - VImpRetardos) + VPago_SueldoFin +(DiasDescansoVacPeriodoCompleto() * VSalDiarioVigente)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Séptimo día", "VSeptimos_Dias", ref primitiveElements);
        }

        /// <summary>
        /// Deductions
        /// </summary>
        private void Deductions(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("I.M.S.S.", "IIF((VPeriodoDeVacaciones = 1 Y VPagoVacaciones() = 0)O(vTotalPercepciones <= 0), 0 , CuotaObreroEG1 + CuotaObreroEG3SMGDF + CuotaObreroEG4 + CuotaObreroInvalidezYVida5 + CuotaObreroCesantia6)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("I.S.R. (mes)", "VAjuste11_ISR_RETENER", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Ajuste al neto", "0", ref primitiveElements);            
        }

        private void ExtraHours(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("Porc_ExentoAnt", "IIF(VSalDiarioAnt <= SalarioMinimoZonadelEmpleado , 1.0 , 0.5)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("Porc_ExentoVig", "IIF(VSalDiarioVigente <= SalarioMinimoZonadelEmpleado , 1.0 , 0.5)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("TopeSemanal", "INT(VDiasdePeriodo / 7) * UMA * 5.0", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("vOpcionHorasExtras", "1", ref primitiveElements);

            //Faltan las HE_ExentasAnt y HE_ExentasVig
            PrepareArgumentWithFormulaFunction("HE_ExentasVig", "HE_ExentasVigente(3 , 3)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("HE_ExentasAnt", "HE_ExentasAnterior(3 , 3)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("HE_Exentas", "(HE_ExentasAnt * 2 * VSalarioXhoraAnt * Porc_ExentoAnt) +(HE_ExentasVig * VSalarioXhoraVig * 2 * Porc_ExentoVig)", ref primitiveElements);
        }

        private void Settlement(ref List<PrimitiveElement> primitiveElements)
        {
            PrepareArgumentWithFormulaFunction("VAntiguedad", "AntiguedadEmpleado()", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFinAnosISR", "ROUNDTO(VAntiguedad, 0)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VCuotaDiariaParaFin", "IIF(Empleado[Salario LFT para finiquito] > 0.1 , Empleado[Salario LFT para finiquito], VSalarioPrestaciones)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("V20_Dias_Indenmizacion", "Finiquito.Indem20{1}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VIndenmizacionTotal_20", "V20_Dias_Indenmizacion * VCuotaDiariaParaFin", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFecha_Alta", "Empleado[Fecha Alta]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFecha_Baja", "Empleado[Fecha Baja]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VFecha_Inicio_Periodo", "Periodo[Fecha inicio]", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSal_Diario_Normal", "VSalDiarioVigente", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSMGDF", "SalarioMinimoDF", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSMGZ", "SalarioMinimoZonadelEmpleado", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDias90_Indenmizacion", "Finiquito.Indem90{1}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VIndenmizacion_90", "VCuotaDiariaParaFin * VDias90_Indenmizacion", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDias_Prima_Antiguedad", "Finiquito.PrimaAntig{1}", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VSal_Para_Prima_Ant", "MIN(2 * SalarioMinimo , VCuotaDiariaParaFin)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VPago_Prima_Anti", "VSal_Para_Prima_Ant * VDias_Prima_Antiguedad", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VDiasSeptimosFin", "VdiasDerechoSueldoFin2 *(1 / 6)", ref primitiveElements);
            PrepareArgumentWithFormulaFunction("VPagosSeptimosFin", "VDiasSeptimosFin * VSalDiarioVigente * IIF(TipoProceso() = 70 , 1 , 0)", ref primitiveElements);
        }

        /// <summary>
        /// Initializate elements to do the calculation
        /// </summary>
        private void Initializate()
        {
            _primitiveElements = new List<PrimitiveElement>();
            instancePrimitiveElements(ref _primitiveElements);
        }

        /// <summary>
        /// Instanciate the Arguments and Functions necesaries for the calculation
        /// </summary>
        /// <param name="functionParams"></param>
        /// <returns></returns>
        private void instancePrimitiveElements(ref List<PrimitiveElement> primitiveElements)
        {
            ProgramaticFunctions(ref primitiveElements);
            Functions(ref primitiveElements);

            if (FunctionParamsProperty.CalculationBaseResult.Overdraft != null)
            {
                //Catalogs
                Catalogs(ref primitiveElements);

                //Generals
                WithoutCategory(ref primitiveElements);
                GeneralsIMSS(ref primitiveElements);
                Vacations(ref primitiveElements);

                //Tablas IMSS
                EG1Calculation(ref primitiveElements);
                EG2Calculation(ref primitiveElements);
                EG3Calculation(ref primitiveElements);
                EG4Calculation(ref primitiveElements);
                Table5Calculation(ref primitiveElements);
                Table6Calculation(ref primitiveElements);
                Table7Calculation(ref primitiveElements);
                Table8Calculation(ref primitiveElements);

                //IMSSWorkRisk
                IMSSWorkRisk(ref primitiveElements);

                //ISPT_Art142
                ISPT_Art142(ref primitiveElements);

                //ISR_Settlement
                ISR_Settlement(ref primitiveElements);

                //ISPT GENERALES 2010
                ISPT_Generals(ref primitiveElements);

                //ISPT_Mensual sp 2010
                ISPT_Monthly(ref primitiveElements);

                //ISPT Mensualizada 2010
                ISPT_Monthlylized(ref primitiveElements);

                //Préstamos y Créditos
                LoanesCredits(ref primitiveElements);

                //Ajuste Mes Subsidio
                MonthFixedSubsidy(ref primitiveElements);

                //INFONAVIT
                INFONAVIT(ref primitiveElements);

                //Misc
                Misc(ref primitiveElements);

                //Perceptions
                Perceptions(ref primitiveElements);

                //Deductions
                Deductions(ref primitiveElements);

                //Tiempo extra
                ExtraHours(ref primitiveElements);

                //Finiquito
                Settlement(ref primitiveElements);
            }
        }

        /// <summary>
        /// Fix the formula necesary for the converter
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        private string FixFormula(string formula)
        {
            //Formulas
            //Replace reservedWords
            formula = replaceIfItsReservedWord(formula);

            //Fix formula - Keywords         
            formula = formula.Replace("IIF", "if").Replace("iif", "if");
            //Fix formula - MAX         
            formula = formula.Replace("max", "MAX");
            //Fix formula - MAX         
            formula = formula.Replace("min", "MIN");
            //Fix formula - MAX         
            formula = formula.Replace("int(", "INT(");
            //When the formula recive empty parameters
            formula = formula.Replace("_0", "0");
            //ú, á, é, í, ó
            formula = formula.Replace("á", "a").Replace("ú", "u").Replace("é", "e").Replace("í", "i").Replace("ó", "o");
            //Ú, Á, É, Í, Ó
            formula = formula.Replace("Á", "A").Replace("Ú", "U").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O");
            //Replace ()
            formula = formula.Replace("()", "(0)");
            //Replace $
            formula = formula.Replace(" $", " _S");
            //Replace  Y 
            formula = formula.Replace(" Y ", " & ");
            //Replace )Y(
            formula = formula.Replace(")Y(", ")&(");
            //Replace ) Y(
            formula = formula.Replace(") Y(", ") &(");
            //Replace )Y (
            formula = formula.Replace(")Y (", ")& (");
            //Replace ) Y (
            formula = formula.Replace(") Y (", ") & (");
            //Replace ) O(
            formula = formula.Replace(") O(", ") |(");
            //Replace )O (
            formula = formula.Replace(")O (", ")| (");
            //Replace )O(
            formula = formula.Replace(")O(", ")|(");
            //Replace ) O (
            formula = formula.Replace(") O (", ") | (");
            //Replace  O
            formula = formula.Replace(" O ", " | ");

            //Replace branches '{' y '}'
            formula = formula.Replace("{", "(").Replace("}", ")");
            //Replace [ ]
            while (formula.Contains("[") && formula.Contains("]"))
            {
                int from = formula.IndexOf("[") + 1;
                int to = formula.IndexOf("]");
                var oldString = formula.Substring(from, to - from);
                var newString = oldString.Replace(" ", "_s_");
                formula = formula.Replace($"[{oldString}]", $"_bs_{newString}_bf_");
            }
            //'Char' for ASCII CODE
            while (formula.Contains("'"))
            {
                int from = formula.IndexOf("'") + 1;
                int to = formula.IndexOf("'", from);
                var oldString = formula.Substring(from, to - from);
                var newString = (double)oldString[0];
                formula = formula.Replace($"'{oldString}'", $"{newString}");
            }
            //Replace "." between letters
            for (int i = 0; i < formula.Length; i++)
            {
                if (formula[i] == '.')
                {
                    //check 
                    if (i - 1 >= 0 && !Char.IsNumber(formula[i - 1]))
                    {
                        formula = formula.Remove(i, 1).Insert(i, "_p_");
                    }
                    else if (i + 1 < formula.Length && !Char.IsNumber(formula[i + 1]))
                    {
                        formula = formula.Remove(i, 1).Insert(i, "_p_");
                    }
                }
            }
          
            return formula;
        }

        /// <summary>
        /// Fix argument inversa - mode
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private string FixReverseArgument(string argument)
        {
            var argumentFixed = argument;
            if (!String.IsNullOrEmpty(argument))
            {
                argumentFixed = argumentFixed.Replace("_p_", ".");
                argumentFixed = argumentFixed.Replace("_s_", " ");
                argumentFixed = argumentFixed.Replace("_bs_", "[");
                argumentFixed = argumentFixed.Replace("_bf_", "]");
                argumentFixed = argumentFixed.Replace("_perc_", "%");
                argumentFixed = argumentFixed.Replace("&", " Y ");
                argumentFixed = argumentFixed.Replace("|", " O ");
                argumentFixed = argumentFixed.Replace("_S", "$");
            }

            return argumentFixed;
        }

        /// <summary>
        /// Tokenize expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private string[] TokenizeExpression(string expression)
        {
            if (String.IsNullOrEmpty(expression))
            {
                return new string[] { };
            }

            var splited = expression
                           .Replace("if(", " ")
                           .Replace("iff(", " ")
                           .Replace("IF(", " ")
                           .Replace("IFF(", " ")
                           .Replace("if (", " ")
                           .Replace("iff (", " ")
                           .Replace("IF (", " ")
                           .Replace("IFF (", " ")
                           .Replace("_0", " ")
                           .Replace("(0)", " ")
                           .Replace(")", " ")
                           .Replace("(", " ")
                           .Replace("+", " ")
                           .Replace("-", " ")
                           .Replace(",", " ")
                           .Replace("{", " ")
                           .Replace("}", " ")
                           .Split(" ").Distinct().ToArray();

            return splited;
        }

        /// <summary>
        /// Include details
        /// </summary>
        /// <param name="calculateResult"></param>
        /// <param name="expression"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        private CalculateResult includeDetails(CalculateResult calculateResult, Expression expression, string formula)
        {
            ConcurrentBag<CalculateArgument> calculateArguments = new ConcurrentBag<CalculateArgument>();
            //Calculate Arguments
            Parallel.ForEach(expression.argumentsList, argument =>
            {
                var valueTemp = 0M;
                if (!double.IsNaN(argument.argumentValue))
                {
                    valueTemp = Convert.ToDecimal(argument.argumentValue);
                }
                calculateArguments.Add(new CalculateArgument()
                {
                    Argument = FixReverseArgument(argument.argumentName),
                    Expression = FixReverseArgument(argument.functionExpressionString),
                    Value = valueTemp
                });
            });

            Parallel.ForEach(expression.functionsList, function =>
            {
                var valueTemp = 0M;
                if (!double.IsNaN(function.lastResult))
                {
                    valueTemp = Convert.ToDecimal(function.lastResult);
                }
                calculateArguments.Add(new CalculateArgument()
                {
                    Argument = FixReverseArgument(function.functionName),
                    Expression = FixReverseArgument(function.functionExpressionString),
                    Value = valueTemp
                });
            });

            calculateResult.CalculateArguments = calculateArguments.ToList();

            //Tokenize
            calculateResult.ResultText = $"{formula}";
            var splited = TokenizeExpression(calculateResult.ResultText);

            //si es una sola fórmula encontrar si tiene Expression tokenizarla
            if (splited.Length == 1)
            {
                var expressionFound = calculateResult.CalculateArguments.FirstOrDefault(p => FixFormula(p.Argument) == splited[0]);
                if (null != expressionFound)
                {
                    calculateResult.ResultText = expressionFound.Expression;
                }
                splited = TokenizeExpression(calculateResult.ResultText);
            }

            if (splited.Any())
            {
                for (int i = 0; i < splited.Length; i++)
                {
                    if (!String.IsNullOrEmpty(splited[i]) && splited[i] != " ")
                    {
                        var calculateargument = calculateResult.CalculateArguments.FirstOrDefault(p => FixFormula(p.Argument) == splited[i]);
                        if (calculateargument != null)
                        {
                            var newArgumentFixed = FixReverseArgument(splited[i]);
                            calculateResult.ResultText = calculateResult.ResultText.Replace(splited[i], $"{newArgumentFixed}<<{calculateargument.Value}>>");
                        }
                    }
                }
            }
            else 
            {
                calculateResult.ResultText = $"{formula}<<{calculateResult.Result}>>";
            }

            calculateResult.ResultText = FixReverseArgument(calculateResult.ResultText);

            return calculateResult;
        }

        /// <summary>
        /// Do the calculation from formula specified
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public CalculateResult Calculate(string formula, bool includeDetail = false)
        {
            var calculateResult = new CalculateResult();
            decimal ToReturn = 0M;
            var formulaFixed = FixFormula(formula);

            //Expression
            Expression expression = new Expression(formulaFixed, _primitiveElements.Clone().ToArray());
            expression.setSilentMode();          
            expression.setRecursiveMode();

            //Do the calculation
            var calcResult = expression.calculate();

            if (!double.IsNaN(calcResult))
            {
                ToReturn = Convert.ToDecimal(calcResult);
            }

            var errorMessage = expression.getErrorMessage();
            if (!errorMessage.Contains("no errors"))
            {
                Trace.WriteLine($"36001 - Calculation Exception formula:{formula}, message:{errorMessage}");
                if (errorMessage.ToLower().Contains("duplicate"))
                {
                    throw new CotorraException(36001, "36001", "Were one or more keywords duplicated.",
                        new Exception(expression.getErrorMessage()));
                }
            }

            //Set Result decimal
            calculateResult.Result = ToReturn;

            if (includeDetail)
            {
                calculateResult = includeDetails(calculateResult, expression, formula);
            }

            return calculateResult;
        }

    }
}

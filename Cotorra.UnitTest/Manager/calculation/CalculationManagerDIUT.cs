using AutoMapper;
using MoreLinq;
using Newtonsoft.Json;
using Cotorra.Core;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cotorra.UnitTest
{
    public class CalculationManagerDIUT
    {
        public class InternalConceptCheck
        {
            public InternalConceptCheck()
            {
            }

            public InternalConceptCheck(ConceptType conceptType)
            {
                ConceptPaymentDI = new ConceptPaymentDI();
                ConceptPaymentDI.ConceptType = conceptType;
            }

            public InternalConceptCheck(ConceptType conceptType, string formulaTotal)
            {
                ConceptPaymentDI = new ConceptPaymentDI();
                ConceptPaymentDI.ConceptType = conceptType;
                ConceptPaymentDI.FormulaTotal = formulaTotal;
            }

            public InternalConceptCheck(ConceptType conceptType, string formulaValue, string formulaTotal)
            {
                ConceptPaymentDI = new ConceptPaymentDI();
                ConceptPaymentDI.ConceptType = conceptType;
                ConceptPaymentDI.FormulaTotal = formulaTotal;
                ConceptPaymentDI.FormulaValue = formulaValue;
            }

            public ConceptPaymentDI ConceptPaymentDI { get; set; }

            public decimal ExpectedResult { get; set; }
        }

        public List<OverdraftDetailDI> GetDefaultDetails(Guid company, Guid instanceID, Guid userID,
            CalculateOverdraftDIParams calculateOverdraftDIParams)
        {
            var details = new List<OverdraftDetailDI>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ConceptPayment, ConceptPaymentDI>();
            });

            var _mapper = config.CreateMapper();

            var memoryManager = new MemoryStorageContext();
            var accumulatedsTypes = memoryManager.GetDefaultAccumulatedType(company, instanceID, userID);
            (var concepts, var lstRelationships) = memoryManager.GetDefaultConcept<ConceptPayment>(company, instanceID, userID, accumulatedsTypes);
            concepts.Where(p => p.GlobalAutomatic).ToList().ForEach(concept =>
            {
                var overdrafDetailDI = new OverdraftDetailDI();
                overdrafDetailDI.ConceptPaymentDI = _mapper.Map<ConceptPayment, ConceptPaymentDI>(concept as ConceptPayment);
                details.Add(overdrafDetailDI);
            });

            return details;
        }

        [Fact]
        public async Task TM_CalculateByOverdraftDI()
        {
            Guid company = Guid.NewGuid();
            Guid instanceID = Guid.NewGuid();
            Guid userID = Guid.NewGuid();

            //Fill unnecesary data
            var overdraftDIManager = new OverdraftCalculationDIManager();
            var calculateOverdraftDIParams = new CalculateOverdraftDIParams();
            calculateOverdraftDIParams.IdentityWorkID = company;
            calculateOverdraftDIParams.InstanceID = instanceID;
            calculateOverdraftDIParams.UserID = userID;
            calculateOverdraftDIParams.ResetCalculation = true;

            //Fill data necesary for the calculation
            calculateOverdraftDIParams.OverdraftDI.OverdraftStatus = OverdraftStatus.None;
            calculateOverdraftDIParams.OverdraftDI.OverdraftType = OverdraftType.Ordinary;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.FullName = "Hector Omar Ramirez Mendez";
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.ContributionBase = BaseQuotation.Fixed;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.DailySalary = 372M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SalaryZone = SalaryZone.ZoneA;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCFixedPart = 390.34M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCVariablePart = 0;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCMax25UMA = 390.34M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SettlementSalaryBase = 0M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.WorkshiftDI.Hours = 8.0;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.InitialDate = new DateTime(2020, 5, 1);
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.FinalDate = new DateTime(2020, 5, 15);
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PaymentDays = 15M;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodBimonthlyIMSS = PeriodBimonthlyIMSS.Initial;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodFiscalYear = PeriodFiscalYear.None;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodMonth = PeriodMonth.Initial;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.SeventhDayPosition = "-1";
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.SeventhDays = 0;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodDI.FiscalYear = 2020;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodDI.FortnightPaymentDays = AdjustmentPay_16Days_Febrary.PayPaymentDays;

            //Add concepts  / details
            var concepts = new Dictionary<int, InternalConceptCheck>()
            {
                //Percepciones
                //Sueldo
                {
                    1, new InternalConceptCheck(ConceptType.SalaryPayment, 
                            formulaTotal: "IIF(VPeriodoDeVacaciones , 0 ,(VDiasDerechoSueldoAnterior * VSalDiarioAnt) +(VDiasDerechoSueldoVigente * VSalDiarioVigente) - VImpRetardos) + VPago_SueldoFin +(DiasDescansoVacPeriodoCompleto(_0) * VSalDiarioVigente) + 20")
                        {
                            ExpectedResult = 5580M + 20M
                        }
                },
                //Séptimo día
                {
                    3, new InternalConceptCheck(ConceptType.SalaryPayment) 
                        { 
                            ExpectedResult = 0M 
                        }
                },

                //Deducciones
                //RET. INV. Y VIDA
                {
                    5, new InternalConceptCheck(ConceptType.DeductionPayment) 
                        { 
                            ExpectedResult = 36.59M 
                        }
                },
                //RET. CESANTIA
                {
                    6, new InternalConceptCheck(ConceptType.DeductionPayment) 
                        { 
                            ExpectedResult = 65.87M 
                        }
                },
            };
            //Fill
            concepts.ForEach(concept =>
            {
                var overdrafDetailDI = new OverdraftDetailDI();
                overdrafDetailDI.ConceptPaymentDI.Code = concept.Key;
                overdrafDetailDI.ConceptPaymentDI.ConceptType = concept.Value.ConceptPaymentDI.ConceptType;
                if (!String.IsNullOrEmpty(concept.Value.ConceptPaymentDI.FormulaTotal))
                {
                    overdrafDetailDI.ConceptPaymentDI.FormulaTotal = concept.Value.ConceptPaymentDI.FormulaTotal;
                }

                //add to list
                calculateOverdraftDIParams.OverdraftDI.OverdraftDetailDIs.Add(overdrafDetailDI);
            });

            var json = JsonConvert.SerializeObject(calculateOverdraftDIParams);

            //Do the calculation
            var response = await overdraftDIManager.CalculateAsync(calculateOverdraftDIParams) as CalculateOverdraftResult;

            //Asserts
            Assert.True(response.OverdraftResult != null);

            //Multiple Validations
            concepts.ForEach(concept =>
            {
                var conceptApplied = response.OverdraftResult.OverdraftDetails.FirstOrDefault(p =>
                p.ConceptPayment.Code == concept.Key &&
                p.ConceptPayment.ConceptType == concept.Value.ConceptPaymentDI.ConceptType).Amount;
                Assert.True(conceptApplied == concept.Value.ExpectedResult);
            });
        }

        [Fact]
        public async Task TM_CalculateByOverdraftDIWithoutConcepts()
        {
            Guid company = Guid.NewGuid();
            Guid instanceID = Guid.NewGuid();
            Guid userID = Guid.NewGuid();

            //Fill unnecesary data
            var overdraftDIManager = new OverdraftCalculationDIManager();
            var calculateOverdraftDIParams = new CalculateOverdraftDIParams();
            calculateOverdraftDIParams.IdentityWorkID = company;
            calculateOverdraftDIParams.InstanceID = instanceID;
            calculateOverdraftDIParams.UserID = userID;
            calculateOverdraftDIParams.ResetCalculation = true;

            //Fill data necesary for the calculation
            calculateOverdraftDIParams.OverdraftDI.OverdraftStatus = OverdraftStatus.None;
            calculateOverdraftDIParams.OverdraftDI.OverdraftType = OverdraftType.Ordinary;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.FullName = "Hector Omar Ramirez Mendez";
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.ContributionBase = BaseQuotation.Fixed;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.DailySalary = 372M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SalaryZone = SalaryZone.ZoneA;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCFixedPart = 390.34M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCVariablePart = 0;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCMax25UMA = 390.34M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SettlementSalaryBase = 0M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.WorkshiftDI.Hours = 8.0;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.InitialDate = new DateTime(2020, 5, 1);
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.FinalDate = new DateTime(2020, 5, 15);
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PaymentDays = 15M;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodBimonthlyIMSS = PeriodBimonthlyIMSS.Initial;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodFiscalYear = PeriodFiscalYear.None;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodMonth = PeriodMonth.Initial;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.SeventhDayPosition = "-1";
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.SeventhDays = 0;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodDI.FiscalYear = 2020;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodDI.FortnightPaymentDays = AdjustmentPay_16Days_Febrary.PayPaymentDays;

            //Add concepts  / details
            var concepts = new Dictionary<int, InternalConceptCheck>()
            {
                //Percepciones
                //Sueldo
                {
                    1, new InternalConceptCheck(ConceptType.SalaryPayment)
                        {
                            ExpectedResult = 5580M
                        }
                },
                //Séptimo día
                {
                    3, new InternalConceptCheck(ConceptType.SalaryPayment)
                        {
                            ExpectedResult = 0M
                        }
                },

                //Deducciones
                //RET. INV. Y VIDA
                {
                    5, new InternalConceptCheck(ConceptType.DeductionPayment)
                        {
                            ExpectedResult = 36.59M
                        }
                },
                //RET. CESANTIA
                {
                    6, new InternalConceptCheck(ConceptType.DeductionPayment)
                        {
                            ExpectedResult = 65.87M
                        }
                },
            };
            //Fill
            calculateOverdraftDIParams.OverdraftDI.OverdraftDetailDIs = new List<OverdraftDetailDI>();

            var json = JsonConvert.SerializeObject(calculateOverdraftDIParams);

            //Do the calculation
            var response = await overdraftDIManager.CalculateAsync(calculateOverdraftDIParams) as CalculateOverdraftResult;

            //Asserts
            Assert.True(response.OverdraftResult != null);

            //Multiple Validations
            concepts.ForEach(concept =>
            {
                var conceptApplied = response.OverdraftResult.OverdraftDetails.FirstOrDefault(p =>
                p.ConceptPayment.Code == concept.Key &&
                p.ConceptPayment.ConceptType == concept.Value.ConceptPaymentDI.ConceptType).Amount;
                Assert.True(conceptApplied == concept.Value.ExpectedResult);
            });
        }

        [Fact]
        public async Task TM_CalculateByOverdraftDIByFormula()
        {
            Guid company = Guid.NewGuid();
            Guid instanceID = Guid.NewGuid();
            Guid userID = Guid.NewGuid();

            //Fill unnecesary data
            var overdraftDIManager = new OverdraftCalculationDIManager();
            var calculateOverdraftDIParams = new CalculateOverdraftDIParams();
            calculateOverdraftDIParams.IdentityWorkID = company;
            calculateOverdraftDIParams.InstanceID = instanceID;
            calculateOverdraftDIParams.UserID = userID;
            calculateOverdraftDIParams.ResetCalculation = true;

            //Fill data necesary for the calculation
            calculateOverdraftDIParams.OverdraftDI.OverdraftStatus = OverdraftStatus.None;
            calculateOverdraftDIParams.OverdraftDI.OverdraftType = OverdraftType.Ordinary;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.FullName = "Hector Omar Ramirez Mendez";
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.ContributionBase = BaseQuotation.Fixed;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.DailySalary = 372M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SalaryZone = SalaryZone.ZoneA;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCFixedPart = 390.34M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCVariablePart = 0;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SBCMax25UMA = 390.34M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.SettlementSalaryBase = 0M;
            calculateOverdraftDIParams.OverdraftDI.EmployeeDI.WorkshiftDI.Hours = 8.0;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.InitialDate = new DateTime(2020, 5, 1);
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.FinalDate = new DateTime(2020, 5, 15);
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PaymentDays = 15M;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodBimonthlyIMSS = PeriodBimonthlyIMSS.Initial;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodFiscalYear = PeriodFiscalYear.None;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodMonth = PeriodMonth.Initial;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.SeventhDayPosition = "-1";
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.SeventhDays = 0;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodDI.FiscalYear = 2020;
            calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodDI.FortnightPaymentDays = AdjustmentPay_16Days_Febrary.PayPaymentDays;

            //Add concepts  / details
            var concepts = new Dictionary<int, InternalConceptCheck>()
            {
                //Percepciones
                //Sueldo
                {
                    1, new InternalConceptCheck(ConceptType.SalaryPayment)
                },
                //Séptimo día
                {
                    3, new InternalConceptCheck(ConceptType.SalaryPayment)
                },

                //Deducciones
                //RET. INV. Y VIDA
                {
                    5, new InternalConceptCheck(ConceptType.DeductionPayment)
                },
                //RET. CESANTIA
                {
                    6, new InternalConceptCheck(ConceptType.DeductionPayment)
                },
            };
            //Fill
            concepts.ForEach(concept =>
            {
                var overdrafDetailDI = new OverdraftDetailDI();
                overdrafDetailDI.ConceptPaymentDI.Code = concept.Key;
                overdrafDetailDI.ConceptPaymentDI.ConceptType = concept.Value.ConceptPaymentDI.ConceptType;
                if (!String.IsNullOrEmpty(concept.Value.ConceptPaymentDI.FormulaTotal))
                {
                    overdrafDetailDI.ConceptPaymentDI.FormulaTotal = concept.Value.ConceptPaymentDI.FormulaTotal;
                }

                //add to list
                calculateOverdraftDIParams.OverdraftDI.OverdraftDetailDIs.Add(overdrafDetailDI);
            });

            //Asserts
            var response = await overdraftDIManager.CalculateFormula(calculateOverdraftDIParams, "VDiasIMSSVigente");
            Assert.True(response.Result == 15);

            var salary = await overdraftDIManager.CalculateFormula(calculateOverdraftDIParams, "Sueldo + 20");
            Assert.True(salary.Result == 5600M);
        }
    }
}

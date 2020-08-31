
using CotorraNode.Common.Config;
using CotorraNode.Layer2.Company.Client;
using CotorraNube.CommonApp.Client;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Cotorra.Core
{
    public class InitializationManager
    {
        private InstanceClient instanceClient;

        public InitializationManager(InstanceClient instancecient)
        {
            instanceClient = instancecient;
        }

        public InitializationManager()
        {
            instanceClient = new InstanceClient(ConfigManager.GetValue("companyhost"));
        }

        public async Task<InitializationResult> InitializeAsync(InitializationParams parameters)
        {
            Stack<Action> rollbackActions = new Stack<Action>();
            InitializationnValidator validator = new InitializationnValidator();
            validator.BeforeProcess(parameters);
            Guid companyID;
            Guid instanceID;
            Guid licenseID;
            Guid licenseServiceID;

            try
            {
                //Create company
                var creationResult = await CreateInstanceAsync(parameters, rollbackActions);
                companyID = creationResult.Item1;
                instanceID = creationResult.Item2;
                licenseID = creationResult.Item3;
                licenseServiceID = creationResult.Item4;
                parameters.PayrollCompanyConfiguration.company = companyID;
                parameters.PayrollCompanyConfiguration.InstanceID = instanceID;

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //PayrollConfiguration
                    await SavePayrollCompanyConfigurationAsync(parameters, companyID);

                    /***Initialize fares and tables catalogs***/

                    //MinimiumSalary
                    await SaveMinimiumSalaryDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //Benefit Type
                    await SaveBenefitTypeDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //IncomeTax
                    await SaveMonthlyIncomeTaxDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //AnualIncomeTax
                    await SaveAnualIncomeTaxDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //Employment monthly subsidy
                    await SaveMonthlyEmploymentSubsidyDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //Employment anual subsidy
                    await SaveAnualEmploymentSubsidyDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //IMSS
                    await SaveIMSSFareDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //SGDFlimits
                    await SaveSGDFLimitsDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //IMSSEmployeeTable
                    await SaveIMSSEmployeeTableDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //IMSSEmployerTable
                    await SaveIMSSEmployerTableDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //IMSSWorkRisk
                    await SaveIMSSWorkRiskDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //UMA
                    await SaveUMADefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //SettlementCatalog
                    await SaveSettlementCatalogDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //WorkShift
                    await SaveWorkshiftDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //AccumulatedType
                    var accumulated = await SaveAccumulatedTypeDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //Concepts (Salary. Liability, Deduction)
                    await SaveConceptPaymentDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user, accumulated);

                    //Employer Registration
                    await SaveEmployerRegistrationAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //IncidentType
                    var incidentTypes = await SaveIncidentTypeDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user, accumulated);

                    //Incident Type Relationship
                    await SaveIncidentTypeRelationshipDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user, incidentTypes);

                    //Period Type
                    var periodTypes = await SavePeriodTypeDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user);

                    //Period
                    await SavePeriodDefaultAsync(parameters, companyID, instanceID, parameters.PayrollCompanyConfiguration.user, periodTypes);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                rollbackActions.DoRollBack();

                if (ex is CotorraException)
                {
                    throw new CotorraException(0001, "Create Company", ex.Message, ex);
                }
                else
                {
                    throw new CotorraException(0002, "Create Company", "Ocurrio un error al crear la compañía por favor vuelve a intentarlo en unos minutos.",
                        ex);
                }
            }

            return new InitializationResult()
            {
                CompanyID = companyID,
                InstanceID = instanceID,
                LicenseID = licenseID,
                LicenseServiceID = licenseServiceID
            };
        }

        private async Task SaveMonthlyEmploymentSubsidyDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultMonthlyEmploymentSubsidy(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<MonthlyEmploymentSubsidy>(new BaseRecordManager<MonthlyEmploymentSubsidy>(),
                new MonthlyEmploymentSubsidyValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveAnualEmploymentSubsidyDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultAnualEmploymentSubsidy(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<AnualEmploymentSubsidy>(new BaseRecordManager<AnualEmploymentSubsidy>(),
                new AnualEmploymentSubsidyValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveAnualIncomeTaxDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultAnualIncomeTax(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<AnualIncomeTax>(new BaseRecordManager<AnualIncomeTax>(),
                new AnualIncomeTaxValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveSGDFLimitsDefaultAsync(InitializationParams initializationParams, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultSGDFLimits(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<SGDFLimits>(new BaseRecordManager<SGDFLimits>(),
                new SGDFLimitsValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveIMSSEmployerTableDefaultAsync(InitializationParams initializationParams, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultIMSSEmployerTable(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<IMSSEmployerTable>(new BaseRecordManager<IMSSEmployerTable>(),
                new IMSSEmployerTableValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveSettlementCatalogDefaultAsync(InitializationParams initializationParams, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultSettlementCatalogTable(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<SettlementCatalog>(new BaseRecordManager<SettlementCatalog>(),
                new SettlementCatalogValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveIMSSEmployeeTableDefaultAsync(InitializationParams initializationParams, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultIMSSEmployeeTable(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<IMSSEmployeeTable>(new BaseRecordManager<IMSSEmployeeTable>(),
                new IMSSEmployeeTableValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveIMSSWorkRiskDefaultAsync(InitializationParams initializationParams, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultIMSSWorkRisk(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<IMSSWorkRisk>(new BaseRecordManager<IMSSWorkRisk>(),
                new IMSSWorkRiskValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveIMSSFareDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultIMSSFare(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<IMSSFare>(new BaseRecordManager<IMSSFare>(),
                new IMSSFareValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SavePayrollCompanyConfigurationAsync(InitializationParams parameters, Guid companyID)
        {
            var middlewareManager = new MiddlewareManager<PayrollCompanyConfiguration>(new BaseRecordManager<PayrollCompanyConfiguration>(),
                new PayrollCompanyConfigurationValidator());

            await middlewareManager.CreateAsync(new List<PayrollCompanyConfiguration>() { parameters.PayrollCompanyConfiguration }, companyID);
        }

        private async Task SaveMinimiumSalaryDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultMinimunSalaries(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<MinimunSalary>(new BaseRecordManager<MinimunSalary>(),
                new MinimunSalaryValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveBenefitTypeDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultBenefitType(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<BenefitType>(new BaseRecordManager<BenefitType>(),
                new BenefitTypeValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveMonthlyIncomeTaxDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultMonthlyIncomeTax(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<MonthlyIncomeTax>(new BaseRecordManager<MonthlyIncomeTax>(),
                new MonthlyIncomeTaxValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveUMADefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetdDefaultUMA(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<UMA>(new BaseRecordManager<UMA>(),
                new UMAValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveWorkshiftDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultWorkShift(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(),
                new WorkshiftValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task SaveConceptPaymentDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user, List<AccumulatedType> accumulatedTypes)
        {
            //Get Memory Default Data
            var memoryTest = new MemoryStorageContext();
            var resultTuple = memoryTest.GetDefaultConcept<ConceptPayment>(companyID, instanceID, user, accumulatedTypes);

            //Results
            var resultConcepts = resultTuple.Item1.Cast<ConceptPayment>().ToList();
            var resultRelationship = resultTuple.Item2.ToList();

            //middlewareManagers
            var middlewareManager = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(),
                new ConceptPaymentValidator());

            var middlewareManagerConceptPaymentRelationship = new MiddlewareManager<ConceptPaymentRelationship>(new BaseRecordManager<ConceptPaymentRelationship>(),
               new ConceptPaymentRelationshipValidator());

            //Create ConceptPayment
            await middlewareManager.CreateAsync(resultConcepts, companyID);
            //Create ConceptPayment relationship with accumulates
            await middlewareManagerConceptPaymentRelationship.CreateAsync(resultRelationship, companyID);
        }

        private async Task<List<AccumulatedType>> SaveAccumulatedTypeDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultAccumulatedType(companyID, instanceID, user);
            var middlewareManager = new MiddlewareManager<AccumulatedType>(new BaseRecordManager<AccumulatedType>(),
                new AccumulatedTypeValidator());

            await middlewareManager.CreateAsync(result, companyID);

            return result;
        }

        private async Task<List<IncidentType>> SaveIncidentTypeDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user, List<AccumulatedType> accumulatedTypes)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultIncidentType(companyID, instanceID, user, accumulatedTypes);
            var middlewareManager = new MiddlewareManager<IncidentType>(new BaseRecordManager<IncidentType>(),
                new IncidentTypeValidator());

            await middlewareManager.CreateAsync(result, companyID);

            return result;
        }

        private async Task<List<IncidentTypeRelationship>> SaveIncidentTypeRelationshipDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user, List<IncidentType> incidentTypes)
        {
            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultIncidentTypeRelationship(companyID, instanceID, user, incidentTypes);
            var middlewareManager = new MiddlewareManager<IncidentTypeRelationship>(new BaseRecordManager<IncidentTypeRelationship>(),
                new IncidentTypeRelationshipValidator());

            await middlewareManager.CreateAsync(result, companyID);

            return result;
        }

        private async Task SaveEmployerRegistrationAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            if (parameters.EmployerRegistration != null && parameters.EmployerRegistration.ID != Guid.Empty)
            {
                parameters.EmployerRegistration.CompanyID = companyID;
                parameters.EmployerRegistration.InstanceID = instanceID;
                parameters.EmployerRegistration.company = companyID;
                parameters.EmployerRegistration.user = user;
                parameters.EmployerRegistration.Name = parameters.EmployerRegistration.Code;

                var result = new List<EmployerRegistration> { parameters.EmployerRegistration };
                var middlewareManager = new MiddlewareManager<EmployerRegistration>(new BaseRecordManager<EmployerRegistration>(),
                    new EmployerRegistrationValidator());

                await middlewareManager.CreateAsync(result, companyID);
            }
        }

        private async Task<List<PeriodType>> SavePeriodTypeDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user)
        {
            var memoryTest = new MemoryStorageContext();
            List<PeriodType> lstPeriodTypes = new List<PeriodType>();

            //Quincenal
            var forceUpdate = parameters.PayrollCompanyConfiguration.PaymentPeriodicity == PaymentPeriodicity.Biweekly;
            lstPeriodTypes.AddRange(memoryTest.GetDefaultPeriodType(companyID, instanceID, user,
                PaymentPeriodicity.Biweekly,
                parameters.PayrollCompanyConfiguration.PaymentDays,
                parameters.PayrollCompanyConfiguration.AdjustmentPay, forceUpdate ));

            //Mensual
            forceUpdate = parameters.PayrollCompanyConfiguration.PaymentPeriodicity == PaymentPeriodicity.Monthly;
            lstPeriodTypes.AddRange(memoryTest.GetDefaultPeriodType(companyID, instanceID, user,
                PaymentPeriodicity.Monthly,
                parameters.PayrollCompanyConfiguration.PaymentDays,
                parameters.PayrollCompanyConfiguration.AdjustmentPay, forceUpdate));

            //Semanal            
            forceUpdate = parameters.PayrollCompanyConfiguration.PaymentPeriodicity == PaymentPeriodicity.Weekly;
            lstPeriodTypes.AddRange(memoryTest.GetDefaultPeriodType(companyID, instanceID, user,
                PaymentPeriodicity.Weekly,
                parameters.PayrollCompanyConfiguration.PaymentDays,
                parameters.PayrollCompanyConfiguration.AdjustmentPay, forceUpdate, parameters.PayrollCompanyConfiguration.WeeklySeventhDay));

            //Extraordinario
            forceUpdate = parameters.PayrollCompanyConfiguration.PaymentPeriodicity == PaymentPeriodicity.OtherPeriodicity;
            lstPeriodTypes.AddRange(memoryTest.GetDefaultPeriodType(companyID, instanceID, user,
                PaymentPeriodicity.OtherPeriodicity,
                parameters.PayrollCompanyConfiguration.PaymentDays,
                parameters.PayrollCompanyConfiguration.AdjustmentPay, forceUpdate ));

            var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(),
                new PeriodTypeValidator());

            await middlewareManager.CreateAsync(lstPeriodTypes, companyID);

            return lstPeriodTypes;
        }

        private async Task SavePeriodDefaultAsync(InitializationParams parameters, Guid companyID, Guid instanceID, Guid user,
            List<PeriodType> periodTypes)
        {
            var memoryTest = new MemoryStorageContext();

            var result = memoryTest.GetDefaultPeriod(companyID, instanceID, user,
                parameters.PayrollCompanyConfiguration.PeriodInitialDate,
                parameters.PayrollCompanyConfiguration.PeriodInitialDate.AddYears(1).Date,
                parameters.PayrollCompanyConfiguration.CurrentExerciseYear,
                periodTypes.FirstOrDefault(p => p.PaymentPeriodicity == parameters.PayrollCompanyConfiguration.PaymentPeriodicity));

            result.AddRange(memoryTest.GetDefaultPeriod(companyID, instanceID, user,
                parameters.PayrollCompanyConfiguration.PeriodInitialDate,
                parameters.PayrollCompanyConfiguration.PeriodInitialDate.AddYears(1).Date,
                parameters.PayrollCompanyConfiguration.CurrentExerciseYear,
                periodTypes.FirstOrDefault(p => p.ExtraordinaryPeriod)));

            var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(),
                new PeriodValidator());

            await middlewareManager.CreateAsync(result, companyID);
        }

        private async Task<(Guid, Guid, Guid, Guid)> CreateInstanceAsync(InitializationParams parameters,
            Stack<Action> rollbackActions)
        {
            var companyID = Guid.Empty;
            var instanceID = Guid.Empty;
            var licenseID = Guid.Empty;
            var licenseServiceID = Guid.Empty;

            rollbackActions.Push(() => instanceClient.DeleteInstance(parameters.AuthTkn, instanceID));

            var result = await instanceClient.CreateInstanceAsync(
                       parameters.AuthTkn,
                       parameters.LicenseServiceID,
                       parameters.RFC,
                       parameters.SocialReason);

            companyID = result.CompanyID;
            instanceID = result.InstanceID;
            licenseID = result.LicenseID;
            licenseServiceID = result.LicenseServiceID;

            return (companyID, instanceID, licenseID, licenseServiceID);
        }

        private async Task UpdateAlias(string authHeader, Guid instanceID, string alias)
        {
            await instanceClient.UpdateAliasAsync(authHeader, instanceID, alias);
        }
    }
}
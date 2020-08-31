using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Cotorra.Core.Extensions;
using Cotorra.Core.Validator;
using CotorraNode.Common.Config;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using CotorraNode.Common.Base.Schema;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Cotorra.UnitTest.Manager;

namespace Cotorra.UnitTest
{
    public class IncidentTypeOverdraftRelationshipUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID) where T : BaseEntity
        {
            var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
            var paymentPeriodicity = PaymentPeriodicity.Biweekly;

            //Act Dependencies
            //var period = (await new PeriodManagerUT().CreateDefaultAsync<Period>(identityWorkId, instanceID, initialDate, finalDate, paymentPeriodicity)).FirstOrDefault();
            var employee = (await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceID)).FirstOrDefault();
            var incidentType = (await CreateDefaultIncidentTypeAsync(identityWorkId, instanceID)).FirstOrDefault();
            var period = (await new PeriodManagerUT().CreateDefaultAsync<Period>(identityWorkId, instanceID, initialDate, finalDate, paymentPeriodicity)).FirstOrDefault();

            //var overdraft = (await new OverdraftManagerUT().CreateDefaultOverdraftAsync(identityWorkId, instanceID)).FirstOrDefault();
            var middlewarePeridDetailsManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetail = (await middlewarePeridDetailsManager.FindByExpressionAsync(p => p.PeriodID == period.ID, identityWorkId)).FirstOrDefault();


            List<Incident> IncidentTypePeriodEmployeeRelationships = new List<Incident>()
            {
                new Incident()
                {
                   ID = Guid.NewGuid(),
                   Active = true,
                   company = identityWorkId,
                   Timestamp = DateTime.UtcNow,
                   InstanceID = instanceID,
                   Description = "2R",
                   CreationDate = DateTime.Now,
                   Name = "Dos horas retardo",
                   StatusID = 1,
                   IncidentTypeID = incidentType.ID,
                   PeriodDetailID = periodDetail.ID,
                   Date  =DateTime.Now,
                   Value  = 2,
                   EmployeeID = employee.ID
                }
            };
            //Act
            var middlewareManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(), new IncidentValidator());
            await middlewareManager.CreateAsync(IncidentTypePeriodEmployeeRelationships, identityWorkId);

            return IncidentTypePeriodEmployeeRelationships as List<T>;
        }

        public async Task<List<IncidentType>> CreateDefaultIncidentTypeAsync(Guid identityWorkId, Guid instanceID)
        {
            var middlewareManager = new MiddlewareManager<IncidentType>(new BaseRecordManager<IncidentType>(), new IncidentTypeValidator());

            List<IncidentType> incidentTypes = new List<IncidentType>()
            {
                new IncidentType()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Incident",
                    CreationDate = DateTime.Now,
                    Name = "Incident",
                    StatusID = 1,
                    Code = "2",
                    DecreasesSeventhDay = false,
                    ItConsiders = ItConsiders.Absence,
                    Percentage = 10,
                    SalaryRight = true,
                    TypeOfIncident = TypeOfIncident.Days,
                    UnitValue = 10,
                }
            };
            await middlewareManager.CreateAsync(incidentTypes, identityWorkId);
            return incidentTypes;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_IncidentTypePeriodEmployeeRelationship_And_Get_ToValidate_Finally_do_Delete()
            {
                //var txOptions = new System.Transactions.TransactionOptions();
                //txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var incidentTypePeriodEmployeeRelationships = await new IncidentTypeOverdraftRelationshipUT().CreateDefaultAsync<Incident>(identityWorkId, instanceId);

                var middlewareManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(), new IncidentValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(incidentTypePeriodEmployeeRelationships.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAsync(incidentTypePeriodEmployeeRelationships.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == incidentTypePeriodEmployeeRelationships.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(incidentTypePeriodEmployeeRelationships.Select(p => p.ID).ToList(), identityWorkId);
                Assert.False(result2.Any());
            }


        }
    }
}

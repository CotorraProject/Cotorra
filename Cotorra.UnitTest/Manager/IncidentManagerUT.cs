using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.UnitTest.Manager
{
    public class IncidentManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID, Guid employeeID, Guid incidentTypeID) where T : BaseEntity
        {
            var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
            var paymentPeriodicity = PaymentPeriodicity.Biweekly;

            //Act Dependencies
            var period = (await new PeriodManagerUT().CreateDefaultAsync<Period>(identityWorkId, instanceID, initialDate, finalDate, paymentPeriodicity)).FirstOrDefault();

            var middlewarePeridDetailsManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetails = (await middlewarePeridDetailsManager.FindByExpressionAsync(p => p.PeriodID == period.ID, identityWorkId)).FirstOrDefault();

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
                   IncidentTypeID = incidentTypeID,
                   PeriodDetailID = periodDetails.ID,
                   Date = new DateTime(DateTime.UtcNow.Year, 1, 1),
                   Value  = 2,
                   EmployeeID = employeeID
                }
            };

            //Act
            var middlewareManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(), new IncidentValidator());
            await middlewareManager.CreateAsync(IncidentTypePeriodEmployeeRelationships, identityWorkId);

            return IncidentTypePeriodEmployeeRelationships as List<T>;
        }

        public async Task<List<T>> CreateDefaultAsyncWithPeriodDetailID<T>(Guid identityWorkId, Guid instanceID, 
            Guid employeeID, Guid periodDetailID, bool salaryRight, DateTime date) where T : BaseEntity
        {
            //Act Dependencies
            var incidentType = await new IncidentTypeManagerUT().CreateDefaultAsync<IncidentType>(identityWorkId, instanceID);

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
                   IncidentTypeID = incidentType.FirstOrDefault(p=>p.SalaryRight == salaryRight).ID,
                   PeriodDetailID = periodDetailID,
                   DeleteDate = null,
                   Date = date,
                   Value  = 2,
                   EmployeeID = employeeID
                }
            };

            //Act
            var middlewareManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(), new IncidentValidator());
            await middlewareManager.CreateAsync(IncidentTypePeriodEmployeeRelationships, identityWorkId);

            return IncidentTypePeriodEmployeeRelationships as List<T>;
        }
    }
}

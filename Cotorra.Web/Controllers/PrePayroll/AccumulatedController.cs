using Microsoft.AspNetCore.Mvc;
using Cotorra.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;

namespace Cotorra.Web.Controllers
{
    public class AccumulatedController : Controller
    {
        private readonly Client<HistoricAccumulatedEmployee> client;
        private readonly Client<AccumulatedType> clientAT;

        public AccumulatedController()
        {
            SessionModel.Initialize();
            client = new Client<HistoricAccumulatedEmployee>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
            clientAT = new Client<AccumulatedType>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid employeeID)
        {
            //Find accumulated by instance
            var findAccumulatedType = clientAT.FindAsync(x => x.InstanceID == SessionModel.InstanceID,
                SessionModel.CompanyID, new String[] { });

            //Find accumulated by employee
            var findAccumulatedEmployee = client.FindAsync(x => x.InstanceID == SessionModel.InstanceID && x.EmployeeID == employeeID,
                SessionModel.CompanyID, new String[] { });

            await Task.WhenAll(findAccumulatedEmployee, findAccumulatedType);

            var accumulatedResult =
                from at in findAccumulatedType.Result.AsParallel()
                join ae in findAccumulatedEmployee.Result.AsParallel()
                on at.ID equals ae.AccumulatedTypeID into accumulated
                from acc in accumulated.DefaultIfEmpty()
                orderby at.Name
                select new
                {
                    AccumulatedTypeID = at.ID,
                    AccumulatedEmployeeID = acc == null ? (Guid?)null : acc.ID,
                    Name = at.Name,
                    InitialExerciseAmount = acc == null ? 0 : acc.InitialExerciseAmount,
                    PreviousExerciseAccumulated = acc == null ? 0 : acc.PreviousExerciseAccumulated,
                    ExerciseFiscalYear = acc == null ? 0 : acc.ExerciseFiscalYear,
                    January = acc == null ? 0 : acc.January,
                    February = acc == null ? 0 : acc.February,
                    March = acc == null ? 0 : acc.March,
                    April = acc == null ? 0 : acc.April,
                    May = acc == null ? 0 : acc.May,
                    June = acc == null ? 0 : acc.June,
                    July = acc == null ? 0 : acc.July,
                    August = acc == null ? 0 : acc.August,
                    September = acc == null ? 0 : acc.September,
                    October = acc == null ? 0 : acc.October,
                    November = acc == null ? 0 : acc.November,
                    December = acc == null ? 0 : acc.December,
                };

            return Json(accumulatedResult);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(List<AccumulatedEmployeeModel> aem)
        {
            var idsToUpdate = aem.Where(x => x.AccumulatedEmployeeID != null).Select(x => x.AccumulatedEmployeeID);

            //Find accumulated to update
            var findAccumulatedType = await client.FindAsync(
                x => x.InstanceID == SessionModel.InstanceID && idsToUpdate.Contains(x.ID),
                SessionModel.CompanyID, new String[] { });

            //Update
            var accumulatedEmployeesToUpdate = findAccumulatedType.ToList();
            for (int i = 0; i < accumulatedEmployeesToUpdate.Count(); i++)
            {
                var ae = accumulatedEmployeesToUpdate[i];
                ae.InitialExerciseAmount = aem.Where(x => x.AccumulatedEmployeeID == ae.ID).Select(x => x.InitialExerciseAmount).FirstOrDefault();
            }

            //Create 
            var accumulatedEmployeesToCreate = from aemtc in aem
                                               where aemtc.AccumulatedEmployeeID == null
                                               select new HistoricAccumulatedEmployee
                                               {
                                                   ID = Guid.NewGuid(),
                                                   EmployeeID = aemtc.EmployeeID,
                                                   AccumulatedTypeID = aemtc.AccumulatedID,
                                                   ExerciseFiscalYear = aemtc.ExerciseFiscalYear,
                                                   InitialExerciseAmount = aemtc.InitialExerciseAmount,
                                                   PreviousExerciseAccumulated = 0,
                                                   January = 0,
                                                   February = 0,
                                                   March = 0,
                                                   April = 0,
                                                   May = 0,
                                                   June = 0,
                                                   July = 0,
                                                   August = 0,
                                                   September = 0,
                                                   October = 0,
                                                   November = 0,
                                                   December = 0,

                                                   //Common
                                                   Active = true,
                                                   company = SessionModel.CompanyID,
                                                   InstanceID = SessionModel.InstanceID,
                                                   CreationDate = DateTime.Now,
                                                   StatusID = 1,
                                                   user = SessionModel.IdentityID,
                                                   Timestamp = DateTime.Now
                                               };

            var createClient = client.CreateAsync(accumulatedEmployeesToCreate.ToList(), SessionModel.InstanceID);
            var updateClient = client.UpdateAsync(accumulatedEmployeesToUpdate.ToList(), SessionModel.InstanceID);

            await Task.WhenAll(createClient, updateClient);

            return Json("OK");
        }

        public class AccumulatedEmployeeModel
        {
            public Guid AccumulatedID { get; set; }
            public Guid EmployeeID { get; set; }
            public Guid? AccumulatedEmployeeID { get; set; }
            public Decimal InitialExerciseAmount { get; set; }
            public Int32 ExerciseFiscalYear { get; set; }
        }
    }
}
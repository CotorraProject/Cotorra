using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Cotorra.Core.Validator;
using CotorraNode.Common.Base.Schema;
using System.Transactions;
using System.Threading.Tasks;
using Cotorra.Core.Utils;
using CotorraNode.Common.Library.Public;
using CotorraNode.Security.ACS.Client.Schema.Entities;
using System.Net;
using CotorraNode.Common.Proxy;
using CotorraNube.CommonApp.RestClient;

namespace Cotorra.UnitTest
{
  
    public class SettlementProcessManagerDBUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {

            var employee = (await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId)).FirstOrDefault();

            var Departments = new List<Settlement>();
            Departments.Add(new Settlement()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Departamento de nominas",
                CreationDate = DateTime.Now,
                Name = "Nominas",
                StatusID = 1,
                EmployeeID = employee.ID,
                SettlementCause = SettlementCause.Absenteeism,
                ApplyEmployeeSubsidyInISRUSMOCalculus = true,
                CompleteISRYears = 5,
                
                
            });

            var middlewareManager = new MiddlewareManager<Settlement>(new BaseRecordManager<Settlement>(), new SettlementValidator());
            await middlewareManager.CreateAsync(Departments, identityWorkId);

            return Departments as List<T>;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_Settlement_And_Get_ToValidate_Finally_do_Delete()
            {
                var txOptions = new  TransactionOptions();
                txOptions.IsolationLevel = IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var settlements = await new SettlementProcessManagerDBUT().CreateDefaultAsync<Settlement>(identityWorkId, instanceId);

                var middlewareManager = new MiddlewareManager<Settlement>(new BaseRecordManager<Settlement>(), new SettlementValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(settlements.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete 
                await middlewareManager.DeleteAsync(settlements.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == settlements.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(settlements.Select(p => p.ID).ToList(), identityWorkId);
                Assert.False(result2.Any());
            }


        }

    }


    public class SettlementLetterUT
    {

        private List<Overdraft> GetOverDraft()
        {
            List<Overdraft> overs = new List<Overdraft>();
           Overdraft oversISR = OverdraftManagerUT.GenerateObject(Guid.NewGuid(), Guid.NewGuid()).FirstOrDefault();
            int code = 55;
            oversISR.OverdraftDetails.ForEach(detail =>
            {
                detail.ConceptPayment.Code = code++;
            });
            overs.Add(oversISR);

            overs.AddRange( OverdraftManagerUT.GenerateObject(Guid.NewGuid(), Guid.NewGuid()));

            return overs;

        }

        [Fact]
        public async Task GenerateSettlementLetterBlob()
        {
            SettlementProcessManager manager = new SettlementProcessManager();

            var url = await manager.GenerateSettlementLetter(new GenerateSettlementLetterParams()
            {
                IdentityWorkID = Guid.Parse("0c08daa6-f775-42a8-b75e-1b9b685b7977"),
                InstanceID = Guid.Parse("f16952c9-3475-4d65-a603-a93cf1ccde48"),
                OverdraftIDs = new List<Guid>() { Guid.Parse("1457133d-5e13-4ae3-935d-c846dcd8518e") }

            });

            Assert.NotNull(url);
        }

        [Fact]
        public async Task GenerateSettlementLetterFileDisk()
        {
            SettlementProcessManager manager = new SettlementProcessManager();
            var overdrafts = GetOverDraft();
            Guid IdentityWorkID = Guid.Parse("0c08daa6-f775-42a8-b75e-1b9b685b7977");
            Guid InstanceID = Guid.Parse("f16952c9-3475-4d65-a603-a93cf1ccde48");
            overdrafts.FirstOrDefault().Employee = new Employee() { Name = "Luis ", FirstLastName = "Tejeda" };

            
            var token = await LoginAsync("yamani@cotorrai.com", "LsyMR123.", "https://dev2authapicti.azurewebsites.net/api/Auth/LoginUser");

            var url = await manager.GenerateSettlementLetter(overdrafts, IdentityWorkID, InstanceID, token, new MSSpreadsheetWriterDisk());
            Assert.NotNull(url);
        }

        public static async Task<string> LoginAsync(string username, string password, string loginURI)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var userCredentials = new UserCredential() { Name = username, Password = password };
            var loginUserParams = new LoginUserParams();
            loginUserParams.UserCredential = userCredentials; 
            var resultdeserealizedJson = await  ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, "", new Uri(loginURI), loginUserParams);
            var resultdeserealized = JsonSerializer.DeserializeObject<LoginResult>(resultdeserealizedJson);

          return resultdeserealized.AccessToken.AuthorizationHeader;
        }

    }
}

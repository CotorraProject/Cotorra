using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest
{
    public class OverdraftManagerUT
    {
        public static   List<Overdraft> GenerateObject(Guid identityWorkId, Guid instanceID)
        {
            var conceptPayment = new ConceptPayment()
            {
                Active = true,
                AutomaticDismissal = true,
                Code = 1,
                company = identityWorkId,
                ConceptType = ConceptType.SalaryPayment,
                CreationDate = DateTime.Now,
                DeleteDate = null,
                Description = "Salary",
                Name =  "Salary",
                GlobalAutomatic = true,
                ID = Guid.NewGuid(),
                InstanceID = instanceID,
                Kind = false,
                Print = true,
                SATGroupCode = "P-001",
                StatusID = 1,
                Timestamp = DateTime.Now,
                user = Guid.NewGuid()
            };

            var conceptPayment2 = new ConceptPayment()
            {
                Active = true,
                AutomaticDismissal = true,
                Code = 2,
                company = identityWorkId,
                ConceptType = ConceptType.DeductionPayment,
                CreationDate = DateTime.Now,
                DeleteDate = null,
                Description = "IMSS",
                Name = "IMSS",
                GlobalAutomatic = true,
                ID = Guid.NewGuid(),
                InstanceID = instanceID,
                Kind = false,
                Print = true,
                SATGroupCode = "P-001",
                StatusID = 1,
                Timestamp = DateTime.Now,
                user = Guid.NewGuid()
            };

            var conceptPayment3 = new ConceptPayment()
            {
                Active = true,
                AutomaticDismissal = true,
                Code = 3,
                company = identityWorkId,
                ConceptType = ConceptType.DeductionPayment,
                CreationDate = DateTime.Now,
                DeleteDate = null,
                Description = "ieps",
                Name = "IEPS",
                GlobalAutomatic = true,
                ID = Guid.NewGuid(),
                InstanceID = instanceID,
                Kind = false,
                Print = true,
                SATGroupCode = "P-001",
                StatusID = 1,
                Timestamp = DateTime.Now,
                user = Guid.NewGuid()
            };

            var conceptPayment4 = new ConceptPayment()
            {
                Active = true,
                AutomaticDismissal = true,
                Code = 4,
                company = identityWorkId,
                ConceptType = ConceptType.DeductionPayment,
                CreationDate = DateTime.Now,
                DeleteDate = null,
                Description = "Bono",
                Name = "Bono",
                GlobalAutomatic = true,
                ID = Guid.NewGuid(),
                InstanceID = instanceID,
                Kind = false,
                Print = true,
                SATGroupCode = "P-001",
                StatusID = 1,
                Timestamp = DateTime.Now,
                user = Guid.NewGuid()
            };

            List<Overdraft> overdraft = new List<Overdraft>()
            {
                new Overdraft()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Sobrerecibo",
                    CreationDate = DateTime.Now,
                    Name = "Sobrerecibo",
                    StatusID = 1,
                    EmployeeID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    PeriodDetailID = Guid.NewGuid(),
                }
            };

            List<OverdraftDetail> overdraftDetail = new List<OverdraftDetail>()
            {
                new OverdraftDetail()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Sobrerecibo",
                    CreationDate = DateTime.Now,
                    Name = "Sobrerecibo",
                    StatusID = 1,
                    user = Guid.NewGuid(),
                    ConceptPaymentID = conceptPayment.ID,
                    Amount = 1500,
                    Taxed = 2,
                    Exempt = 3,
                    IMSSTaxed = 4,
                    IMSSExempt = 5,
                    Label1 = "Label1",
                    Label2 = "Label2",
                    Label3 = "Label3",
                    Label4 = "Label4",
                    OverdraftID = overdraft.FirstOrDefault().ID,
                    Value = 15,
                    ConceptPayment = conceptPayment
                },
                new OverdraftDetail()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Sobrerecibo",
                    CreationDate = DateTime.Now,
                    Name = "Sobrerecibo",
                    StatusID = 1,
                    user = Guid.NewGuid(),
                    ConceptPaymentID = conceptPayment2.ID,
                    Amount = 150,
                    Taxed = 2,
                    Exempt = 3,
                    IMSSTaxed = 4,
                    IMSSExempt = 5,
                    Label1 = "Label1",
                    Label2 = "Label2",
                    Label3 = "Label3",
                    Label4 = "Label4",
                    OverdraftID = overdraft.FirstOrDefault().ID,
                    Value = 15,
                    ConceptPayment = conceptPayment2
                },
                new OverdraftDetail()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Sobrerecibo",
                    CreationDate = DateTime.Now,
                    Name = "Sobrerecibo",
                    StatusID = 1,
                    user = Guid.NewGuid(),
                    ConceptPaymentID =conceptPayment3.ID,
                    Amount = 150,
                    Taxed = 2,
                    Exempt = 3,
                    IMSSTaxed = 4,
                    IMSSExempt = 5,
                    Label1 = "Label1",
                    Label2 = "Label2",
                    Label3 = "Label3",
                    Label4 = "Label4",
                    OverdraftID = overdraft.FirstOrDefault().ID,
                    Value = 15,
                    ConceptPayment = conceptPayment3
                },
                new OverdraftDetail()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Sobrerecibo",
                    CreationDate = DateTime.Now,
                    Name = "Sobrerecibo",
                    StatusID = 1,
                    user = Guid.NewGuid(),
                    ConceptPaymentID = conceptPayment4.ID,
                    Amount = 150,
                    Taxed = 2,
                    Exempt = 3,
                    IMSSTaxed = 4,
                    IMSSExempt = 5,
                    Label1 = "Label1",
                    Label2 = "Label2",
                    Label3 = "Label3",
                    Label4 = "Label4",
                    OverdraftID = overdraft.FirstOrDefault().ID,
                    Value = 15,
                    ConceptPayment = conceptPayment4
                }
            };

            overdraft.FirstOrDefault().OverdraftDetails = overdraftDetail;
            return overdraft; 
        }

        public async Task<List<Overdraft>> CreateDefaultOverdraftAsync(Guid identityWorkId, Guid instanceID)
        {
            var middlewareManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());

            var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
            var paymentPeriodicity = PaymentPeriodicity.Biweekly;

            //Act Dependencies
            var period = (await new PeriodManagerUT().CreateDefaultAsync<Period>(identityWorkId, instanceID, initialDate, finalDate, paymentPeriodicity)).FirstOrDefault();
            var employee = (await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceID)).FirstOrDefault();

            var middlewarePeridDetailsManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetail = (await middlewarePeridDetailsManager.FindByExpressionAsync(p => p.PeriodID == period.ID, identityWorkId)).FirstOrDefault();

            List<Overdraft> overdraft = new List<Overdraft>()
            {
                new Overdraft()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Sobrerecibo",
                    CreationDate = DateTime.Now,
                    Name = "Sobrerecibo",
                    StatusID = 1,
                    EmployeeID = employee.ID,
                    user = Guid.NewGuid(),
                    PeriodDetailID = periodDetail.ID
                }
            };

            var middlewareConceptManager = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(), new ConceptPaymentValidator());
            var concept = (await middlewareConceptManager.FindByExpressionAsync(p => p.Active && p.InstanceID == instanceID, identityWorkId));
            if (!concept.Any())
            {
                concept = (await new ConceptManagerUT().CreateDefaultSalaryPaymentConceptsAsync(identityWorkId, instanceID));
            }

            var middlewareOverdraftDetailManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());
            List<OverdraftDetail> overdraftDetail = new List<OverdraftDetail>()
            {
                new OverdraftDetail()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Sobrerecibo",
                    CreationDate = DateTime.Now,
                    Name = "Sobrerecibo",
                    StatusID = 1,
                    user = Guid.NewGuid(),
                    ConceptPaymentID = concept.FirstOrDefault().ID,
                    Amount = 1,
                    Taxed = 2,
                    Exempt = 3,
                    IMSSTaxed = 4,
                    IMSSExempt = 5,
                    Label1 = "Label1",
                    Label2 = "Label2",
                    Label3 = "Label3",
                    Label4 = "Label4",
                    OverdraftID = overdraft.FirstOrDefault().ID,
                    Value = 15,
                }
            };

            await middlewareManager.CreateAsync(overdraft, identityWorkId);
            await middlewareOverdraftDetailManager.CreateAsync(overdraftDetail, identityWorkId);

            overdraft.FirstOrDefault().OverdraftDetails = overdraftDetail;

            return overdraft;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_Overdraft_And_Get_ToValidate_Finally_do_Delete()
            {
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                
                var middlewareManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), 
                    new OverdraftValidator());
                var overdr = await middlewareManager.FindByExpressionAsync(p => 
                    p.ID == Guid.Parse("24062420-7C73-4453-B837-A9B25AC26595"), identityWorkId , 
                    new string[] { "OverdraftDetails" });
                var detail = overdr.FirstOrDefault().OverdraftDetails
                    .Where(p => p.ID == Guid.Parse("c1314de9-13cf-4531-8641-23b04a4ce187"));

                var middlewareManagerDetail = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(),
                    new OverdraftDetailValidator());
                var detail2 = await middlewareManagerDetail.FindByExpressionAsync(p => p.ID == Guid.Parse("c1314de9-13cf-4531-8641-23b04a4ce187"), identityWorkId);

                var overdrafts = await new OverdraftManagerUT().CreateDefaultOverdraftAsync(identityWorkId, instanceId);

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(overdrafts.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                var middlewareOverdraftDetailManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());

                await middlewareOverdraftDetailManager.DeleteAsync(overdrafts.SelectMany(p => p.OverdraftDetails).Select(p => p.ID).ToList(), identityWorkId);
                await middlewareManager.DeleteAsync(overdrafts.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == overdrafts.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(overdrafts.Select(p => p.ID).ToList(), identityWorkId);
                Assert.False(result2.Any());
            }


        }
    }
}

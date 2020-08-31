using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Cotorra.Core.Extensions;
using Cotorra.Core.Validator;
using Cotorra.Client;
using System.Transactions;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class PayrollCompanyConfigurationUT
    {
        [Fact]
        public async Task Should_Create_Delete()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            List<PayrollCompanyConfiguration> payrollCompanyConfigurations = new List<PayrollCompanyConfiguration>();
            var identityWorkId = Guid.NewGuid();

            //Fills the dummyData
            var payrollConfig = new PayrollCompanyConfiguration();
            payrollConfig.Active = true;
            payrollConfig.company = identityWorkId;
            payrollConfig.CurrentExerciseYear = 2014;
            payrollConfig.ID = Guid.NewGuid();
            payrollConfig.InstanceID = Guid.NewGuid();
            payrollConfig.StartDate = new DateTime(2014, 1, 1).Date;
            payrollConfig.Timestamp = DateTime.Now;
            payrollConfig.user = Guid.Empty;
            payrollConfig.CurrencyID = Guid.NewGuid();
            payrollConfig.CurrentPeriod = 1;

            payrollCompanyConfigurations.Add(payrollConfig);

            //Act
            var middlewareManager = new MiddlewareManager<PayrollCompanyConfiguration>(new BaseRecordManager<PayrollCompanyConfiguration>(), new PayrollCompanyConfigurationValidator());
            await middlewareManager.CreateAsync(payrollCompanyConfigurations, identityWorkId);

            //Asserts
            //Get
            var result = await middlewareManager
                .GetByIdsAsync(payrollCompanyConfigurations.Select(p => p.ID).ToList(), identityWorkId);
            Assert.True(result.Any());

            //Delete
            await middlewareManager.DeleteAsync(payrollCompanyConfigurations.Select(p => p.ID).ToList(), identityWorkId);
            Assert.True(result.FirstOrDefault().ID == payrollCompanyConfigurations.FirstOrDefault().ID);

            //Get it again to verify if the registry it was deleted
            var result2 = await middlewareManager
                .GetByIdsAsync(payrollCompanyConfigurations.Select(p => p.ID).ToList(), identityWorkId);
            Assert.True(!result2.Any());
        }

        [Fact]
        public async Task Should_Fail_When_company_Is_Empty()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Client<PayrollCompanyConfiguration>  client = new Client<PayrollCompanyConfiguration>("",
                ClientConfiguration.ClientAdapter.Local);

            List<PayrollCompanyConfiguration> payrollCompanyConfigurations = new List<PayrollCompanyConfiguration>();
            var identityWorkId = Guid.Empty;

            //Fills the dummyData
            var payrollConfig = new PayrollCompanyConfiguration();
            payrollConfig.Active = true;
            payrollConfig.company = identityWorkId;
            payrollConfig.CurrentExerciseYear = 2014;
            payrollConfig.ID = Guid.NewGuid();
            payrollConfig.InstanceID = Guid.NewGuid();
            payrollConfig.StartDate = new DateTime(2014, 1, 1).Date;
            payrollConfig.Timestamp = DateTime.Now;
            payrollConfig.user = Guid.Empty;
            payrollConfig.CurrencyID = Guid.NewGuid();

            payrollCompanyConfigurations.Add(payrollConfig);
            try
            {
                  await client.CreateAsync(payrollCompanyConfigurations, identityWorkId);
            }
            catch (AggregateException ex)
            {
                var resex = ex.InnerExceptions.First() as CotorraException;

                var res = resex.ValidationInfo;
                Assert.NotEmpty(res);
                Assert.Equal(1301, resex.ErrorCode);
            }
            catch(Exception ex)
            {
                var e = ex;
            }
        }

        [Fact]
        public async Task Should_Fail_When_InstanceID_Is_Empty()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Client<PayrollCompanyConfiguration> client = new Client<PayrollCompanyConfiguration>("",
                ClientConfiguration.ClientAdapter.Local);

            List<PayrollCompanyConfiguration> payrollCompanyConfigurations = new List<PayrollCompanyConfiguration>();
            var identityWorkId = Guid.NewGuid();

            //Fills the dummyData
            var payrollConfig = new PayrollCompanyConfiguration();
            payrollConfig.Active = true;
            payrollConfig.company = identityWorkId;
            payrollConfig.CurrentExerciseYear = 2014;
            payrollConfig.ID = Guid.NewGuid();
            payrollConfig.InstanceID = Guid.Empty;
            payrollConfig.StartDate = new DateTime(2014, 1, 1).Date;
            payrollConfig.Timestamp = DateTime.Now;
            payrollConfig.user = Guid.Empty;
            payrollConfig.CurrencyID = Guid.NewGuid();

            payrollCompanyConfigurations.Add(payrollConfig);
            try
            {
                await client.CreateAsync(payrollCompanyConfigurations, identityWorkId);
            }
            catch (AggregateException ex)
            {
                var resex = ex.InnerExceptions.First() as CotorraException;

                var res = resex.ValidationInfo;
                Assert.NotEmpty(res);
                Assert.Equal(1302, resex.ErrorCode);
            }
            catch (Exception ex)
            {
                var e = ex;
            }
        }

        [Fact]
        public async Task Should_Fail_When_CurrencyID_Is_Empty()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Client<PayrollCompanyConfiguration> client = new Client<PayrollCompanyConfiguration>("",
                ClientConfiguration.ClientAdapter.Local);

            List<PayrollCompanyConfiguration> payrollCompanyConfigurations = new List<PayrollCompanyConfiguration>();
            var identityWorkId = Guid.NewGuid();

            //Fills the dummyData
            var payrollConfig = new PayrollCompanyConfiguration();
            payrollConfig.Active = true;
            payrollConfig.company = identityWorkId;
            payrollConfig.CurrentExerciseYear = 2014;
            payrollConfig.ID = Guid.NewGuid();
            payrollConfig.InstanceID = Guid.NewGuid();
            payrollConfig.StartDate = new DateTime(2014, 1, 1).Date;
            payrollConfig.Timestamp = DateTime.Now;
            payrollConfig.user = Guid.Empty;
            payrollConfig.CurrencyID = Guid.Empty;

            payrollCompanyConfigurations.Add(payrollConfig);
            try
            {
                await client.CreateAsync(payrollCompanyConfigurations, identityWorkId);
            }
            catch (AggregateException ex)
            {
                var resex = ex.InnerExceptions.First() as CotorraException;

                var res = resex.ValidationInfo;
                Assert.NotEmpty(res);
                Assert.Equal(1303, resex.ErrorCode);
            }
            catch (Exception ex)
            {
                var e = ex;
            }
        }

        [Fact]
        public async Task Should_Fail_When_CurrentExerciseYear_Is_Lower_Than_2000()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Client<PayrollCompanyConfiguration> client = new Client<PayrollCompanyConfiguration>("",
                ClientConfiguration.ClientAdapter.Local);

            List<PayrollCompanyConfiguration> payrollCompanyConfigurations = new List<PayrollCompanyConfiguration>();
            var identityWorkId = Guid.NewGuid();

            //Fills the dummyData
            var payrollConfig = new PayrollCompanyConfiguration();
            payrollConfig.Active = true;
            payrollConfig.company = identityWorkId;
            payrollConfig.CurrentExerciseYear = 1999;
            payrollConfig.ID = Guid.NewGuid();
            payrollConfig.InstanceID = Guid.NewGuid();
            payrollConfig.StartDate = new DateTime(2014, 1, 1).Date;
            payrollConfig.Timestamp = DateTime.Now;
            payrollConfig.user = Guid.Empty;
            payrollConfig.CurrencyID = Guid.NewGuid();

            payrollCompanyConfigurations.Add(payrollConfig);
            try
            {
                await client.CreateAsync(payrollCompanyConfigurations, identityWorkId);
            }
            catch (AggregateException ex)
            {
                var resex = ex.InnerExceptions.First() as CotorraException;

                var res = resex.ValidationInfo;
                Assert.NotEmpty(res);
                Assert.Equal(1304, resex.ErrorCode);
            }
            catch (Exception ex)
            {
                var e = ex;
            }
        }

        [Fact]
        public async Task Should_Fail_When_NonDeducibleFactor_Is_Not_47_nor_53()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Client<PayrollCompanyConfiguration> client = new Client<PayrollCompanyConfiguration>("",
                ClientConfiguration.ClientAdapter.Local);
            List<PayrollCompanyConfiguration> payrollCompanyConfigurations = new List<PayrollCompanyConfiguration>();
            var identityWorkId = Guid.NewGuid();

            //Fills the dummyData
            var payrollConfig = new PayrollCompanyConfiguration();
            payrollConfig.Active = true;
            payrollConfig.company = identityWorkId;
            payrollConfig.CurrentExerciseYear = 2018;
            payrollConfig.ID = Guid.NewGuid();
            payrollConfig.InstanceID = Guid.NewGuid();
            payrollConfig.StartDate = new DateTime(2014, 1, 1).Date;
            payrollConfig.Timestamp = DateTime.Now;
            payrollConfig.user = Guid.Empty;
            payrollConfig.CurrencyID = Guid.NewGuid();
            payrollCompanyConfigurations.Add(payrollConfig);
            try
            {
                await client.CreateAsync(payrollCompanyConfigurations, identityWorkId);
            }
            catch (AggregateException ex)
            {
                var resex = ex.InnerExceptions.First() as CotorraException;
                var res = resex.ValidationInfo;
                Assert.NotEmpty(res);
                Assert.Equal(1305, resex.ErrorCode);
            }
            catch (Exception ex)
            {
                var e = ex;
            }
        }
    }
}

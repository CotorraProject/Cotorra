using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Cotorra.Core.Extensions;
using Cotorra.Core.Validator;
using CotorraNode.Common.Base.Schema;
using System.Transactions;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class DepartmentManagerUT 
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var Departments = new List<Department>();
            Departments.Add(new Department()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Departamento de nominas",
                BanksBeneficiary = "Beneficiario",
                CreationDate = DateTime.Now,
                Name = "Nominas",
                Number = 1,
                StatusID = 1,
            });

            var middlewareManager = new MiddlewareManager<Department>(new BaseRecordManager<Department>(), new DepartmentValidator());
            await middlewareManager.CreateAsync(Departments, identityWorkId);

            return Departments as List<T>;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_Department_And_Area_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var Areas = await new AreaManagerUT().CreateDefaultAsync<Area>(identityWorkId, instanceId);

                var Departments = await new DepartmentManagerUT().CreateDefaultAsync<Department>(identityWorkId, instanceId);
                Departments.FirstOrDefault().AreaID = Areas.FirstOrDefault().ID;

                //Act
                var middlewareManager = new MiddlewareManager<Department>(new BaseRecordManager<Department>(), new DepartmentValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(Departments.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAsync(Departments.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == Departments.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(Departments.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }
        }

        public class DeleteAll
        {
            [Fact]
            public async Task Should_Delete_all()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var Areas = await new AreaManagerUT().CreateDefaultAsync<Area>(identityWorkId, instanceId);

                var Departments = await new DepartmentManagerUT().CreateDefaultAsync<Department>(identityWorkId, instanceId);
                Departments.FirstOrDefault().AreaID = Areas.FirstOrDefault().ID;

                //Act
                var middlewareManager = new MiddlewareManager<Department>(new BaseRecordManager<Department>(), new DepartmentValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(Departments.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAllAsync( identityWorkId, instanceId);
                Assert.True(result.FirstOrDefault().ID == Departments.FirstOrDefault().ID);

                //Get it again to verify if the registry   was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(Departments.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }
        }
    }
}

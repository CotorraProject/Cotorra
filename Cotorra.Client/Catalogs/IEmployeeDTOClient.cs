using Cotorra.Schema.DTO.Catalogs;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IEmployeeDTOClient
    {
        Task<EmployeeDTO> GetById(Guid companyId, Guid instanceId, Guid employeeId);

        Task<EmployeeDTO> GetByIdentityId(Guid identityUserID);
    }
}

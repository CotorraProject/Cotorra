using Cotorra.Schema.DTO.Catalogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IOverdraftDTOClient
    {
        Task<List<OverdraftDTO>> GetByEmployeeId(Guid companyId, Guid instanceId, Guid employeeId);
    }
}

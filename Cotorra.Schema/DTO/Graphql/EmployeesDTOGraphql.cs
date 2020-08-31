using Cotorra.Schema.DTO.Catalogs;
using System.Collections.Generic;

namespace Cotorra.Schema
{

    public class EmployeesDTOGraphql
    {
        public List<EmployeeDTO> Employees { get; set; }
    }

    public class EmployeeDTOGraphql
    {
        public EmployeeDTO Employee { get; set; }
    }

    public class EmployeeIdentityDTOGraphqE
    {
        public EmployeeDTO EmployeeIdentity { get; set; }
    }
}

using AutoMapper; 
using Cotorra.Core.Validator; 
using Cotorra.Schema.DTO;
using System; 
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{
    
    public interface IDepartmentService
    {
        Task<DepartmentDTO> Post(Guid instanceID, Guid userID, Guid identityWorkID, DepartmentValidator validator, IMapper mapper,
             DepartmentDTO departmentDTO);
    }
}

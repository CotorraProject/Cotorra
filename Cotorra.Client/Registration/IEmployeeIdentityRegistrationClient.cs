using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IEmployeeIdentityRegistrationClient
    {
        Task<Guid?> GetIdentityUserAsync(string email);
    }
}

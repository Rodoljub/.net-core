using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface ICLRTypeRepository : IBaseRepository<CLR_Type, IdentityUser>
	{
		Task<CLR_Type> GetClrTypeByName(string clrTypeName);
    }
}

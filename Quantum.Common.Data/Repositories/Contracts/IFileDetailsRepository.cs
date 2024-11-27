using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
	public interface IFileDetailsRepository : IBaseRepository<FileDetails, IdentityUser>
	{
		Task<string> InsertFileDetails(FileDetails fileDetails, IdentityUser user);
	}
}

using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Data.Repositories.Contracts
{
	public interface IFolderRepository : IBaseRepository<Folder, IdentityUser>
	{
	}
}

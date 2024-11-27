using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
	public interface IReportedContentReasonRepository : IBaseRepository<ReportedContentReason, IdentityUser>
	{
		Task<IEnumerable<ReportedContentReason>> GetReportedContentReasons();
	}
}

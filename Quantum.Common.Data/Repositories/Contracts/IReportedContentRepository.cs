using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IReportedContentRepository : IBaseRepository<ReportedContent, IdentityUser>
	{
		//Task<ReportedContent> ReportContent(string reportedId, CLR_Type reportedType, string userId, ReportedContentReason reportedContentReason);
	}
}

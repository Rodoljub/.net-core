using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
	public interface IReportedContentService
	{
		Task CreateReportedContentReasons(ReportedContentReasonsModel model, IIdentity identity);

		Task<IEnumerable<ReportedContentReasonModel>> GetReportedContentReasons();

		Task SetReportedContent(ReportedContentModel model, IIdentity identity);
	}
}

using Quantum.Core.Models;
using Quantum.Data.Entities;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services.Contracts
{
	public interface IMappingReportedContentService
	{
		Task<ReportedContentReason> MapReportedContentReasonFromString(string reportedContentReasonDescriptions, string clrTypeId);

		Task<ReportedContent> MapReportedContentFromReportedContentModel(ReportedContentModel model, string clrTypeId, string userId);

		Task<ReportedContentReasonModel> MapReportedContentReasonModelFromReportedContentReason(ReportedContentReason model);
	}
}

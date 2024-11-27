using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;

namespace Quantum.Data.Repositories
{
    public class ReportedContentRepository : BaseRepository<ReportedContent>, IReportedContentRepository
	{
		private QDbContext _context;

		public ReportedContentRepository(QDbContext context)
			: base(context)
		{
			_context = context;
		}

		//public async Task<ReportedContent> ReportContent(string reportedId, CLR_Type reportedType, string userId, ReportedContentReason reportedContentReason)
		//{
			//var newReportedContent = new ReportedContent()
			//{
			//	ReportedId = reportedId,
			//	ReportedType = reportedType,
			//	ReporterId = userId,
			//	ReportedContentReason = reportedContentReason
			//};

			//await base.Insert(newReportedContent);

			//return await Task.FromResult(newReportedContent);
		//}
	}
}

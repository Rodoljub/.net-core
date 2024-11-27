using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
	public class ReportedContentReasonRepository : BaseRepository<ReportedContentReason>, IReportedContentReasonRepository
	{
		public QDbContext _context;
		public ReportedContentReasonRepository(QDbContext context)
			: base(context)
		{
			_context = context;
		}

		public async Task<IEnumerable<ReportedContentReason>> GetReportedContentReasons()
		{
			var reportedContentReasons = await Query(rcr => !rcr.IsDeleted)
				.Include(rcr => rcr.ReportedContentType)
				.ToListAsync();

			return reportedContentReasons;
		}
	}
}

using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Data.Repositories
{
	public class FolderRepository : BaseRepository<Folder>, IFolderRepository
	{
		private QDbContext _context;

		public FolderRepository(
			QDbContext context
			) : base(context)
		{
			_context = context;
		}
	}
}

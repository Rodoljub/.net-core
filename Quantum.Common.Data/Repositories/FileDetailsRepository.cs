using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
	public class FileDetailsRepository : BaseRepository<FileDetails>, IFileDetailsRepository
	{
		private QDbContext _context;

		public FileDetailsRepository(
			QDbContext context
			) : base(context)
		{
			_context = context;
		}

		public async Task<string> InsertFileDetails(FileDetails fileDetails, IdentityUser user)
		{
			await base.Insert(fileDetails, user);

			return fileDetails.ID;
		}
	}
}

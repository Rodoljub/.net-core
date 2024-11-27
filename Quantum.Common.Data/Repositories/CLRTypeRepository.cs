using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Net;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
	public class CLRTypeRepository : BaseRepository<CLR_Type>, ICLRTypeRepository
	{
		private QDbContext _context;

		public CLRTypeRepository(QDbContext context)
			: base(context)
		{
			_context = context;
		}

		public async Task<CLR_Type> GetClrTypeByName(string clrTypeName)
		{
			switch (clrTypeName)
			{
				case "Item":
					clrTypeName = typeof(Item).Name;
					break;

				case "Comment":
					clrTypeName = typeof(Comment).Name;
					break;

				default:

					throw new ClrTypeNotFoundException(
						HttpStatusCode.BadRequest, Errors.ErrorClrTypeNotFound);
			}

			var result = await Query(t => !t.IsDeleted && t.Name == clrTypeName)
				.AsNoTracking()
				.FirstOrDefaultAsync();

			if (result == null || result == default(CLR_Type))
			{
				throw new ClrTypeNotFoundException(
					HttpStatusCode.BadRequest, Errors.ErrorClrTypeNotFound
					);
			}

			return result;
		}
    }
}

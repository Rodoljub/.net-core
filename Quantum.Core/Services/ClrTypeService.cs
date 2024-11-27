using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
	public class CLRTypeService : ICLRTypeService
	{
		private ICLRTypeRepository _clrTypeRepo;

		public CLRTypeService(
			ICLRTypeRepository clrTypeRepo
		)
		{
			_clrTypeRepo = clrTypeRepo;
		}

		public async Task InsertClrType(string clrTypeName)
		{
			var result = new CLR_Type { Name = clrTypeName };

			await _clrTypeRepo.Insert(result, null);
		}

	}
}

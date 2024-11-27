using Quantum.Data.Entities;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
	public interface ICLRTypeService
	{
		Task InsertClrType(string clrTypeName);
	}
}

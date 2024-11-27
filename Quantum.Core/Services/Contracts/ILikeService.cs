using Quantum.Core.Models;
using Quantum.Data.Entities;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
	public interface ILikeService
	{
		Task AddOrRemoveLike(LikeModel model, IIdentity identity);
	}
}

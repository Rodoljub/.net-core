using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
	public interface IFavouriteService
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		Task AddOrRemoveFavourite(FavouriteModel model, IIdentity identity);
	}
}

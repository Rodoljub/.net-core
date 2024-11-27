using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Utility.Filters;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
	[Produces("application/json")]
	[Route("api/Favourites")]
	//[ApiController]
	[Authorize(LocalApi.PolicyName)]
    public class FavoritesController : Controller
	{
		private IFavouriteService _favouriteServ;
		private IItemsListService _itemListServ;

		public FavoritesController(
			IFavouriteService favouriteServ,
			IItemsListService itemListServ
		)
		{
			_favouriteServ = favouriteServ;
			_itemListServ = itemListServ;
		}

		[HttpPost]
		[ValidateModel]
		public async Task<IActionResult> AddRemoveFavorite([FromBody] FavouriteModel model)
		{
			await _favouriteServ.AddOrRemoveFavourite(model, User.Identity);

            return Ok(model);
		}

		[HttpGet("list")]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> GetFavouritesItems([FromQuery] int skip)
		{
			var viewItems = await _itemListServ.GetFavouritesItems(skip, User.Identity);

			return Ok(viewItems);
		}
	}
}

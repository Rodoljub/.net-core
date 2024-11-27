using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quantum.Core.Services.Contracts;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
	[Produces("application/json")]
	[Route("api/items/portfolio")]
	[ApiController]
	[Authorize(LocalApi.PolicyName)]
	public class PortfolioController : Controller
	{
		private IMemoryCache _cache;
		private IItemsListService _itemsListServ;

		public PortfolioController(
			IMemoryCache cache,
			IItemsListService itemsListServ
		)
		{
			_cache = cache;
			_itemsListServ = itemsListServ;
		}

		[HttpGet]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> Get([FromQuery] int skip)
		{
			var viewItems = await _itemsListServ.GetPortfolioItems(skip, User.Identity);

			return Ok(viewItems);
		}

		[AllowAnonymous]
		[HttpGet("{name}")]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> GetAnonymous([FromQuery] int skip, string name)
		{
			var viewItems = await _itemsListServ.GetPortfolioAnonymousItems(skip, User.Identity, name);

			return Ok(viewItems);
		}
	}
}

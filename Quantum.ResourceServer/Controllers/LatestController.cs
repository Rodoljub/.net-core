
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Models;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
    [Produces("application/json")]
    [Route("api/items/latest")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class LatestController : Controller
    {
        private IMemoryCache _cache;
		private IItemsListService _itemsListServ;
        

        public LatestController(
            IMemoryCache cache,
			IItemsListService itemListServ
            
        )
        {
            _cache = cache;
			_itemsListServ = itemListServ;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Latest([FromQuery] int skip)
		{
			var viewItems = await _itemsListServ.GetLatestItems(skip, User.Identity);

			return Ok(viewItems);
		}
	}
}

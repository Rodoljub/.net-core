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
	[Route("api/items")]
	[ApiController]
	[Authorize(LocalApi.PolicyName)]
	public class ItemsController : Controller
	{
		private IItemsService _itemServ;
		private IItemsListService _itemsListServ;

		public ItemsController(
			IItemsService itemServ,
			IItemsListService itemsListServ
		)
		{
			_itemServ = itemServ;
			_itemsListServ = itemsListServ;
		}

        //[AllowAnonymous]
        //[HttpPost("agg")]
        //public IActionResult Aggragation([FromForm] string userId)
        //{
        //    _itemServ.Aggragatiosns(userId);

        //    return Ok();
        //}

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromForm] ItemCreateModel model)
        {
			var image = await _itemServ.CreateItemForAnalysis(model, User.Identity);

			return Ok(image);
		}

        [HttpPut]
		public async Task<IActionResult> Update([FromForm] ItemUpdateModel model)
		{
			var itemId = await _itemServ.UpdateItem(model, User.Identity);

			var itemUrl = Url.Link("GetItemByID", new { id = itemId });

			return Created(itemUrl, null);
		}

		[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
			var itemId = await _itemServ.DeleteItem(id, User.Identity);

			return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetItemByID")]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetItem(string id)
        {
            var viewItem = await _itemServ.GetItemView(id, User.Identity);

			return Ok(viewItem); 
		}

        //[AllowAnonymous]
        //[HttpGet("{id}/resized", Name = "GetItemByIdResized")]
        //public async Task<IActionResult> GetItemResized(string id)
        //{

        //}

        [AllowAnonymous]
        [HttpGet("related/{id}")]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> Related(string id, [FromQuery] int skip)
        {
			var viewItems = await _itemsListServ.GetRelatedItems(id, skip, User.Identity);

			return Ok(viewItems);
        }
	
	
		[HttpGet("analyzingImages")]
		public async Task<IActionResult> GetAnalyzingImages([FromQuery] string[] filesIds)
        {
			var analyzingImages = await _itemServ.GetAnalyzingImages(filesIds, User.Identity);
			return Ok(analyzingImages);

		}
	}
}

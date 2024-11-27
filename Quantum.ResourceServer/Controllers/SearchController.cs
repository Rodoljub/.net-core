using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Utility.Filters;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
	[Produces("application/json")]
	[Route("api/search")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class SearchController : Controller
	{
		private IItemsListService _itemsListService;
		private ITagService _tagService;
        private ISearchService _searchServ;
		private IMemoryCache _cache;

		public SearchController(
			IItemsListService itemsListService,
			ITagService tagService,
            ISearchService searchServ,
            IMemoryCache cache
		)
		{
			_itemsListService = itemsListService;
			_tagService = tagService;
            _searchServ = searchServ;
			_cache = cache;
		}

		[AllowAnonymous]
		[HttpGet]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> Search([FromQuery] string query, int skip)
		{
			var viewItems = await _itemsListService.GetSearchItems(skip, User.Identity, query);

			return Ok(viewItems);
		}

		[AllowAnonymous]
		[HttpGet("tags")]
		[ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
		public async Task<IActionResult> SearchTags([FromQuery] string query, string selectedTags, int skip)
		{
			var tags = await _tagService.SearchTags(query, selectedTags);

			return Ok(tags);
		}

        [ValidateModel]
        [HttpPost("SaveSearch")]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> SaveSearch([FromBody] SaveSearchModel model)
        {
            var saveSearchResultsView = await _searchServ.SaveSearch(model, User.Identity);

            return Ok(saveSearchResultsView);
        }

        [ValidateModel]
        [HttpPost("UpdateSaveSearch")]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> UpdateSaveSearch([FromBody] SavedSearchResultModel model)
        {
            var saveSearchResultsView = await _searchServ.UpdateSaveSearch(model, User.Identity);

            return Ok(saveSearchResultsView);
        }

        [ValidateModel]
        [HttpPost("DeleteSaveSearch")]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> DeleteSaveSearch([FromBody] string[] model)
        {
            var deletedResultsIds = await _searchServ.DeleteSaveSearch(model, User.Identity);

            return Ok(deletedResultsIds);
        }

        //[AllowAnonymous]  
        [HttpGet("SaveSearch")]
        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 60)]
        public async Task<IActionResult> GetSavedSearch()
        {
            var savedSearchResults = await _searchServ.GetSavedSearchResults(User.Identity);

            return Ok(savedSearchResults);
        }
    }
}

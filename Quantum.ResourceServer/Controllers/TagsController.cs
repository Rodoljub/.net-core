using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
	[Produces("application/json")]
	[Route("api/Tags")]
	[ApiController]
	[Authorize(LocalApi.PolicyName)]
	public class TagsController : Controller
	{
        private ITagService _tagService;


		public TagsController(
            ITagService tagService
		)

		{
            _tagService = tagService;
        }

        [Authorize("create:tags")]
        [HttpPost("")]
		public async Task<IActionResult> SetTags([FromForm] TagsModel model)
		{
           await _tagService.CreateTags(model.TagName, User.Identity);

           return Ok();
		
		}

        //[Authorize("read:tags")]
        [HttpGet("")]
		public async Task<IActionResult> GetTags()
		{
            var tags = await _tagService.GetTags();

			return Ok(tags);
		}
	}
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Models.ReadModels;
using Quantum.Utility.Filters;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
    [EnableCors("GeneralPolicy")]
	[Produces("application/json")]
	[Route("api/Comments")]
	[ApiController]
	[Authorize(LocalApi.PolicyName)]
	public class CommentController : Controller
	{
		private IMemoryCache _cache;
		private ICommentService _commentServ;

		public CommentController(
			IMemoryCache cache,
			ICommentService commentServ
		)
		{
			_cache = cache;
			_commentServ = commentServ;
		}

		[HttpPost]
		[ValidateModel]
		public async Task<IActionResult> AddComment([FromBody] CommentModel model)
		{
			var viewComent = await _commentServ.AddComment(model, User.Identity);

			if (viewComent == default)
                return BadRequest();

            return Ok(viewComent);
		}

		[HttpPost("Update")]
		[ValidateModel]
		public async Task<IActionResult> UpdateComment([FromBody] CommentViewModel model)
		{
			var viewComment = await _commentServ.UpdateComment(model, User.Identity);

			return Ok(viewComment);
		}

		[AllowAnonymous]
		[HttpGet("{parentId}")]
		public async Task<IActionResult> GetItemComments(string parentId, [FromQuery] string[] initialCommentsIds, string typeName, int skip)
		{
			var viewComents = await _commentServ.GetViewCommentsModels(parentId, initialCommentsIds, typeName, skip, User.Identity);

			return Ok(viewComents);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteComments(string id)
		{
			var isDeleted = await _commentServ.DeleteComment(id, User.Identity);

			return Ok(isDeleted);
		}
	}
}

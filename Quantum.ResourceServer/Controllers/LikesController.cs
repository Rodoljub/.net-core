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
	[Route("api/Likes")]
	[ApiController]
	[Authorize(LocalApi.PolicyName)]
	public class LikesController : Controller
    {
		private ILikeService _likeServ;

		public LikesController(
			ILikeService likeServ
		)
		{
			_likeServ = likeServ;
		}

		[HttpPost]
		[ValidateModel]
		public async Task<IActionResult> AddRemoveLike([FromBody] LikeModel model)
		{
			await _likeServ.AddOrRemoveLike(model, User.Identity);

            return Ok();
		}
	}
}

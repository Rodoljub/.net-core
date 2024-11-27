using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quantum.Core.Services.Contracts;
using Quantum.Core.Services.ImageSharp.Contracts;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
	[Produces("application/json")]
	[Route("api/image")]
	[ApiController]
	[Authorize(LocalApi.PolicyName)]
	public class ImageController : Controller
	{
		private IImageService _imageServ;
		private IImageSharpService _imageSharpServ;

		public ImageController(
			IImageService imageServ,
			IImageSharpService imageSharpServ
		)
		{
			_imageServ = imageServ;
			_imageSharpServ = imageSharpServ;
		}

		//[AllowAnonymous]
		//[HttpGet("{id}")]
		//public async Task<IActionResult> GetImageBase64ByFileId(string id, [FromQuery] bool reduced, int width)
		//{
		//	var imageBase64 = await _imageServ.GetBase64ImageByFileId(id, reduced, width);

		//	return Ok(imageBase64);
		//}

		//[AllowAnonymous]
		//[HttpPost("quality/test")]
		//public async Task<IActionResult> QualitiTest([FromForm] IFormFile formFile)
  //      {
		//	await _imageSharpServ.TestQuality(formFile);

		//	return Ok();
  //      }

	}
}

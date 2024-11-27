using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quantum.Core.Services;
using Quantum.Core.Services.Contracts;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
	[Produces("application/json")]
	[Route("api/filetype")]
	[ApiController]
	[Authorize(LocalApi.PolicyName)]
	public class FileTypeController : Controller
    {
		private IFileTypeService _fileTypeServ;
		private ICLRTypeService _cLRTypeService;

		public FileTypeController(
			IFileTypeService fileTypeServ,
			ICLRTypeService cLRTypeService
		)
		{
			_fileTypeServ = fileTypeServ;
			_cLRTypeService = cLRTypeService;
		}


		[Authorize("create:file-type")]
		[HttpPost("")]
		public async Task<IActionResult> CreateFileType([FromForm] string Name)
		{
			await _fileTypeServ.InsertFileType(Name, User.Identity);

            return Ok();
		}

		[Authorize("create:file-type")]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateFileType([FromBody] string fileTypeName, string id)
		{
			await _fileTypeServ.UpdateFileType(fileTypeName, id, User.Identity);

			return Ok();
		}

		[Authorize("create:file-type")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFileType(string id)
		{
			await _fileTypeServ.DeleteFileType(id, User.Identity);

            return Ok();
		}

		[AllowAnonymous]
        [Authorize("create:file-type")]
        [HttpPost("clr")]
		public async Task<IActionResult> CreateClrType([FromForm] string name)
		{
			await _cLRTypeService.InsertClrType(name);

			return Ok();
		}
	}
}

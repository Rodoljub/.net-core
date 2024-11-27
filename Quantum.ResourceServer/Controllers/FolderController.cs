using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quantum.Core.Models.Folder;
using Quantum.Core.Services.Contracts;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
    [Produces("application/json")]
    [Route("api/folder")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class FolderController : Controller
    {
		private IFolderService _folderServ;

        public FolderController(
			IFolderService folderServ
		)
        {
			_folderServ = folderServ;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateFolder([FromBody] FolderModel model)
        {
			await _folderServ.CreateFolder(model, User.Identity);

			return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFolder([FromBody] FolderModel model, string id)
        {
			await _folderServ.UpdateFolder(id, model, User.Identity);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(string id)
        {
            await _folderServ.DeleteFolder(id, User.Identity);

            return Ok();
        }
    }
}

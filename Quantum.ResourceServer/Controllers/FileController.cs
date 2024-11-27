using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quantum.Core.Models.File;
using Quantum.Core.Services.Contracts;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
    [Produces("application/json")]
    [Route("api/file")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class FileController : Controller
    {
		private IFileService _fileServ;

        public FileController(
			IFileService fileServ
		)
        {
			_fileServ = fileServ;
        }


        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
  //      [HttpPost("")]
  //      public async Task<IActionResult> CreateFile([FromForm] FileModel model)
  //      {
		//	var fileId = await _fileServ.InsertFile(model, User.Identity);

		//	return Ok(fileId);
		//}

        /// <summary>
        /// Updates the file.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
   //     [HttpPut("{id}")]
   //     public async Task<IActionResult> UpdateFile([FromForm] FileModel model, string id)
   //     {
			//await _fileServ.UpdateFile(id, model, User.Identity);

			//return Ok();
   //     }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFile(string id)
        //{
        //    await _fileServ.DeleteFile(id, User.Identity);

        //    return Ok();
        //}


        [HttpGet("{id}")]
        public async Task<IActionResult> Getile(string id)
        {
            var file = await _fileServ.GetFile(id);

            return Ok(file);
        }

    }
}

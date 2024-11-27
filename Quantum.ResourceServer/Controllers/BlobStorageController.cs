using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quantum.Integration.External.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Quantum.ResourceServer.Controllers
{
    [EnableCors("GeneralPolicy")]
    [Produces("application/json")]
    [Route("api/blobs")]
    public class BlobStorageController : Controller
    {
        private IBlobsStorageService _blobStorageServ;
        public BlobStorageController(
            IBlobsStorageService blobStorageServ
        )
        {
            _blobStorageServ = blobStorageServ;

        }

        //[Authorize]
        //[HttpPost("CreateContainer")]
        //public async Task<IActionResult> CreateContainer([FromForm] string blobContainerName, bool createPublic = false)
        //{
        //    //Create a unique name for the container
        //    //string containerNameGuid = "quickstartblobs" + Guid.NewGuid().ToString();
        //    var containerClient = await _blobStorageServ.CreateBlobContainerAsync(blobContainerName, createPublic);

        //    return Ok();
        //}

        //[HttpPost("DeleteContainer")]
        //public async Task<IActionResult> DeleteContainer([FromForm] string blobContainerName)
        //{
        //    await _blobStorageServ.DeleteBlobContainerAsync(blobContainerName);

        //    return Ok();
        //}

        //[HttpPost("DeleteBlob")]
        //public async Task<IActionResult> DeleteBlob([FromForm] string blobContainerName, string fileName)
        //{
        //    var response = await _blobStorageServ.DeleteBlobFromContainer(blobContainerName, fileName);

        //    return Ok();
        //}

        //[HttpPost("ListBlobs")]
        //public async Task<IActionResult> ListBlobs([FromForm] string blobContainerName, int? segmentSize = null)
        //{
        //    await _blobStorageServ.GetBlobsFlatListening(blobContainerName, segmentSize);

        //    return Ok();
        //}

        //[HttpPost("GetContainer")]
        //public IActionResult GetContainer([FromForm] string blobContainerName)
        //{
        //    var containerClient = _blobStorageServ.GetContainerClient(blobContainerName);

        //    return Ok();
        //}

        //[HttpPost("UpdateBlobContentType")]
        //public IActionResult UpdateBlobContentType([FromForm] string blobContainerName, string fileName, string fileContentType)
        //{
        //    var containerClient = _blobStorageServ.UpdateBlobContentType(blobContainerName, fileName, fileContentType);

        //    return Ok();
        //}
    }
}

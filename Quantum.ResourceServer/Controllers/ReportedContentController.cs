using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quantum.Core.Models;
using Quantum.Core.Services;
using Quantum.Utility.Filters;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
	[Produces("application/json")]
	[Route("api/ReportedcontentReasons")]
	[ApiController]
	[Authorize(LocalApi.PolicyName)]
	public class ReportedContentController : Controller
    {
		private IReportedContentService _reportedContentServ;

		public ReportedContentController(
			IReportedContentService reportedContentServ
		)
		{
			_reportedContentServ = reportedContentServ;
		}

		[Authorize("create:reported-content-reasons")]
		[HttpPost("create")]
		public async Task<IActionResult> CreateReportedContentReasons([FromForm] ReportedContentReasonsModel model)
		{
			await _reportedContentServ.CreateReportedContentReasons(model, User.Identity);

			return Ok();
		}

		[HttpGet]
		public async Task<IActionResult> GetReportedContentReasons()
		{
			var user = User.Identity;
			var reportedContentReasons = await _reportedContentServ.GetReportedContentReasons();

			return Ok(reportedContentReasons);
		}

		[HttpPost]
		[ValidateModel]
		public async Task<IActionResult> SetReportedContent([FromBody] ReportedContentModel model)
		{
			await _reportedContentServ.SetReportedContent(model, User.Identity);
                
			return Ok();
		}
	}
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quantum.Core.Models.UserProfile;
using Quantum.Core.Services.Contracts;
using Quantum.Utility.Filters;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
	[EnableCors("GeneralPolicy")]
    [Produces("application/json")]
    [Route("api/profile")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class ProfileController : Controller
    {
        private IMemoryCache _cache;
        private IUserProfileService _userProfileServ;

        public ProfileController(
            IMemoryCache cache,
            IUserProfileService userProfileServ
        )
        {
            _cache = cache;
            _userProfileServ = userProfileServ;
        }


        [HttpGet()]
        public async Task<IActionResult> GetUserProfileViewModel()
        {
            var userProfileViewModel = await _userProfileServ.GetUserProfileViewModel(User.Identity);

            return Ok(userProfileViewModel);
        }

		[AllowAnonymous]
		[HttpGet("userProfile/urlSegment/{urlSegment}")]
		public async Task<IActionResult> GetUserProfileViewModelByUrlSegment(string urlSegment)
		{
			UserProfileViewModel userProfileViewModel = await _userProfileServ.GetUserProfileViewModelByUrlSegment(urlSegment);

			return Ok(userProfileViewModel);
		}

        [AllowAnonymous]
        [HttpGet("userProfile/email/{email}")]
        public async Task<IActionResult> GetProfileDetailsByEmail(string email)
        {
            var userProfileViewModel = await _userProfileServ.GetUserProfileViewModelByEmail(email);

            return Ok(userProfileViewModel);
        }

        [AllowAnonymous]
        [HttpGet("userProfile/counter")]
        public async Task<IActionResult> SetUserProfileCounters()
        {
            var savedProfileCounters = await _userProfileServ.SetUserProfileCounters();

            return Ok(savedProfileCounters);
        }

        [ValidateModel]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileModel model)
        {
            var profileModel = await _userProfileServ.UpdateProfile(model, User.Identity);

            return Ok(profileModel);
        }
    }
}
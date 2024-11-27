using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quantum.Data.Entities;
using Quantum.Data.Entities.Common;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
    {
		private QDbContext _context;
		private ILogger<UserProfileRepository> _logger;

		public UserProfileRepository(
			QDbContext context,
			ILogger<UserProfileRepository> logger)
			: base(context)
		{
			_context = context;
			_logger = logger;
		}


		public async Task<UserProfile> GetByUserId(string id)
		{
			return await base.Query(up => up != null && !up.IsDeleted && up.CreatedById == id)
				//.AsNoTracking()
				.Include(up => up.File)
				.FirstOrDefaultAsync();
		}

		public async Task<UserProfile> GetByUserIdNoTrack(string id)
		{
			return await base.Query(up => up != null && !up.IsDeleted && up.CreatedById == id, false)
				//.AsNoTracking()
				.Include(up => up.File)
				.FirstOrDefaultAsync();
		}

		public async Task<UserProfile> GetByUserIdAsNoTracking(string id)
		{
			return await base.Query(up => up != null && !up.IsDeleted && up.CreatedById == id )
                .AsNoTracking()
                .Include(up => up.File)
				.FirstOrDefaultAsync();
		}

        public async Task<bool> UrlSegmentAlreadyExists(string name)
        {
            return await base.Query(up => !up.IsDeleted && !up.IsDeleted && up.UrlSegment == name)
                .AnyAsync();
        }

		public async Task<string> GetUserProfileIdByUserId(string userId)
        {
			var uPId = await _context.UserProfiles.Where(up => !up.IsDeleted && up.CreatedById == userId)
				.Select(up => up.ID)
				.FirstOrDefaultAsync();
							
			return uPId;
								

        }

		public async Task<UserProfile> GetUserProfileByEmail(string email)
		{
			var userProfile = await base.Query(up => !up.IsDeleted && !up.IsDeleted && up.Email == email)
				.OrderByDescending(up => up.LastModified)
				.FirstOrDefaultAsync();

			if (userProfile == null)
			{
				throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
				   null,
				   Errors.ErrorUserProfileNotFound,
				   null,
				   $"User Profile cannot be found for the user profile email: '{email}'.");
			}

			return userProfile;
		}

		public async Task<UserProfile> GetUserProfileByUrlSegment(string urlSegment)
		{
			var userProfile = await base.Query(up => !up.IsDeleted && !up.IsDeleted && up.UrlSegment == urlSegment)
				.OrderByDescending(up => up.LastModified)
				.FirstOrDefaultAsync();

			if (userProfile == null)
			{
				throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
				   null,
				   Errors.ErrorUserProfileNotFound,
				   null,
				   $"User Profile cannot be found for the user profile urlSegment: '{urlSegment}'.");
			}

			return userProfile;
		}
	}
}

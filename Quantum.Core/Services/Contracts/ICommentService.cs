using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models;
using Quantum.Data.Models.ReadModels;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
    public interface ICommentService
	{
		Task<CommentViewModel> AddComment(CommentModel commentModel, IIdentity identity);

		Task<CommentViewModel> UpdateComment(CommentViewModel model, IIdentity identityUser);

		Task<bool> DeleteComment(string id, IIdentity identity);

		Task<IEnumerable<CommentViewModel>> GetViewCommentsModels(string ParentId, string[] initialCommentsIds, string typeName, int skip, IIdentity identity);
	}
}

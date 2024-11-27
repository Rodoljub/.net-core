using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface ICommentRepository : IBaseRepository<Comment, IdentityUser>
    {
		Task<Comment> InsertComment(Comment comment, IdentityUser user);

		Task<Comment> DeleteComment(string id, IdentityUser user);

		Task DeleteCommentMaxReported(string commentId);

		Task<IEnumerable<Comment>> GetChildComments(int skip, int take, string[] initialCommentsIds, string parentId, string parentTypeId);

		Task<int> CountComments(string parentId, string parentTypeId);

		Task UpdateParentCommentChildCount(string parentId, bool isCommentAdd);
	}
}

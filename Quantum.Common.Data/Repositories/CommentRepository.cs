using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
	{
		private QDbContext _context;

		public CommentRepository(QDbContext context) 
			: base(context)
		{
			_context = context;
		}

		public async Task<Comment> InsertComment(Comment comment, IdentityUser user)
		{
			await base.Insert(comment, user);

			await Task.FromResult(comment);

			if (comment == null)
			{
				throw new CommentNotSavedException(
					HttpStatusCode.BadRequest, Errors.ErrorCommentNotSaved);
			}

			return comment;
		}

		public async Task<Comment> DeleteComment(string id, IdentityUser user)
		{
			var comment = await base.GetById(id);

			if (comment != null)
			{
				if (user.Id == comment.CreatedById)
				{
					await base.Delete(id, user);

					return comment;
				}
			}

			throw new CommentNotDeletedException(HttpStatusCode.BadRequest, Errors.ErrorCommentNotDeleted);
		}

		public async Task DeleteCommentMaxReported(string commentId)
		{
			await base.Delete(commentId, null);
		}

		public async Task<IEnumerable<Comment>> GetChildComments(int skip, int take, string[] initialCommentsIds, string parentId, string parentTypeId)
		{
			var comments = await Query(co => !co.IsDeleted 
										&& co.ParentId == parentId 
										&& co.ParentTypeID == parentTypeId 
										&& !initialCommentsIds.Contains(co.ID))
				.OrderByDescending(co => co.CreatedDate)
				.Skip(skip)
				.Take(take)
				.ToListAsync();

			if(comments.Count == 0)
			{
				throw new CommentsNotFoundException(
					HttpStatusCode.BadRequest, Errors.ErrorCommentsNotFound);
			}

			return comments;
		}

		public async Task<int> CountComments(string parentId, string parentTypeId)
		{
			return await base.Query(co => !co.IsDeleted && co.ParentId == parentId && co.ParentTypeID == parentTypeId).CountAsync();
		}

		public async Task UpdateParentCommentChildCount(string parentId, bool isCommentAdd)
		{
			var parentComment = await base.GetById(parentId);

			if (parentComment != null || parentComment != default(Comment))
			{
				//if (isCommentAdd)
				//	parentComment.ChildCount = parentComment.ChildCount + 1;
				//else
				//	parentComment.ChildCount = parentComment.ChildCount - 1;

				var childCount = await base.Query(c => !c.IsDeleted && c.ParentId == parentId)
					.CountAsync();

				parentComment.ChildCount = childCount;

				await base.Update(parentComment, null);
			}
		}
	}
}

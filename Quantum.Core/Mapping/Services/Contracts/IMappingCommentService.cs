using Quantum.Core.Models;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services.Contracts
{
    public interface IMappingCommentService
	{
		Task<CommentViewModel> MapCommentViewModelFromComment(Comment comment, string userId);

		Task<Comment> MapCommentFromCommentModel(CommentModel commentModel, string clrTypeId, string userProfileId);
	}
}

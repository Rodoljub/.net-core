using AutoMapper;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;

namespace Quantum.Core.Mapping.Profiles
{
    public class CommentsMappingProfile : Profile
    {
		public CommentsMappingProfile()
		{
			CreateMap<CommentModel, Comment>();

			CreateMap<Comment, CommentViewModel>();
		}
    }
}

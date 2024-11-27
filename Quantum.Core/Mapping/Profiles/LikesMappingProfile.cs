using AutoMapper;
using Quantum.Core.Models;
using Quantum.Data.Entities;

namespace Quantum.Core.Mapping.Profiles
{
	public class LikesMappingProfile : Profile
	{
		public LikesMappingProfile()
		{
			CreateMap<LikeModel, Like>();
		}
	}
}

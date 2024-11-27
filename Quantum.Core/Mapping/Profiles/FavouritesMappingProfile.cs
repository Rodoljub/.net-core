using AutoMapper;
using Quantum.Core.Models;
using Quantum.Data.Entities;

namespace Quantum.Core.Mapping.Profiles
{
	public class FavouritesMappingProfile : Profile
	{
		public FavouritesMappingProfile()
		{
			CreateMap<FavouriteModel, Favourite>()
				.ForMember(f => f.ItemID,
				opt => opt.MapFrom((src, dest, destMember, resContext) => src.EntityId)); ;
		}
	}
}
